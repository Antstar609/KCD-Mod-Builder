using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace MakeModFolder
{
    public partial class App
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Contains("--silent"))
            {
                AllocConsole();
                var mainWindow = new MainWindow
                {
                    isSilent = true
                };
                mainWindow.Run_Button_Click(null, null);
            }
            else
            {
                FreeConsole();
            }
        }
    }
}