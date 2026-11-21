using System.Buffers.Binary;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var options = VerifierOptions.Parse(args);
var verifier = new PackageArtifactVerifier(options);
return verifier.Run();

internal sealed record VerifierOptions(
    string PackageDirectory,
    string? ExpectedCommit,
    bool RequireDeterministicPaths)
{
    public static VerifierOptions Parse(string[] args)
    {
        string? packageDirectory = null;
        string? expectedCommit = null;
        var requireDeterministicPaths = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--package-dir":
                    if (i + 1 >= args.Length)
                    {
                        throw new InvalidOperationException("Missing value for --package-dir.");
                    }

                    packageDirectory = args[++i];
                    break;
                case "--expected-commit":
                    if (i + 1 >= args.Length)
                    {
                        throw new InvalidOperationException("Missing value for --expected-commit.");
                    }

                    expectedCommit = args[++i].Trim();
                    break;
                case "--require-deterministic-paths":
                    requireDeterministicPaths = true;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown argument: '{args[i]}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(packageDirectory))
        {
            throw new InvalidOperationException("Usage: PackageArtifactVerifier --package-dir <path> [--expected-commit <sha>] [--require-deterministic-paths]");
        }

        var fullDirectory = Path.GetFullPath(packageDirectory);
        if (!Directory.Exists(fullDirectory))
        {
            throw new DirectoryNotFoundException($"Package directory does not exist: '{fullDirectory}'.");
        }

        if (expectedCommit is not null && !Regex.IsMatch(expectedCommit, "^[0-9a-f]{40}$"))
        {
            throw new InvalidOperationException($"--expected-commit must be a 40-character lowercase hex SHA, got '{expectedCommit}'.");
        }

        return new VerifierOptions(fullDirectory, expectedCommit, requireDeterministicPaths);
    }
}

internal sealed class PackageArtifactVerifier
{
    private static readonly Guid SourceLinkGuid = new("cc110556-a091-4d38-9fec-25ab9a351a6a");
    private static readonly Guid EmbeddedSourceGuid = new("0e8a571b-6926-466e-b4ad-8ab04611f5fe");
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static readonly string[] ExpectedPackageIds =
    [
        "MELT",
        "MELT.AspNetCore",
        "MELT.Serilog",
        "MELT.Serilog.AspNetCore",
        "MELT.Xunit",
        "MELT.Xunit.v3"
    ];

    private readonly VerifierOptions _options;

    public PackageArtifactVerifier(VerifierOptions options)
    {
        _options = options;
    }

    public int Run()
    {
        var nupkgPaths = Directory
            .GetFiles(_options.PackageDirectory, "*.nupkg", SearchOption.TopDirectoryOnly)
            .Where(path => !path.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (nupkgPaths.Length == 0)
        {
            throw new InvalidOperationException($"No .nupkg files were found in '{_options.PackageDirectory}'.");
        }

        var foundIds = new List<string>(nupkgPaths.Length);
        string? commonVersion = null;
        string? commonCommit = _options.ExpectedCommit;

        foreach (var nupkgPath in nupkgPaths)
        {
            var result = VerifyPackage(nupkgPath, ref commonVersion, ref commonCommit);
            foundIds.Add(result.PackageId);
            Console.WriteLine(
                $"Verified {result.PackageId} {result.Version} (commit {result.Commit[..8]}, " +
                $"{result.DllCount} DLL/PDB pair(s), {result.EmbeddedSourceCount} embedded source(s), " +
                $"icon {result.IconBytes} bytes)");
        }

        var duplicateIds = foundIds
            .GroupBy(id => id, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        if (duplicateIds.Length > 0)
        {
            throw new InvalidOperationException($"Duplicate package IDs found: {string.Join(", ", duplicateIds)}.");
        }

        var missingIds = ExpectedPackageIds.Except(foundIds, StringComparer.Ordinal).ToArray();
        var unexpectedIds = foundIds.Except(ExpectedPackageIds, StringComparer.Ordinal).ToArray();
        if (missingIds.Length > 0 || unexpectedIds.Length > 0)
        {
            throw new InvalidOperationException(
                $"Package inventory mismatch. Missing: [{string.Join(", ", missingIds)}]; Unexpected: [{string.Join(", ", unexpectedIds)}].");
        }

        Console.WriteLine($"All {ExpectedPackageIds.Length} MELT packages passed artifact, symbol, and Source Link verification.");
        return 0;
    }

    private PackageVerificationResult VerifyPackage(string nupkgPath, ref string? commonVersion, ref string? commonCommit)
    {
        using var nupkg = ZipFile.OpenRead(nupkgPath);

        var nuspecEntry = GetSingleRootNuspec(nupkg, nupkgPath);
        var (packageId, version, commit) = VerifyNuspec(nuspecEntry, nupkgPath, ref commonVersion, ref commonCommit);

        var expectedSnupkgName = $"{packageId}.{version}.snupkg";
        var snupkgPath = Path.Combine(_options.PackageDirectory, expectedSnupkgName);
        if (!File.Exists(snupkgPath))
        {
            throw new FileNotFoundException($"Missing matching symbol package '{expectedSnupkgName}' for '{Path.GetFileName(nupkgPath)}'.", snupkgPath);
        }

        var iconBytes = VerifyIcon(nupkg, packageId);
        VerifyReadme(nupkg, packageId, version, commit);

        using var snupkg = ZipFile.OpenRead(snupkgPath);
        var (dllCount, embeddedSourceCount) = VerifyAssembliesAndSymbols(nupkg, snupkg, packageId, commit);

        return new PackageVerificationResult(packageId, version, commit, dllCount, embeddedSourceCount, iconBytes);
    }

    private static ZipArchiveEntry GetSingleRootNuspec(ZipArchive nupkg, string nupkgPath)
    {
        var nuspecs = nupkg.Entries
            .Where(entry =>
                entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase) &&
                !entry.FullName.Contains('/') &&
                !entry.FullName.Contains('\\'))
            .ToArray();

        if (nuspecs.Length != 1)
        {
            throw new InvalidOperationException($"Expected exactly 1 root .nuspec in '{Path.GetFileName(nupkgPath)}', found {nuspecs.Length}.");
        }

        return nuspecs[0];
    }

    private static (string PackageId, string Version, string Commit) VerifyNuspec(
        ZipArchiveEntry nuspecEntry,
        string nupkgPath,
        ref string? commonVersion,
        ref string? commonCommit)
    {
        XDocument doc;
        using (var stream = nuspecEntry.Open())
        {
            doc = XDocument.Load(stream);
        }

        var metadata = doc.Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "metadata")
            ?? throw new InvalidOperationException($"Missing <metadata> element in '{nuspecEntry.FullName}' of '{Path.GetFileName(nupkgPath)}'.");

        var packageId = GetRequiredElementValue(metadata, "id", nuspecEntry.FullName);
        var version = GetRequiredElementValue(metadata, "version", nuspecEntry.FullName);

        if (commonVersion is null)
        {
            commonVersion = version;
        }
        else if (!string.Equals(commonVersion, version, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' has version '{version}', expected '{commonVersion}'.");
        }

        var repository = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "repository")
            ?? throw new InvalidOperationException($"Missing <repository> element in '{packageId}' nuspec.");

        var repoType = repository.Attribute("type")?.Value;
        if (!string.Equals(repoType, "git", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' has repository type '{repoType}', expected 'git'.");
        }

        var repoUrl = repository.Attribute("url")?.Value;
        if (!string.Equals(repoUrl, "https://github.com/alefranz/MELT.git", StringComparison.Ordinal) &&
            !string.Equals(repoUrl, "https://github.com/alefranz/MELT", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' has unexpected repository url '{repoUrl}'.");
        }

        var commit = repository.Attribute("commit")?.Value ?? string.Empty;
        if (!Regex.IsMatch(commit, "^[0-9a-f]{40}$"))
        {
            throw new InvalidOperationException($"Package '{packageId}' has invalid repository commit '{commit}'.");
        }

        if (commonCommit is null)
        {
            commonCommit = commit;
        }
        else if (!string.Equals(commonCommit, commit, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' has commit '{commit}', expected '{commonCommit}'.");
        }

        var releaseNotes = GetRequiredElementValue(metadata, "releaseNotes", packageId);
        var expectedReleaseNotes = $"https://github.com/alefranz/MELT/blob/{commit}/docs/RELEASE-NOTES-v2.md";
        if (!string.Equals(releaseNotes, expectedReleaseNotes, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' releaseNotes is '{releaseNotes}', expected '{expectedReleaseNotes}'.");
        }

        var icon = GetRequiredElementValue(metadata, "icon", packageId);
        if (!string.Equals(icon, "logo.png", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' icon is '{icon}', expected 'logo.png'.");
        }

        var readme = GetRequiredElementValue(metadata, "readme", packageId);
        if (!string.Equals(readme, "README.md", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' readme is '{readme}', expected 'README.md'.");
        }

        return (packageId, version, commit);
    }

    private static string GetRequiredElementValue(XElement parent, string localName, string context)
    {
        var value = parent.Elements().FirstOrDefault(e => e.Name.LocalName == localName)?.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing or empty <{localName}> in '{context}'.");
        }

        return value.Trim();
    }

    private static int VerifyIcon(ZipArchive nupkg, string packageId)
    {
        var iconEntry = nupkg.GetEntry("logo.png")
            ?? throw new InvalidOperationException($"Package '{packageId}' is missing root 'logo.png'.");

        var bytes = ReadEntryBytes(iconEntry);
        if (bytes.Length >= 65536)
        {
            throw new InvalidOperationException($"Package '{packageId}' icon 'logo.png' is {bytes.Length} bytes, expected < 65536 bytes.");
        }

        if (bytes.Length < 24 || !bytes.AsSpan(0, 8).SequenceEqual(PngSignature))
        {
            throw new InvalidOperationException($"Package '{packageId}' icon 'logo.png' does not have a valid PNG header.");
        }

        var ihdrLength = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(8, 4));
        var chunkType = Encoding.ASCII.GetString(bytes, 12, 4);
        if (ihdrLength != 13 || !string.Equals(chunkType, "IHDR", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' icon 'logo.png' has invalid IHDR chunk ('{chunkType}', length {ihdrLength}).");
        }

        var width = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4));
        var height = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4));
        if (width != 128 || height != 128)
        {
            throw new InvalidOperationException($"Package '{packageId}' icon 'logo.png' dimensions are {width}x{height}, expected 128x128.");
        }

        return bytes.Length;
    }

    private static void VerifyReadme(ZipArchive nupkg, string packageId, string version, string commit)
    {
        var readmeEntry = nupkg.GetEntry("README.md")
            ?? throw new InvalidOperationException($"Package '{packageId}' is missing root 'README.md'.");

        var bytes = ReadEntryBytes(readmeEntry);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            throw new InvalidOperationException($"Package '{packageId}' README.md must not contain a UTF-8 BOM.");
        }

        var text = Encoding.UTF8.GetString(bytes).Replace("\r\n", "\n", StringComparison.Ordinal);
        if (text.Contains("__MELT_VERSION__", StringComparison.Ordinal) ||
            text.Contains("__MELT_COMMIT__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' README.md contains unreplaced template placeholders.");
        }

        if (!text.StartsWith("# MELT\n\n", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' README.md does not preserve the leading '# MELT\\n\\n' heading separation.");
        }

        var blankLineSeparatorCount = Regex.Matches(text, "\n\n").Count;
        if (blankLineSeparatorCount < 5)
        {
            throw new InvalidOperationException(
                $"Package '{packageId}' README.md appears to have stripped blank lines (found {blankLineSeparatorCount} paragraph separators).");
        }

        var expectedInstallSnippet = $"Install version `{version}`";
        if (!text.Contains(expectedInstallSnippet, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' README.md does not contain '{expectedInstallSnippet}'.");
        }

        var expectedGuideUrl = $"https://github.com/alefranz/MELT/blob/{commit}/docs/guide.md";
        if (!text.Contains(expectedGuideUrl, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Package '{packageId}' README.md does not contain commit-pinned guide URL '{expectedGuideUrl}'.");
        }
    }

    private (int DllCount, int EmbeddedSourceCount) VerifyAssembliesAndSymbols(
        ZipArchive nupkg,
        ZipArchive snupkg,
        string packageId,
        string commit)
    {
        var dllEntries = GetLibEntries(nupkg, ".dll");
        var pdbEntries = GetLibEntries(snupkg, ".pdb");

        if (dllEntries.Count == 0)
        {
            throw new InvalidOperationException($"Package '{packageId}' does not contain any lib/<tfm>/*.dll entries.");
        }

        var expectedPdbKeys = dllEntries.Keys
            .Select(path => Path.ChangeExtension(path, ".pdb")!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingPdbs = expectedPdbKeys.Except(pdbEntries.Keys, StringComparer.OrdinalIgnoreCase).ToArray();
        var orphanPdbs = pdbEntries.Keys.Except(expectedPdbKeys, StringComparer.OrdinalIgnoreCase).ToArray();
        if (missingPdbs.Length > 0 || orphanPdbs.Length > 0)
        {
            throw new InvalidOperationException(
                $"Package '{packageId}' DLL/PDB mismatch. Missing PDBs: [{string.Join(", ", missingPdbs)}]; Orphan PDBs: [{string.Join(", ", orphanPdbs)}].");
        }

        var totalEmbeddedSources = 0;

        foreach (var (dllPath, dllEntry) in dllEntries.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            var pdbPath = Path.ChangeExtension(dllPath, ".pdb")!;
            var pdbEntry = pdbEntries[pdbPath];

            using var dllStream = ReadEntryToMemoryStream(dllEntry);
            using var pdbStream = ReadEntryToMemoryStream(pdbEntry);

            totalEmbeddedSources += VerifyDllPdbPair(dllStream, pdbStream, packageId, dllPath, pdbPath, commit);
        }

        return (dllEntries.Count, totalEmbeddedSources);
    }

    private static Dictionary<string, ZipArchiveEntry> GetLibEntries(ZipArchive archive, string extension)
    {
        var result = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in archive.Entries)
        {
            var normalized = entry.FullName.Replace('\\', '/');
            var segments = normalized.Split('/');
            if (segments.Length == 3 &&
                string.Equals(segments[0], "lib", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(segments[1]) &&
                segments[2].EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(normalized, entry);
            }
        }

        return result;
    }

    private int VerifyDllPdbPair(
        MemoryStream dllStream,
        MemoryStream pdbStream,
        string packageId,
        string dllPath,
        string pdbPath,
        string commit)
    {
        using var peReader = new PEReader(dllStream);
        var codeViewEntries = peReader.ReadDebugDirectory()
            .Where(entry => entry.Type == DebugDirectoryEntryType.CodeView)
            .ToArray();

        if (codeViewEntries.Length != 1)
        {
            throw new InvalidOperationException(
                $"Package '{packageId}' assembly '{dllPath}' expected 1 CodeView debug directory entry, found {codeViewEntries.Length}.");
        }

        var codeViewDirectory = codeViewEntries[0];
        if (!codeViewDirectory.IsPortableCodeView)
        {
            throw new InvalidOperationException($"Package '{packageId}' assembly '{dllPath}' does not use Portable CodeView debug format.");
        }

        var codeViewData = peReader.ReadCodeViewDebugDirectoryData(codeViewDirectory);
        if (codeViewData.Age != 1)
        {
            throw new InvalidOperationException($"Package '{packageId}' assembly '{dllPath}' CodeView age is {codeViewData.Age}, expected 1.");
        }

        using var pdbProvider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdbReader = pdbProvider.GetMetadataReader();
        var debugHeader = pdbReader.DebugMetadataHeader
            ?? throw new InvalidOperationException($"Package '{packageId}' symbol file '{pdbPath}' is missing a DebugMetadataHeader.");

        var pdbId = new BlobContentId(debugHeader.Id);
        if (pdbId.Guid != codeViewData.Guid || pdbId.Stamp != codeViewDirectory.Stamp)
        {
            throw new InvalidOperationException(
                $"Package '{packageId}' PDB '{pdbPath}' (Guid={pdbId.Guid}, Stamp={pdbId.Stamp:X8}) " +
                $"does not match DLL '{dllPath}' (Guid={codeViewData.Guid}, Stamp={codeViewDirectory.Stamp:X8}).");
        }

        string? sourceLinkJson = null;
        var embeddedDocuments = new HashSet<DocumentHandle>();

        foreach (var handle in pdbReader.CustomDebugInformation)
        {
            var info = pdbReader.GetCustomDebugInformation(handle);
            var kind = pdbReader.GetGuid(info.Kind);

            if (kind == SourceLinkGuid)
            {
                if (sourceLinkJson is not null)
                {
                    throw new InvalidOperationException($"Package '{packageId}' PDB '{pdbPath}' contains multiple Source Link records.");
                }

                sourceLinkJson = Encoding.UTF8.GetString(pdbReader.GetBlobBytes(info.Value));
            }
            else if (kind == EmbeddedSourceGuid && info.Parent.Kind == HandleKind.Document)
            {
                embeddedDocuments.Add((DocumentHandle)info.Parent);
            }
        }

        if (string.IsNullOrWhiteSpace(sourceLinkJson))
        {
            throw new InvalidOperationException($"Package '{packageId}' PDB '{pdbPath}' is missing Source Link custom debug information.");
        }

        if (embeddedDocuments.Count == 0)
        {
            throw new InvalidOperationException($"Package '{packageId}' PDB '{pdbPath}' contains no embedded untracked source documents.");
        }

        var sourceLinkMappings = ParseAndVerifySourceLink(sourceLinkJson, packageId, pdbPath, commit);

        foreach (var documentHandle in pdbReader.Documents)
        {
            var document = pdbReader.GetDocument(documentHandle);
            var documentName = pdbReader.GetString(document.Name);
            if (embeddedDocuments.Contains(documentHandle))
            {
                continue;
            }

            if (!MatchesAnySourceLinkMapping(documentName, sourceLinkMappings))
            {
                throw new InvalidOperationException(
                    $"Package '{packageId}' PDB '{pdbPath}' document '{documentName}' is neither embedded nor mapped by Source Link.");
            }
        }

        return embeddedDocuments.Count;
    }

    private List<string> ParseAndVerifySourceLink(string sourceLinkJson, string packageId, string pdbPath, string commit)
    {
        using var json = JsonDocument.Parse(sourceLinkJson);
        if (!json.RootElement.TryGetProperty("documents", out var documentsElement) ||
            documentsElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException($"Package '{packageId}' PDB '{pdbPath}' Source Link JSON is missing a 'documents' object.");
        }

        var expectedExactUrl = $"https://raw.githubusercontent.com/alefranz/MELT/{commit}/*";
        var expectedUrlPrefix = $"https://raw.githubusercontent.com/alefranz/MELT/{commit}/";
        var keys = new List<string>();

        foreach (var property in documentsElement.EnumerateObject())
        {
            var localKey = property.Name;
            var remoteUrl = property.Value.GetString() ?? string.Empty;

            if (!string.Equals(remoteUrl, expectedExactUrl, StringComparison.Ordinal) &&
                !remoteUrl.StartsWith(expectedUrlPrefix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Package '{packageId}' PDB '{pdbPath}' Source Link maps '{localKey}' to '{remoteUrl}', expected commit-pinned URL under '{expectedUrlPrefix}'.");
            }

            if (_options.RequireDeterministicPaths && !localKey.StartsWith("/_/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Package '{packageId}' PDB '{pdbPath}' Source Link key '{localKey}' is not deterministic (expected prefix '/_/').");
            }

            keys.Add(localKey);
        }

        if (keys.Count == 0)
        {
            throw new InvalidOperationException($"Package '{packageId}' PDB '{pdbPath}' Source Link 'documents' object is empty.");
        }

        return keys;
    }

    private static bool MatchesAnySourceLinkMapping(string documentName, List<string> sourceLinkKeys)
    {
        foreach (var key in sourceLinkKeys)
        {
            if (key.EndsWith('*'))
            {
                var prefix = key[..^1];
                if (documentName.StartsWith(prefix, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            else if (string.Equals(documentName, key, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static byte[] ReadEntryBytes(ZipArchiveEntry entry)
    {
        using var stream = ReadEntryToMemoryStream(entry);
        return stream.ToArray();
    }

    private static MemoryStream ReadEntryToMemoryStream(ZipArchiveEntry entry)
    {
        var memoryStream = new MemoryStream();
        using (var entryStream = entry.Open())
        {
            entryStream.CopyTo(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private sealed record PackageVerificationResult(
        string PackageId,
        string Version,
        string Commit,
        int DllCount,
        int EmbeddedSourceCount,
        int IconBytes);
}
