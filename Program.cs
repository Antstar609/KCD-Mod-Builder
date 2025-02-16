using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;

namespace KCDModBuilder;

public static class Program
{
	[DllImport("kernel32.dll")]
	private extern static bool FreeConsole();
	
	public async static Task<int> Main(string[] _args)
	{
		bool silentMode = _args.Any(_arg => _arg.Equals("-silent", StringComparison.OrdinalIgnoreCase));
		if (silentMode)
		{
			await RunSilentMode(_args);
			return 0;
		}
		
		FreeConsole();
		return BuildAvaloniaApp().StartWithClassicDesktopLifetime(_args);
	}
	
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();

	private async static Task RunSilentMode(string[] _args)
	{
		BuildAvaloniaApp().SetupWithoutStarting();
		
		var window = new MainWindow
		{
			IsSilent = true
		};
		var presetData = new PresetData(window);
		
		int silentIndex = Array.FindIndex(_args, _arg => _arg.Equals("-silent", StringComparison.OrdinalIgnoreCase));
		string presetName = string.Join(" ", _args.Skip(silentIndex + 1));

		if (!string.IsNullOrWhiteSpace(presetName))
		{
			if (presetData.PresetExists(presetName))
			{
				presetData.LoadPresetData(presetName);
			}
			else
			{
				Console.WriteLine("Preset '" + presetName + "' does not exist.");
				Environment.Exit(1);
				return;
			}
		}
		else
		{
			Console.WriteLine("No preset was entered. Using the last preset used.");
		}
		
		await window.RunAsync();
		Environment.Exit(0);
	}
}