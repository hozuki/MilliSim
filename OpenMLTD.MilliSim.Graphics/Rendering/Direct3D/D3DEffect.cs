using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering.Direct3D {
    public abstract class D3DEffect : DisposableBase {

        protected D3DEffect(Device device, string textSource, bool sourceIsText) {
            _device = device;
            _textSource = textSource;
            _sourceIsText = sourceIsText;
        }

        internal Effect NativeEffect => _effect;

        internal void Compile() {
            var device = _device;

            if (_sourceIsText) {
                var textSource = _textSource;
                // https://github.com/hozuki/noire_history/blob/f74fa79d4cc6355d2561a7259663b52f69d7ee80/Noire.Graphics.D3D11/FX/EffectBase11.cs#L24-L27
                using (var compilationResult = ShaderBytecode.Compile(textSource, null, "fx_5_0")) {
                    if (compilationResult.HasErrors || compilationResult.Bytecode == null) {
                        throw new SharpDXException(compilationResult.ResultCode, compilationResult.Message);
                    }
                    _effect = new Effect(device, compilationResult.Bytecode);
                }
            } else {
                var fileName = _textSource;
                fileName = Path.GetFullPath(fileName);
                if (!File.Exists(fileName)) {
                    throw new FileNotFoundException($"Effect file '{fileName}' is not found.", fileName);
                }

                var fileInfo = new FileInfo(fileName);
                using (var includeProcessor = new IncludeProcessor(fileInfo.DirectoryName)) {
                    using (var compilationResult = ShaderBytecode.CompileFromFile(fileName, null, "fx_5_0", ShaderFlags.None, EffectFlags.None, null, includeProcessor)) {
                        if (compilationResult.HasErrors || compilationResult.Bytecode == null) {
                            throw new SharpDXException(compilationResult.ResultCode, compilationResult.Message);
                        }
                        _effect = new Effect(device, compilationResult.Bytecode, EffectFlags.None, fileName);
                    }
                }
            }
        }

        internal abstract void Initialize();

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Utilities.Dispose(ref _effect);
            }
        }

        protected T ReadStruct<T>(EffectVariable variable) where T : struct {
            T result;
            var stride = Marshal.SizeOf(typeof(T));
            using (var dataStream = variable.GetRawValue(stride)) {
                result = dataStream.Read<T>();
            }
            return result;
        }

        protected void WriteStruct<T>(EffectVariable variable, T value) where T : struct {
            var stride = Marshal.SizeOf(typeof(T));
            var bytes = InteropHelper.StructureToBytes(value);
            using (var dataStream = DataStream.Create(bytes, false, false)) {
                variable.SetRawValue(dataStream, stride);
            }
        }

        protected T GetResource<T>(EffectShaderResourceVariable variable) where T : Resource {
            var view = variable.GetResource();
            return view?.ResourceAs<T>();
        }

        protected void SetResource<T>(EffectShaderResourceVariable variable, T value, [CallerMemberName, NotNull] string callerName = CallerHelper.EmptyName) where T : Resource {
            _createdResourceViews.TryGetValue(callerName, out var lastView);
            Utilities.Dispose(ref lastView);
            var newView = new ShaderResourceView(value.Device, value);
            _createdResourceViews[callerName] = newView;
            variable.SetResource(newView);
        }

        protected void SetResource<T>(EffectShaderResourceVariable variable, T value, ShaderResourceViewDescription description, [CallerMemberName, NotNull] string callerName = CallerHelper.EmptyName) where T : Resource {
            _createdResourceViews.TryGetValue(callerName, out var lastView);
            Utilities.Dispose(ref lastView);
            var newView = new ShaderResourceView(value.Device, value, description);
            _createdResourceViews[callerName] = newView;
            variable.SetResource(newView);
        }

        private Effect _effect;
        private readonly Device _device;
        private readonly string _textSource;
        private readonly bool _sourceIsText;

        private readonly Dictionary<string, ShaderResourceView> _createdResourceViews = new Dictionary<string, ShaderResourceView>();

    }
}
