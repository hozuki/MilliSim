using System;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Theater.Internal {
    internal static class ComponentFactory {

        internal static T CreateAndAdd<T>([NotNull] IComponentContainer parent) where T : IComponent {
            var isVisual = typeof(T).ImplementsInterface(typeof(IVisual));
            var ctorTypes = isVisual ? IVisualCtorTypes : IComponentCtorTypes;
            var ctor = typeof(T).GetConstructor(CtorFlags, null, ctorTypes, null);
            if (ctor == null) {
                throw new MissingMethodException();
            }
            var obj = (T)ctor.Invoke(new object[] { parent });
            return obj;
        }

        private static readonly BindingFlags CtorFlags = BindingFlags.Instance | BindingFlags.Public;

        private static readonly Type[] IVisualCtorTypes = { typeof(IVisualContainer) };
        private static readonly Type[] IComponentCtorTypes = { typeof(IComponentContainer) };

    }
}
