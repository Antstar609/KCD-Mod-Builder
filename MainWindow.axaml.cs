using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;

namespace KCDModBuilder;

public partial class MainWindow : Window
{
	public bool IsSilent = false;

	private readonly PresetData m_presetData;
	private readonly ModManifestWriter m_modManifestWriter;

	private const string m_GameExePath1 = @"\Bin\Win64\KingdomCome.exe";
	private const string m_GameExePath2 = @"\Bin\Win64MasterMasterSteamPGO\KingdomCome.exe";

	public MainWindow()
	{
		InitializeComponent();

		m_presetData = new PresetData(this);
		m_modManifestWriter = new ModManifestWriter(this);

		CheckCanRun(null, null);
		m_presetData.LoadAllPresets();
		m_presetData.LoadLastPreset();
	}

	private async Task MakeModFolderAsync()
	{
		string modPath = Path.Combine(xGamePath.Text, "Mods", xModName.Text);

		Directory.CreateDirectory(modPath);
		CopyModdingEula(modPath);
		if (!ZipDirectories(modPath)) return;

		m_modManifestWriter.WriteModManifest();
		CompressArchive(modPath);

		if (!IsSilent)
		{
			m_presetData.StorePresetData();
			m_presetData.LoadAllPresets();
			xPresets.SelectedItem = xModName.Text;
			xPresets.IsEnabled = true;
		}

		await DisplayMessageAsync("The mod folder has been created at" + modPath);
	}

	private void CopyModdingEula(string _modPath)
	{
		string targetFilePath = Path.Combine(_modPath, "modding_eula.txt");
		if (File.Exists(targetFilePath)) return;

		using Stream resourceStream = AssetLoader.Open(new Uri("avares://KCDModBuilder/Assets/modding_eula.txt"));
		using FileStream fileStream = new FileStream(targetFilePath, FileMode.CreateNew, FileAccess.Write);

		resourceStream.CopyTo(fileStream);
	}

	private bool ZipDirectories(string _modPath)
	{
		// Copy the data folder and zip it
		string[] directories = Directory.GetDirectories(xProjectPath.Text);
		bool isDataZipped = false;
		bool isLocalizationZipped = false;
		bool isTablesZipped = false;

		foreach (string directoryName in directories)
		{
			if (directoryName.Contains("Data") && !isDataZipped)
			{
				Directory.CreateDirectory(Path.Combine(_modPath, "Data"));
				string dataDataPak = Path.Combine(_modPath, "Data", "Data.pak");
				ZipFile.CreateFromDirectory(directoryName, dataDataPak, CompressionLevel.Optimal, false);
				isDataZipped = true;
			}

			if (directoryName.Contains("Libs") && !isTablesZipped)
			{
				string dataTablesPak = Path.Combine(_modPath, "Data", "Tables.pak");
				ZipFile.CreateFromDirectory(directoryName, dataTablesPak, CompressionLevel.Optimal, true);
				isTablesZipped = true;
			}

			if (directoryName.Contains("Localization") && !isLocalizationZipped)
			{
				Directory.CreateDirectory(Path.Combine(_modPath, "Localization"));
				string[] localizationDirectories = Directory.GetDirectories(directoryName);

				foreach (string localizationDirectory in localizationDirectories)
				{
					string localizationPath = Path.Combine(_modPath, "Localization", Path.GetFileName(localizationDirectory) + "_xml.pak");
					ZipFile.CreateFromDirectory(localizationDirectory, localizationPath, CompressionLevel.Optimal, false);
				}

				isLocalizationZipped = true;
			}

			if (isDataZipped && isLocalizationZipped && isTablesZipped)
				break;
		}

		return true;
	}

	private void CompressArchive(string _modPath)
	{
		string archivePath = Path.Combine(xProjectPath.Text, "Archives");

		if (!Directory.Exists(archivePath))
		{
			Directory.CreateDirectory(archivePath);
		}

		string formatedName = xModName.Text.Replace(" ", "-");
		string archiveFileName = Path.Combine(archivePath, formatedName + "-v" + xModVersion.Text + ".zip");

		if (File.Exists(archiveFileName))
		{
			File.Delete(archiveFileName);
		}

		ZipFile.CreateFromDirectory(_modPath, archiveFileName, CompressionLevel.Optimal, true);
	}

	private async Task ProjectBrowsePathAsync()
	{
		var topLevel = GetTopLevel(this);
		var options = new FolderPickerOpenOptions
		{
			Title = "Select the Project Folder",
			AllowMultiple = false
		};

		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
		if (folders.Count <= 0) return;

		var selectedFolder = folders[0];
		xProjectPath.Text = selectedFolder.Path.LocalPath.TrimEnd(Path.DirectorySeparatorChar);
	}

	private async void ProjectBrowsePath_Click(object _sender, RoutedEventArgs _event)
	{
		await ProjectBrowsePathAsync();
	}

	private async Task GameBrowsePathAsync()
	{
		var topLevel = GetTopLevel(this);

		var folderUri = new Uri("file:///C:/Program Files (x86)/Steam/steamapps/common/");
		var suggestedFolder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(folderUri);

		var options = new FolderPickerOpenOptions
		{
			Title = "Select the Game Folder",
			AllowMultiple = false,
			SuggestedStartLocation = suggestedFolder
		};

		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
		if (folders.Count <= 0) return;

		var selectedFolder = folders[0];
		string potentialGamePath = selectedFolder.Path.LocalPath.TrimEnd(Path.DirectorySeparatorChar);

		// Check if the selected directory is a valid game path
		string exePath1 = potentialGamePath + m_GameExePath1;
		string exePath2 = potentialGamePath + m_GameExePath2;

		if (File.Exists(exePath1) || File.Exists(exePath2))
		{
			xGamePath.Text = potentialGamePath;
		}
		else
		{
			await DisplayMessageAsync("The selected directory does not appear to be a valid game path. Please select the directory where Kingdom Come: Deliverance is installed.");
		}
	}

	private async void GameBrowsePath_Click(object _sender, RoutedEventArgs _event)
	{
		await GameBrowsePathAsync();
	}

	public async Task RunAsync()
	{
		if (!string.IsNullOrEmpty(xModName.Text) && !string.IsNullOrEmpty(xProjectPath.Text) && !string.IsNullOrEmpty(xGamePath.Text) &&
		    !string.IsNullOrEmpty(xModVersion.Text) && !string.IsNullOrEmpty(xAuthor.Text))
		{
			// Check if the mod already exists and if I can access it (if it's not in use)
			string modPath = Path.Combine(xGamePath.Text, "Mods", xModName.Text);
			if (Directory.Exists(modPath))
			{
				try
				{
					Directory.Delete(modPath, true);
				}
				catch (Exception)
				{
					await DisplayMessageAsync("Please close the game and try again!");
					return;
				}
			}

			await MakeModFolderAsync();
		}
		else
		{
			await DisplayMessageAsync("Please fill all the fields in the app before using the silent mode.");
		}
	}

	private async void Run_Click(object _sender, RoutedEventArgs _event)
	{
		await RunAsync();
	}

	private void CheckCanRun(object _sender, TextChangedEventArgs _e)
	{
		xRunButton.IsEnabled = !string.IsNullOrEmpty(xModName.Text)
		                       && !string.IsNullOrEmpty(xProjectPath.Text)
		                       && !string.IsNullOrEmpty(xGamePath.Text)
		                       && !string.IsNullOrEmpty(xModVersion.Text)
		                       && !string.IsNullOrEmpty(xAuthor.Text);
	}

	private void Presets_SelectionChanged(object _sender, SelectionChangedEventArgs _e)
	{
		if (xPresets.SelectedItem == null) return;

		string? selectedPreset = xPresets.SelectedItem.ToString();
		m_presetData.LoadPresetData(selectedPreset);

		if (xPresets.SelectedItem.ToString() == "No presets created")
		{
			xPresets.IsEnabled = false;
		}
	}

	private async Task DeletePresetAsync()
	{
		if (xPresets.SelectedItem == null || xPresets.SelectedItem.ToString() == "No presets created") return;

		string? selectedPreset = xPresets.SelectedItem.ToString();
		xPresets.Items.Remove(selectedPreset);
		m_presetData.DeletePreset(selectedPreset);
		await DisplayMessageAsync("Preset " + selectedPreset + " has been deleted.");
	}

	private async void DeletePreset_Click(object _sender, RoutedEventArgs _e)
	{
		await DeletePresetAsync();
	}

	private async Task DisplayMessageAsync(string _message)
	{
		if (IsSilent)
		{
			Console.WriteLine(_message);
		}
		else
		{
			await MessageBoxManager.GetMessageBoxStandard("KCD Mod Builder", _message).ShowAsync();
		}
	}
}

public class NonSpecialCharTextBox : TextBox
{
	protected override Type StyleKeyOverride => typeof(TextBox);
	private readonly static Regex SpecialCharRegex = new(@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);

	protected override void OnTextInput(TextInputEventArgs _event)
	{
		if (SpecialCharRegex.IsMatch(_event.Text))
		{
			_event.Handled = true;
			return;
		}

		base.OnTextInput(_event);
	}
}

public partial class NumbersOnlyTextBox : TextBox
{
	protected override Type StyleKeyOverride => typeof(TextBox);

	[GeneratedRegex("[^0-9.]+")]
	private static partial Regex NumberValidation();

	private readonly static Regex NumberRegex = NumberValidation();

	protected override void OnTextInput(TextInputEventArgs _event)
	{
		if (NumberRegex.IsMatch(_event.Text))
		{
			_event.Handled = true;
			return;
		}

		base.OnTextInput(_event);
	}
}