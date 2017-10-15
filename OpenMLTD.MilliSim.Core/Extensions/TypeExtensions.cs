using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Extensions {
    public static class TypeExtensions {

        public static bool ImplementsInterface<T>([NotNull] this Type type) {
            return ReflectionHelper.ImplementsInterface<T>(type);
        }

        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            return ReflectionHelper.ImplementsInterface(type, interfaceType);
        }

        public static bool ImplementsGenericInterface<T>([NotNull] this Type type) {
            return ReflectionHelper.ImplementsGenericInterface<T>(type);
        }

        public static bool ImplementsGenericInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            return ReflectionHelper.ImplementsGenericInterface(type, interfaceType);
        }

    }
}
