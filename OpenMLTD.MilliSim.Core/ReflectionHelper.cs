using System;
using System.Linq;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    public static class ReflectionHelper {

        public static bool ImplementsInterface<T>([NotNull] Type type) {
            var tt = typeof(T);
            return ImplementsInterface(type, tt);
        }

        public static bool ImplementsInterface([NotNull] Type type, [NotNull] Type interfaceType) {
            if (!interfaceType.IsInterface) {
                throw new ArgumentException("T must be an interface type.", nameof(interfaceType));
            }
            return type.GetInterfaces().Any(@if => @if == interfaceType);
        }

        public static bool ImplementsGenericInterface<T>([NotNull] Type type) {
            var tt = typeof(T);
            return ImplementsGenericInterface(type, tt);
        }

        public static bool ImplementsGenericInterface([NotNull] Type type, [NotNull] Type interfaceType) {
            if (!interfaceType.IsInterface || !interfaceType.IsGenericType) {
                throw new ArgumentException("T must be an generic interface type.", nameof(interfaceType));
            }
            return type.GetInterfaces().Any(@if => @if.IsGenericType && @if.GetGenericTypeDefinition() == interfaceType);
        }

        public static readonly object[] EmptyObjects = new object[0];

    }
}
