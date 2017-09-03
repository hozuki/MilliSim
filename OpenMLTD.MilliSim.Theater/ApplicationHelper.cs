using System.Reflection;

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

        private static string _title;

    }
}
