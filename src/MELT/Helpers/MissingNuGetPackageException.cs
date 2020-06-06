using System;
using System.Runtime.Serialization;

namespace MELT
{
    [Serializable]
    internal class MissingNuGetPackageException : Exception
    {
        public MissingNuGetPackageException()
        {
        }

        public MissingNuGetPackageException(string message) : base(message)
        {
        }

        public MissingNuGetPackageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingNuGetPackageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
