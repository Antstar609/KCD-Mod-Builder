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

        private void App_OnStartup(object _sender, StartupEventArgs _event)
        {
            AllocConsole();

            if (_event.Args.Contains("-silent"))
            {
                var window = new MainWindow
                {
                    IsSilent = true
                };

                PresetData presetData = new(window);
                string presetName = string.Join(" ", _event.Args.SkipWhile(arg => arg != "-silent").Skip(1));

                if (!string.IsNullOrWhiteSpace(presetName))
                {
                    if (presetData.PresetExists(presetName))
                    {
                        presetData.LoadPresetData(presetName);
                    }
                    else
                    {
                        CustomMessageBox.Display($"Preset '{presetName}' does not exist.", true);
                        Current.Shutdown();
                        return;
                    }
                }
                else
                {
                    CustomMessageBox.Display("No preset was entered. Using the last preset used.", true);
                }

                window.Run_Button_Click(null, null);
            }

            FreeConsole();
        }
    }
}