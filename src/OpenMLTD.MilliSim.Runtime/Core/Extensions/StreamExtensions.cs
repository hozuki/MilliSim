using System.IO;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Extensions {
    /// <summary>
    /// Stream extension methods.
    /// </summary>
    public static class StreamExtensions {

        /// <summary>
        /// Reads all data in a <see cref="Stream"/> to a byte array, using a buffer of default size.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>Read data.</returns>
        [NotNull]
        public static byte[] ReadAll([NotNull] this Stream stream) {
            return ReadAll(stream, 10240);
        }

        /// <summary>
        /// Reads all data in a <see cref="Stream"/> to a byte array.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns>Read data.</returns>
        [NotNull]
        public static byte[] ReadAll([NotNull] this Stream stream, int bufferSize) {
            var buffer = new byte[bufferSize];
            byte[] ret;

            using (var memoryStream = new MemoryStream()) {
                var read = 1;

                while (read > 0) {
                    read = stream.Read(buffer, 0, bufferSize);

                    if (read > 0) {
                        memoryStream.Write(buffer, 0, read);
                    }

                    if (read < bufferSize) {
                        break;
                    }
                }

                ret = memoryStream.ToArray();
            }

            return ret;
        }

    }
}
