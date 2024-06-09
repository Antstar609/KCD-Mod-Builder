using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MakeModFolder;

public class UserData(MainWindow _MainWindow)
{
    public void SetUserData()
    {
        var Data = new Dictionary<string, string>
        {
            { "ModName", _MainWindow.ModName },
            { "GamePath", _MainWindow.GamePath },
            { "RepoPath", _MainWindow.RepoPath },
            { "ModVersion", _MainWindow.ModVersion },
            { "IsMapModified", _MainWindow.IsMapModified },
            { "Author", _MainWindow.Author }
        };

        string Json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_MainWindow.JsonPath, Json);
    }

    public void GetUserData()
    {
        if (!File.Exists(_MainWindow.JsonPath)) return;

        string Json = File.ReadAllText(_MainWindow.JsonPath);
        var Data = JsonSerializer.Deserialize<Dictionary<string, string>>(Json);

        if (Data == null) return;

        _MainWindow.ModName = Data["ModName"];
        _MainWindow.GamePath = Data["GamePath"];
        _MainWindow.RepoPath = Data["RepoPath"];
        _MainWindow.ModVersion = Data["ModVersion"];
        _MainWindow.IsMapModified = Data["IsMapModified"];
        _MainWindow.Author = Data["Author"];
    }
}