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
    public readonly string JsonPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\KCDModPacker.saved.json";
    public bool IsSilent = false;

    private readonly UserData m_UserData;
    private readonly ModManifestWriter m_ModManifestWriter;

    private const string ModdingEulaFileName = "\\modding_eula.txt";
    private const string GameExePath = "\\Bin\\Win64\\KingdomCome.exe";

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();

        m_UserData = new UserData(this);
        m_ModManifestWriter = new ModManifestWriter(this);

        m_UserData.GetUserData();
    }

    private void MakeModFolder()
    {
        string ModPath = GamePath.Text + "\\Mods\\" + ModName.Text;

        Directory.CreateDirectory(ModPath);
        CopyModdingEula(ModPath);

        if (!ZipDirectories(ModPath)) return;

        m_ModManifestWriter.WriteModManifest();

        CustomMessageBox.Display("The mod folder has been created at " + ModPath, IsSilent);

        Application.Current.Shutdown();
    }

    private void CopyModdingEula(string _ModPath)
    {
        string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string ModdingEulaPath = Path.GetDirectoryName(ExePath) + ModdingEulaFileName;

        if (File.Exists(ModdingEulaPath))
        {
            File.Copy(ModdingEulaPath, _ModPath + ModdingEulaFileName);
        }
        else
        {
            CustomMessageBox.Display("The modding_eula.txt file is missing", IsSilent, false);
        }
    }

    private bool ZipDirectories(string _ModPath)
    {
        // Copy the data folder and zip it
        string[] Directories = Directory.GetDirectories(RepoPath.Text);
        bool IsDataZipped = false;
        bool IsLocalizationZipped = false;
        bool IsTablesZipped = false;

        foreach (string DirectoryName in Directories)
        {
            if (DirectoryName.Contains("Data") && !IsDataZipped)
            {
                Directory.CreateDirectory(_ModPath + "\\Data");
                string DataDataPak = _ModPath + "\\Data\\Data.pak";
                ZipFile.CreateFromDirectory(DirectoryName, DataDataPak, CompressionLevel.Optimal, false);
                IsDataZipped = true;
            }

            if (DirectoryName.Contains("Libs") && !IsTablesZipped)
            {
                string DataTablesPak = _ModPath + "\\Data\\Tables.pak";
                ZipFile.CreateFromDirectory(DirectoryName, DataTablesPak, CompressionLevel.Optimal, true);
                IsTablesZipped = true;
            }

            if (DirectoryName.Contains("Localization") && !IsLocalizationZipped)
            {
                Directory.CreateDirectory(_ModPath + "\\Localization");
                string[] LocalizationDirectories = Directory.GetDirectories(DirectoryName);

                foreach (string LocalizationDirectory in LocalizationDirectories)
                {
                    string LocalizationPath = _ModPath + "\\Localization\\" + Path.GetFileName(LocalizationDirectory) + "_xml.pak";
                    ZipFile.CreateFromDirectory(LocalizationDirectory, LocalizationPath, CompressionLevel.Optimal, false);
                }

                IsLocalizationZipped = true;
            }

            if (IsDataZipped && IsLocalizationZipped && IsTablesZipped)
                break;
        }

        return true;
    }

    private void RepoBrowsePath_Button_Click(object _Sender, RoutedEventArgs _Event)
    {
        CommonOpenFileDialog OpenFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (OpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        // Don't check here if it's a valid repo but when the mod is created
        RepoPath.Text = OpenFileDialog.FileName;
    }

    private void GameBrowsePath_Button_Click(object _Sender, RoutedEventArgs _Event)
    {
        CommonOpenFileDialog OpenFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (OpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        // Check if the selected directory is a valid game path
        string? PotentialGamePath = OpenFileDialog.FileName;
        string ExePath = PotentialGamePath + GameExePath;

        if (File.Exists(ExePath))
        {
            GamePath.Text = PotentialGamePath;
        }
        else
        {
            // Display an error message to the user
            CustomMessageBox.Display("The selected directory does not appear to be a valid game path. Please select the directory where Kingdom Come: Deliverance is installed.", IsSilent);
        }
    }

    public void Run_Button_Click(object _Sender, RoutedEventArgs _Event)
    {
        if (!string.IsNullOrEmpty(ModName.Text) && !string.IsNullOrEmpty(RepoPath.Text) && !string.IsNullOrEmpty(GamePath.Text) &&
            !string.IsNullOrEmpty(ModVersion.Text) && !string.IsNullOrEmpty(Author.Text))
        {
            // Check if the mod already exists and if I can access it (if it's not in use)
            string ModPath = GamePath.Text + "\\Mods\\" + ModName.Text;

            if (Directory.Exists(ModPath))
            {
                try
                {
                    Directory.Delete(ModPath, true);
                }
                catch (Exception)
                {
                    CustomMessageBox.Display("Please close the game and try again !", IsSilent);
                    return;
                }
            }

            MakeModFolder();
            m_UserData.SetUserData();
        }
        else
        {
            if (IsSilent)
            {
                // All fields are not filled
                Console.WriteLine("Please ensure all fields are filled in the application before using silent mode");
                Application.Current.Shutdown();
            }
            else
            {
                var MessageBox = new CustomMessageBox("Please fill all the fields");
                MessageBox.ShowDialog();
            }
        }
    }

    [GeneratedRegex("[^0-9.]+")]
    private static partial Regex NumberValidation();

    private void NumberValidationTextBox(object _Sender, TextCompositionEventArgs _Event)
    {
        var Regex = NumberValidation();
        _Event.Handled = Regex.IsMatch(_Event.Text);
    }

    [GeneratedRegex("[^a-zA-Z0-9_]+")]
    private static partial Regex NonSpecialCharValidation();

    private void NonSpecialCharValidationTextBox(object _Sender, TextCompositionEventArgs _Event)
    {
        var Regex = NonSpecialCharValidation();
        _Event.Handled = Regex.IsMatch(_Event.Text);
    }
}