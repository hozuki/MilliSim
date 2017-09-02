using System;
using System.IO;
using System.Text;
using SharpDX;
using SharpDX.D3DCompiler;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    internal sealed class IncludeProcessor : DisposeBase, Include {
        internal IncludeProcessor()
            : this(string.Empty) {
        }

        internal IncludeProcessor(string baseDirectory) {
            _baseDirectory = baseDirectory;
        }

        public IDisposable Shadow { get; set; }

        public Stream Open(IncludeType type, string fileName, Stream parentStream) {
            switch (type) {
                case IncludeType.Local:
                    fileName = Path.Combine(_baseDirectory, fileName);
                    break;
                case IncludeType.System:
                    break;
                default:
                    break;
            }
            // .fx 支持的字符集是 ASCII，而不是 UTF-8 或其他，这就是为什么 SharpDX 在进行文件预处理的时候使用的是 Encoding.ASCII。不过 fxc 看起来更智能一些，即使是带 BOM 头
            // 的 UTF-8 也是能读取和编译的。有些 .fx（在 Windows 下）保存时选择的“UTF-8”实际上都是带 BOM 头的，所以这些文件就出现了 BOM 头。但是为什么 ShaderBytecode.CompileFromFile
            // 又是正常的？
            // 我这里仿照的是（推测的）fxc 的处理逻辑。
            using (var streamReader = new StreamReader(fileName, Encoding.UTF8, true)) {
                var text = streamReader.ReadToEnd();
                var textBytes = Encoding.ASCII.GetBytes(text);
                return new MemoryStream(textBytes);
            }
        }

        public void Close(Stream stream) {
            Utilities.Dispose(ref stream);
        }

        protected override void Dispose(bool disposing) {
            Close(null);
            Shadow?.Dispose();
        }

        private readonly string _baseDirectory;

    }
}
