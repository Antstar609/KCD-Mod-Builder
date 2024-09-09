using System;
using System.Windows;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

// ReSharper disable InconsistentNaming
// ReSharper disable UseVerbatimString

namespace KCDModPacker;

public partial class MainWindow
{
    public bool IsSilent = false;

    private readonly PresetData m_presetData;
    private readonly ModManifestWriter m_modManifestWriter;

    private const string m_moddingEulaFileName = "\\modding_eula.txt";
    private const string m_gameExePath = "\\Bin\\Win64\\KingdomCome.exe";

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();

        m_presetData = new PresetData(this);
        m_modManifestWriter = new ModManifestWriter(this);

        m_presetData.LoadAllPresets();
        m_presetData.LoadLastPreset();
    }

    private void MakeModFolder()
    {
        string modPath = xGamePath.Text + "\\Mods\\" + xModName.Text;

        Directory.CreateDirectory(modPath);
        CopyModdingEula(modPath);

        if (!ZipDirectories(modPath)) return;

        m_modManifestWriter.WriteModManifest();

        CustomMessageBox.Display("The mod folder has been created at " + modPath, IsSilent);

        Application.Current.Shutdown();
    }

    private void CopyModdingEula(string _modPath)
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string moddingEulaPath = Path.GetDirectoryName(exePath) + m_moddingEulaFileName;

        if (File.Exists(moddingEulaPath))
        {
            File.Copy(moddingEulaPath, _modPath + m_moddingEulaFileName);
        }
        else
        {
            CustomMessageBox.Display("The modding_eula.txt file is missing", IsSilent, false);
        }
    }

    private bool ZipDirectories(string _modPath)
    {
        // Copy the data folder and zip it
        string[] directories = Directory.GetDirectories(xRepoPath.Text);
        bool isDataZipped = false;
        bool isLocalizationZipped = false;
        bool isTablesZipped = false;

        foreach (string directoryName in directories)
        {
            if (directoryName.Contains("Data") && !isDataZipped)
            {
                Directory.CreateDirectory(_modPath + "\\Data");
                string dataDataPak = _modPath + "\\Data\\Data.pak";
                ZipFile.CreateFromDirectory(directoryName, dataDataPak, CompressionLevel.Optimal, false);
                isDataZipped = true;
            }

            if (directoryName.Contains("Libs") && !isTablesZipped)
            {
                string DataTablesPak = _modPath + "\\Data\\Tables.pak";
                ZipFile.CreateFromDirectory(directoryName, DataTablesPak, CompressionLevel.Optimal, true);
                isTablesZipped = true;
            }

            if (directoryName.Contains("Localization") && !isLocalizationZipped)
            {
                Directory.CreateDirectory(_modPath + "\\Localization");
                string[] LocalizationDirectories = Directory.GetDirectories(directoryName);

                foreach (string LocalizationDirectory in LocalizationDirectories)
                {
                    string LocalizationPath = _modPath + "\\Localization\\" + Path.GetFileName(LocalizationDirectory) + "_xml.pak";
                    ZipFile.CreateFromDirectory(LocalizationDirectory, LocalizationPath, CompressionLevel.Optimal, false);
                }

                isLocalizationZipped = true;
            }

            if (isDataZipped && isLocalizationZipped && isTablesZipped)
                break;
        }

        return true;
    }

    private void RepoBrowsePath_Button_Click(object _sender, RoutedEventArgs _event)
    {
        CommonOpenFileDialog openFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (openFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        // Don't check here if it's a valid repo but when the mod is created
        xRepoPath.Text = openFileDialog.FileName;
    }

    private void GameBrowsePath_Button_Click(object _sender, RoutedEventArgs _event)
    {
        CommonOpenFileDialog openFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (openFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        // Check if the selected directory is a valid game path
        string? potentialGamePath = openFileDialog.FileName;
        string exePath = potentialGamePath + m_gameExePath;

        if (File.Exists(exePath))
        {
            xGamePath.Text = potentialGamePath;
        }
        else
        {
            // Display an error message to the user
            CustomMessageBox.Display("The selected directory does not appear to be a valid game path. Please select the directory where Kingdom Come: Deliverance is installed.", IsSilent);
        }
    }

    public void Run_Button_Click(object _sender, RoutedEventArgs _event)
    {
        if (!string.IsNullOrEmpty(xModName.Text) && !string.IsNullOrEmpty(xRepoPath.Text) && !string.IsNullOrEmpty(xGamePath.Text) &&
            !string.IsNullOrEmpty(xModVersion.Text) && !string.IsNullOrEmpty(xAuthor.Text))
        {
            // Check if the mod already exists and if I can access it (if it's not in use)
            string modPath = xGamePath.Text + "\\Mods\\" + xModName.Text;

            if (Directory.Exists(modPath))
            {
                try
                {
                    Directory.Delete(modPath, true);
                }
                catch (Exception)
                {
                    CustomMessageBox.Display("Please close the game and try again !", IsSilent);
                    return;
                }
            }

            MakeModFolder();
            m_presetData.StorePresetData();
        }
        else
        {
            if (IsSilent)
            {
                if (m_presetData.LoadLastPreset()) return;
                
                // All fields are not filled
                CustomMessageBox.Display("Please ensure all fields are filled in the application before using silent mode", IsSilent);
                Application.Current.Shutdown();
            }
            else
            {
                var messageBox = new CustomMessageBox("Please fill all the fields");
                messageBox.ShowDialog();
            }
        }
    }
    
    private void Presets_SelectionChanged(object _sender, System.Windows.Controls.SelectionChangedEventArgs _e)
    {
        if (xPresets.SelectedItem == null) return;
        
        string? selectedPreset = xPresets.SelectedItem.ToString();
        m_presetData.LoadPresetData(selectedPreset);
    }

    [GeneratedRegex("[^0-9.]+")]
    private static partial Regex NumberValidation();

    private void NumberValidationTextBox(object _sender, TextCompositionEventArgs _event)
    {
        Regex regex = NumberValidation();
        _event.Handled = regex.IsMatch(_event.Text);
    }

    [GeneratedRegex("[^a-zA-Z0-9_]+")]
    private static partial Regex NonSpecialCharValidation();

    private void NonSpecialCharValidationTextBox(object _sender, TextCompositionEventArgs _event)
    {
        Regex regex = NonSpecialCharValidation();
        _event.Handled = regex.IsMatch(_event.Text);
    }
}