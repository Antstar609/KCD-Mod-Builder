using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KCDModPacker;

public class PresetData(MainWindow _mainWindow)
{
    private readonly JsonSerializerOptions m_options = new() { WriteIndented = true };
    private static readonly string m_presetsDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    private readonly string m_lastPresetPath = m_presetsDirectory + "\\lastPreset.json";
    
    public void LoadAllPresets()
    {
        string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
        foreach (var file in jsonFiles)
        {
            string json = File.ReadAllText(file);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data != null && data.TryGetValue("ModName", out string? value) && !_mainWindow.xPresets.Items.Contains(value))
            {
                _mainWindow.xPresets.Items.Add(value);
            }
        }

        if (jsonFiles.Length <= 0)
        {
            _mainWindow.xPresets.Items.Add("No presets created");
            _mainWindow.xPresets.SelectedIndex = 0;
        }
        else
        {
            _mainWindow.xPresets.Items.Remove("No presets created");
        }
    }
    
    public bool PresetExists(string _presetName)
    {
        string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
        foreach (var file in jsonFiles)
        {
            string json = File.ReadAllText(file);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data != null && data.TryGetValue("ModName", out string? value) && value == _presetName)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool LoadLastPreset()
    {
        if (!File.Exists(m_lastPresetPath)) return false;
        
        string json = File.ReadAllText(m_lastPresetPath);
        string? presetName = JsonSerializer.Deserialize<string>(json);
        _mainWindow.xPresets.SelectedItem = presetName;
        LoadPresetData(presetName);
        return true;
    }
    
    public void LoadPresetData(string? _presetName)
    {
        string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
        foreach (var file in jsonFiles)
        {
            string json = File.ReadAllText(file);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data != null && data.TryGetValue("ModName", out string? value) && value == _presetName)
            {
                _mainWindow.xModName.Text = data["ModName"];
                _mainWindow.xGamePath.Text = data["GamePath"];
                _mainWindow.xRepoPath.Text = data["RepoPath"];
                _mainWindow.xModVersion.Text = data["ModVersion"];
                _mainWindow.xIsMapModified.IsChecked = bool.Parse(data["IsMapModified"]);
                _mainWindow.xAuthor.Text = data["Author"];
                break;
            }
        }
    }
    
    public void StorePresetData()
    {
        var data = new Dictionary<string, string>
        {
            { "ModName", _mainWindow.xModName.Text },
            { "GamePath", _mainWindow.xGamePath.Text },
            { "RepoPath", _mainWindow.xRepoPath.Text },
            { "ModVersion", _mainWindow.xModVersion.Text },
            { "IsMapModified", _mainWindow.xIsMapModified.IsChecked.ToString() },
            { "Author", _mainWindow.xAuthor.Text }
        };
    
        string json = JsonSerializer.Serialize(data, m_options);
        string path = m_presetsDirectory + "\\" + _mainWindow.xModName.Text + ".preset.json";
        File.WriteAllText(path, json);
        
        json = JsonSerializer.Serialize(_mainWindow.xModName.Text, m_options);
        File.WriteAllText(m_lastPresetPath, json);
    }
}