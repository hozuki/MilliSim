using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Extensions;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class BaseGameComponentFactory : DisposableBase, IBaseGameComponentFactory {

        public int ApiVersion => 1;

        public string PluginCategory => "Base Game Component Factory";

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public abstract IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent);

        public static BaseGameComponent CreateAndAddTo([NotNull] IBaseGameComponentContainer parent, [NotNull] Type type) {
            if (!type.IsSubclassOf(typeof(BaseGameComponent))) {
                throw new ArgumentException($"{type} is not a subclass of {typeof(BaseGameComponent)}");
            }

            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (ctors.Length > 0) {
                ctors = ctors.Where(c => {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 &&
                           (parameters[0].ParameterType == typeof(IBaseGameComponentContainer) || parameters[0].ParameterType.ImplementsInterface(typeof(IBaseGameComponentContainer)));
                }).ToArray();
            }

            if (ctors.Length == 0) {
                throw new MissingMethodException("Cannot find a proper constructor.");
            }

            var parentType = parent.GetType();
            var ctor = (from c in ctors
                        let param = c.GetParameters()[0]
                        where param.ParameterType == parentType
                        select c).FirstOrDefault();

            if (ctor == null) {
                ctor = ctors[0];
            }

            return (BaseGameComponent)ctor.Invoke(new object[] { parent });
        }

        public static T CreateAndAddTo<T>([NotNull] IBaseGameComponentContainer parent) where T : BaseGameComponent {
            var t = typeof(T);
            var c = CreateAndAddTo(parent, t);
            return (T)c;
        }

        protected override void Dispose(bool disposing) {
        }

    }
}
