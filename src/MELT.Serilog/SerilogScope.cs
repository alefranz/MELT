namespace MELT
{
    /// <summary>
    /// Represents a Serilog-specific logging scope for use in tests.
    /// </summary>
    public class SerilogScope : Scope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogScope"/> class.
        /// </summary>
        /// <param name="scope">The scope object to capture.</param>
        public SerilogScope(object? scope) : base(scope)
        {
        }
    }
}
