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

// ReSharper disable InconsistentNaming
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

    public bool isSilent = false;
    private readonly string JsonPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\MakeModFolder.saved.json";

    private void SetJsonData()
    {
        var data = new Dictionary<string, string>
        {
            { "ModName", ModName },
            { "GamePath", GamePath },
            { "RepoPath", RepoPath },
            { "ModVersion", ModVersion },
            { "IsMapModified", IsMapModified },
            { "Author", Author }
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(JsonPath, json);
    }

    private void GetJsonData()
    {
        if (!File.Exists(JsonPath)) return;

        var json = File.ReadAllText(JsonPath);
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        if (data == null) return;

        ModName = data["ModName"];
        GamePath = data["GamePath"];
        RepoPath = data["RepoPath"];
        ModVersion = data["ModVersion"];
        IsMapModified = data["IsMapModified"];
        Author = data["Author"];
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
            Display("The modding_eula.txt file is missing", MessageBoxButton.OK, MessageBoxImage.Warning, false);
        }

        // Copy the data folder and zip it
        var directories = Directory.GetDirectories(RepoPath);
        var isDataZipped = false;
        var isLocalizationZipped = false;
        var isTablesZipped = false;
        foreach (var directory in directories)
        {
            if (directory.Contains("Data") && !isDataZipped)
            {
                var dataPath = modPath + "\\Data\\Data.pak";
                ZipFile.CreateFromDirectory(directory, dataPath, CompressionLevel.Optimal, false);
                isDataZipped = true;
            }

            if (directory.Contains("Libs") && !isTablesZipped)
            {
                var tablesPath = modPath + "\\Data\\Tables.pak";
                ZipFile.CreateFromDirectory(directory, tablesPath, CompressionLevel.Optimal, true);
                isTablesZipped = true;
            }

            if (directory.Contains("Localization") && !isLocalizationZipped)
            {
                var localizationDirectories = Directory.GetDirectories(directory);

                if (localizationDirectories.Length == 0)
                {
                    Display("The localization folder is empty", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var localizationDirectory in localizationDirectories)
                {
                    var localizationPath = modPath + "\\Localization\\" + Path.GetFileName(localizationDirectory) + "_xml.pak";
                    ZipFile.CreateFromDirectory(localizationDirectory, localizationPath, CompressionLevel.Optimal, false);
                }

                isLocalizationZipped = true;
            }

            if (isDataZipped && isLocalizationZipped && isTablesZipped)
                break;
        }

        if (!isDataZipped)
        {
            Display("The data folder is missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!isTablesZipped)
        {
            Display("The tables folder is missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!isLocalizationZipped)
        {
            Display("The localization folder is missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        //Create the mod.manifest file
        WriteModManifest();

        // MessageBox the user that the mod folder has been created and the location of it
        Display("The mod folder has been created at " + modPath, MessageBoxButton.OK, MessageBoxImage.Information, false, "Success");

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

        using var writer = XmlWriter.Create(GamePath + "\\Mods\\" + ModName + "\\mod.manifest", settings);

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
        writer.WriteValue(Author);
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

        if (openFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

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
            MessageBox.Show("The selected folder is not a valid repository", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        if (openFileDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

        // if in the folder there is a mod.manifest file and a modding_eula.txt file, then it's the right folder
        var files = Directory.GetFiles(openFileDialog.FileName);
        var isGame = false;
        if (files.Any(file => file.Contains("kcd.log")))
        {
            isGame = true;
            GamePath = openFileDialog.FileName;
        }

        if (!isGame)
        {
            MessageBox.Show("The selected folder is not a valid game folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public void Run_Button_Click(object _sender, RoutedEventArgs _e)
    {
        if (!string.IsNullOrEmpty(ModName) && !string.IsNullOrEmpty(RepoPath) && !string.IsNullOrEmpty(GamePath) &&
            !string.IsNullOrEmpty(ModVersion) && !string.IsNullOrEmpty(Author))
        {
            //check if the mod already exists and if I can access it (if it's not in use)
            var modPath = GamePath + "\\Mods\\" + ModName;
            if (Directory.Exists(modPath))
            {
                try
                {
                    Directory.Delete(modPath, true);
                }
                catch (Exception)
                {
                    Display("Please close the game and try again !", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            MakeModFolder();
            SetJsonData();
        }
        else
        {
            if (isSilent)
            {
                Console.WriteLine("Please ensure all fields are filled in the application before using silent mode");
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Please fill all the fields", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

    private string _modVersion = "";

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

    private string IsMapModified
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

    private string _author = "";

    public string Author
    {
        get => _author;
        set
        {
            if (_author != value)
            {
                _author = value;
                OnPropertyChanged();
            }
        }
    }

    private void Display(string _message, MessageBoxButton _button, MessageBoxImage _image, bool _shutdown = true, string _caption = "Warning")
    {
        if (isSilent)
        {
            Console.WriteLine(_message);
            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
        }
        else
        {
            MessageBox.Show(_message, _caption, _button, _image);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [GeneratedRegex("[^0-9.]+")]
    private static partial Regex NumberValidation();

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = NumberValidation();
        e.Handled = regex.IsMatch(e.Text);
    }

    [GeneratedRegex("[^a-zA-Z0-9_]+")]
    private static partial Regex NonSpecialCharValidation();

    private void NonSpecialCharValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = NonSpecialCharValidation();
        e.Handled = regex.IsMatch(e.Text);
    }
}