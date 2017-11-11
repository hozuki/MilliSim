using OpenMLTD.MilliSim.Core;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public abstract class AudioObject : DisposableBase {

        public static string Version => AL.Get(ALGetString.Version);

        public static string Vendor => AL.Get(ALGetString.Vendor);

        public static string Renderer => AL.Get(ALGetString.Renderer);

        public static string[] Extensions {
            get {
                var str = AL.Get(ALGetString.Extensions);
                return str.Split(' ');
            }
        }

    }
}
