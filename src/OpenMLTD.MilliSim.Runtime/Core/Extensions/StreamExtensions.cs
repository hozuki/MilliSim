using System.IO;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Extensions {
    public static class StreamExtensions {

        public static byte[] ReadAll([NotNull] this Stream stream) {
            return ReadAll(stream, 10240);
        }

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
