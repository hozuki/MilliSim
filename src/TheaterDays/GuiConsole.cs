namespace OpenMLTD.TheaterDays {
#if ENABLE_GUI_CONSOLE
    internal static class GuiConsole {

        internal static void Initialize() {
            if (_isInitialized) {
                return;
            }

            _isInitialized = true;

            // Follow Windows traditions.
            var consoleEncoding = Encoding.Default;

            try {
                if (!NativeMethods.AttachConsole(NativeConstants.ATTACH_PARENT_PROCESS)) {
                    NativeMethods.AllocConsole();
                }
            } catch (EntryPointNotFoundException) {
            }

            var input = Console.OpenStandardInput();
            var output = Console.OpenStandardOutput();
            var error = Console.OpenStandardError();

            _in = new StreamReader(input, consoleEncoding);
            _out = new StreamWriter(output, consoleEncoding);
            _error = new StreamWriter(error, consoleEncoding);

            _out.WriteLine("hello!");

            _out.AutoFlush = true;
            _error.AutoFlush = true;
        }

        internal static void Uninitialize() {
            if (!_isInitialized) {
                return;
            }

            _in?.Dispose();
            _out?.Dispose();
            _error?.Dispose();

            _in = null;
            _out = null;
            _error = null;

            try {
                NativeMethods.FreeConsole();
            } catch (EntryPointNotFoundException) {
            }

            _isInitialized = false;
        }

        internal static TextReader In => _in;
        internal static TextWriter Error => _error;
        internal static TextWriter Out => _out;

        private static StreamReader _in;
        private static StreamWriter _error;
        private static StreamWriter _out;

        private static bool _isInitialized;

    }
#endif
}
