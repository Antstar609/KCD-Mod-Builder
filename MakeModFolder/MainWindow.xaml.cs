using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;

// ReSharper disable UseVerbatimString

namespace MakeModFolder;

public partial class MainWindow : INotifyPropertyChanged
{
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        GetJsonData();
    }

    private string JsonPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\MakeModFolder.saved.json";

    private void SetJsonData()
    {
        var data = new Dictionary<string, string>
        {
            { "ModName", ModName },
            { "GamePath", GamePath },
            { "RepoPath", RepoPath },
            { "ModVersion", ModVersion },
            { "IsMapModified", IsMapModified }
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(JsonPath, json);
    }

    private void GetJsonData()
    {
        if (File.Exists(JsonPath))
        {
            var json = File.ReadAllText(JsonPath);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            ModName = data["ModName"];
            GamePath = data["GamePath"];
            RepoPath = data["RepoPath"];
            ModVersion = data["ModVersion"];
            IsMapModified = data["IsMapModified"];
        }
    }

    private void MakeModFolder()
    {
        var modPath = GamePath + "\\Mods\\" + ModName;

        // Create the mod folder
        Directory.CreateDirectory(modPath);

        // Create the main folders
        Directory.CreateDirectory(modPath + "\\Data");
        Directory.CreateDirectory(modPath + "\\Localization");

        // Copy the modding_eula.txt
        var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var moddingEulaPath = Path.GetDirectoryName(exePath) + "\\modding_eula.txt";
        if (File.Exists(moddingEulaPath))
        {
            File.Copy(moddingEulaPath, modPath + "\\modding_eula.txt");
        }
        else
        {
            MessageBox.Show("The modding_eula.txt file is missing", "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        // Copy the data folder and zip it
        var directories = Directory.GetDirectories(RepoPath);
        var isDatazipped = false;
        var isLocalizationzipped = false;
        var isTablesZipped = false;
        foreach (var directory in directories)
        {
            if (directory.Contains("Data") && !isDatazipped)
            {
                var dataPath = modPath + "\\Data\\Data.pak";
                ZipFile.CreateFromDirectory(directory, dataPath, CompressionLevel.Optimal,
                    false);
                isDatazipped = true;
            }

            if (directory.Contains("Libs") && !isTablesZipped)
            {
                var tablesPath = modPath + "\\Data\\Tables.pak";
                ZipFile.CreateFromDirectory(directory, tablesPath, CompressionLevel.Optimal,
                    true);
                isTablesZipped = true;
            }

            if (directory.Contains("Localization") && !isLocalizationzipped)
            {
                var localizationDirectories = Directory.GetDirectories(directory);

                if (localizationDirectories.Length == 0)
                {
                    MessageBox.Show("The localization folder is empty", "Warning", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                foreach (var localizationDirectory in localizationDirectories)
                {
                    var localizationPath = modPath + "\\Localization\\" + Path.GetFileName(localizationDirectory) + "_xml.pak";
                    ZipFile.CreateFromDirectory(localizationDirectory, localizationPath,
                        CompressionLevel.Optimal, false);
                }

                isLocalizationzipped = true;
            }

            if (isDatazipped && isLocalizationzipped && isTablesZipped)
                break;
        }

        if (!isDatazipped)
        {
            MessageBox.Show("The data folder is missing", "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!isTablesZipped)
        {
            MessageBox.Show("The tables folder is missing", "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!isLocalizationzipped)
        {
            MessageBox.Show("The localization folder is missing", "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        //Create the mod.manifest file
        WriteModManifest();

        // MessageBox the user that the mod folder has been created and the location of it
        MessageBox.Show("The mod folder has been created at " + modPath, "Success", MessageBoxButton.OK,
            MessageBoxImage.Information);
        Application.Current.Shutdown();
    }

    private void WriteModManifest()
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };

        using XmlWriter writer = XmlWriter.Create(GamePath + "\\Mods\\" + ModName + "\\mod.manifest", settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("kcd_mod"); // kcd_mod
        writer.WriteStartElement("info"); // info
        writer.WriteStartElement("name"); // name
        writer.WriteValue(ModName);
        writer.WriteEndElement(); // /name
        writer.WriteStartElement("modid"); // modid
        writer.WriteValue(ModName.Replace(" ", "").ToLower());
        writer.WriteEndElement(); // /modid
        writer.WriteStartElement("description"); // description
        writer.WriteValue("A mod for Kingdom Come: Deliverance");
        writer.WriteEndElement(); // /description
        writer.WriteStartElement("author"); // author
        writer.WriteValue("Antstar609");
        writer.WriteEndElement(); // /author
        writer.WriteStartElement("version"); // version
        writer.WriteValue(ModVersion);
        writer.WriteEndElement(); // /version
        writer.WriteStartElement("created_on"); // created_on
        writer.WriteValue(DateTime.Now.ToString("dd.MM.yyyy"));
        writer.WriteEndElement(); // /created_on
        writer.WriteStartElement("modifies_level"); // modifies_level
        writer.WriteValue(IsMapModified.ToLower());
        writer.WriteEndElement(); // /modifies_level
        writer.WriteEndElement(); // /info
        writer.WriteEndElement(); // /kcd_mod
        writer.WriteEndDocument();
    }

    private void RepoBrowsePath_Button_Click(object _sender, RoutedEventArgs _e)
    {
        CommonOpenFileDialog openFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            // if in the folder there is a mod.manifest file and a modding_eula.txt file, then it's the right folder
            var files = Directory.GetFiles(openFileDialog.FileName);
            var isRepository = false;
            if (files.Any(file => file.Contains("ModRepository.txt")))
            {
                isRepository = true;
                RepoPath = openFileDialog.FileName;
            }

            if (!isRepository)
            {
                MessageBox.Show("The selected folder is not a valid repository", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    private void GameBrowsePath_Button_Click(object _sender, RoutedEventArgs _e)
    {
        CommonOpenFileDialog openFileDialog = new()
        {
            InitialDirectory = "c:\\",
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            // if in the folder there is a mod.manifest file and a modding_eula.txt file, then it's the right folder
            var files = Directory.GetFiles(openFileDialog.FileName);
            var isGame = false;
            if (files.Any(file => file.Contains("kcd.log")))
            {
                isGame = true;
                GamePath = openFileDialog.FileName;
                SetJsonData();
            }

            if (!isGame)
            {
                MessageBox.Show("The selected folder is not a valid game folder", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    private void Run_Button_Click(object _sender, RoutedEventArgs _e)
    {
        if (!string.IsNullOrEmpty(ModName) && !string.IsNullOrEmpty(RepoPath) &&
            !string.IsNullOrEmpty(GamePath) && !string.IsNullOrEmpty(ModVersion))
        {
            //check if the mod already exists and if i can access it (if it's not in use)
            var modPath = GamePath + "\\Mods\\" + ModName;
            if (Directory.Exists(modPath))
            {
                try
                {
                    Directory.Delete(modPath, true);
                }
                catch (Exception)
                {
                    MessageBox.Show("The mod folder is in use", "Warning", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            MakeModFolder();
            SetJsonData();
        }
        else
        {
            MessageBox.Show("Please fill all the fields", "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private string _modName = "";

    public string ModName
    {
        get => _modName;

        set
        {
            if (_modName != value)
            {
                _modName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _repoPath = "";

    public string RepoPath
    {
        get => _repoPath;

        set
        {
            if (_repoPath != value)
            {
                _repoPath = value;
                OnPropertyChanged();
            }
        }
    }

    private string _gamePath = "";

    public string GamePath
    {
        get => _gamePath;

        set
        {
            if (_gamePath != value)
            {
                _gamePath = value;
                OnPropertyChanged();
            }
        }
    }

    private string _modVersion = "0.1";

    public string ModVersion
    {
        get => _modVersion;
        set
        {
            if (_modVersion != value)
            {
                _modVersion = value;
                OnPropertyChanged();
            }
        }
    }

    private string _isMapModified = "False";

    public string IsMapModified
    {
        get => _isMapModified;

        set
        {
            if (_isMapModified != value)
            {
                _isMapModified = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void NonSpecialCharValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^a-zA-Z0-9_]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}