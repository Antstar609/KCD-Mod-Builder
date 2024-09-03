using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KCDModPacker;

public class UserData(MainWindow _mainWindow)
{
    private readonly JsonSerializerOptions m_options = new() { WriteIndented = true };

    public void SetUserData()
    {
        var Data = new Dictionary<string, string>
        {
            { "ModName", _mainWindow.ModName.Text },
            { "GamePath", _mainWindow.GamePath.Text },
            { "RepoPath", _mainWindow.RepoPath.Text },
            { "ModVersion", _mainWindow.ModVersion.Text },
            { "IsMapModified", _mainWindow.IsMapModified.IsChecked.ToString() },
            { "Author", _mainWindow.Author.Text }
        };

        string Json = JsonSerializer.Serialize(Data, m_options);
        File.WriteAllText(_mainWindow.JsonPath, Json);
    }

    public void GetUserData()
    {
        if (!File.Exists(_mainWindow.JsonPath)) return;

        string Json = File.ReadAllText(_mainWindow.JsonPath);
        var Data = JsonSerializer.Deserialize<Dictionary<string, string>>(Json);

        if (Data == null) return;

        _mainWindow.ModName.Text = Data["ModName"];
        _mainWindow.GamePath.Text = Data["GamePath"];
        _mainWindow.RepoPath.Text = Data["RepoPath"];
        _mainWindow.ModVersion.Text = Data["ModVersion"];
        _mainWindow.IsMapModified.IsChecked = bool.Parse(Data["IsMapModified"]);
        _mainWindow.Author.Text = Data["Author"];
    }
}