using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace KCDModPacker
{
    public partial class App
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private void App_OnStartup(object _Sender, StartupEventArgs _Event)
        {
            if (_Event.Args.Contains("--silent"))
            {
                AllocConsole();
                var Window = new MainWindow
                {
                    IsSilent = true
                };
                Window.Run_Button_Click(null, null);
            }
            else
            {
                FreeConsole();
            }
        }
    }
}