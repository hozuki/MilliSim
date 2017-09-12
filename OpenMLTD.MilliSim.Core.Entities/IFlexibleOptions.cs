using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    public interface IFlexibleOptions : IReadOnlyDictionary<string, object> {

        object GetValue([NotNull] string key);

        void SetValue([NotNull] string key, [CanBeNull] object value);

        T GetValue<T>([NotNull] string key);

        void SetValue<T>([NotNull] string key, [CanBeNull] T value);

    }
}
