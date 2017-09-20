using System.Reflection;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Theater {
    public static class ApplicationHelper {

        public static string GetTitle() {
            if (_title != null) {
                return _title;
            }

            var assembly = Assembly.GetAssembly(typeof(ApplicationHelper));
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            if (titleAttr == null) {
                return string.Empty;
            }

            _title = titleAttr.Title;
            return _title;
        }

        public static string GetCodeName() {
            if (_codeName != null) {
                return _codeName;
            }

            var assembly = Assembly.GetAssembly(typeof(ApplicationHelper));
            var codeNameAttr = assembly.GetCustomAttribute<MilliSimCodeNameAttribute>();
            if (codeNameAttr == null) {
                return MilliSimCodeNameAttribute.DefaultCodeName;
            }

            _codeName = codeNameAttr.CodeName;
            return _codeName;
        }

        private static string _title;
        private static string _codeName;

    }
}
