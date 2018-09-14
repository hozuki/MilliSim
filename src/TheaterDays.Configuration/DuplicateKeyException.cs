using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace OpenMLTD.TheaterDays.Configuration {
    public sealed class DuplicateKeyException : ApplicationException {

        public DuplicateKeyException() {
        }

        public DuplicateKeyException(string message)
            : base(message) {
        }

        public DuplicateKeyException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public DuplicateKeyException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

    }
}
