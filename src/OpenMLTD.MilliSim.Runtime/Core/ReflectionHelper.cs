using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// Helper functions for reflection operations.
    /// </summary>
    public static class ReflectionHelper {

        /// <summary>
        /// Gets full caller name.
        /// Note that this method is not efficient.
        /// </summary>
        /// <returns>The caller name.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static string GetCallerName() {
            var parentFrame = new StackFrame(1);
            var parentMethod = parentFrame.GetMethod();

            var declaringType = parentMethod.DeclaringType;
            var declaringTypeName = declaringType?.Name ?? "<global>";

            return $"{declaringTypeName}.{parentMethod.Name}";
        }

        /// <summary>
        /// Gets the caller name. This method can only retrieve the method name of the caller.
        /// </summary>
        /// <param name="callerName">Set by .NET Framework. Do not assign values to it.</param>
        /// <returns>The caller name.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static string GetCallerNameFast([NotNull, CallerMemberName] string callerName = CallerHelper.EmptyName) {
            return callerName ?? throw new ArgumentNullException(nameof(callerName));
        }

        /// <summary>
        /// Checks if a type implements a specified interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface to check against.</typeparam>
        /// <param name="type">The type to be checked.</param>
        /// <returns><see langword="true"/> if that type implements the interface, otherwise <see langword="false"/>.</returns>
        public static bool ImplementsInterface<T>([NotNull] this Type type) {
            var tt = typeof(T);
            return ImplementsInterface(type, tt);
        }

        /// <summary>
        /// Checks if a type implements a specified interface.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <param name="interfaceType">The type of the interface to check against.</param>
        /// <returns><see langword="true"/> if that type implements the interface, otherwise <see langword="false"/>.</returns>
        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            if (!interfaceType.IsInterface) {
                throw new ArgumentException("T must be an interface type.", nameof(interfaceType));
            }
            return type.GetInterfaces().Any(@if => @if == interfaceType);
        }

        /// <summary>
        /// Checks if a type implements a specified generic interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface to check against.</typeparam>
        /// <param name="type">The type to be checked.</param>
        /// <returns><see langword="true"/> if that type implements the interface, otherwise <see langword="false"/>.</returns>
        public static bool ImplementsGenericInterface<T>([NotNull] this Type type) {
            var tt = typeof(T);
            return ImplementsGenericInterface(type, tt);
        }

        /// <summary>
        /// Checks if a type implements a specified generic interface.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <param name="interfaceType">The type of the interface to check against.</param>
        /// <returns><see langword="true"/> if that type implements the interface, otherwise <see langword="false"/>.</returns>
        public static bool ImplementsGenericInterface([NotNull] this Type type, [NotNull] Type interfaceType) {
            if (!interfaceType.IsInterface || !interfaceType.IsGenericType) {
                throw new ArgumentException("T must be an generic interface type.", nameof(interfaceType));
            }
            return type.GetInterfaces().Any(@if => @if.IsGenericType && @if.GetGenericTypeDefinition() == interfaceType);
        }

        /// <summary>
        /// A global empty object array.
        /// </summary>
        [NotNull, ItemNotNull]
        public static readonly object[] EmptyObjects = new object[0];

    }
}
