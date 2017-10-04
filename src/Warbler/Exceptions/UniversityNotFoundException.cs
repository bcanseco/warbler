using System;
using System.Runtime.Serialization;

namespace Warbler.Exceptions
{
    /// <summary>
    ///   Indicates that a university search yielded zero results.
    /// </summary>
    public class UniversityNotFoundException : Exception
    {
        public UniversityNotFoundException()
        { }

        public UniversityNotFoundException(string message)
            : base(message)
        { }

        public UniversityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected UniversityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
