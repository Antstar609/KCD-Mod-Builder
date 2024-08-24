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
            { "ModName", _MainWindow.ModName.Text },
            { "GamePath", _MainWindow.GamePath.Text },
            { "RepoPath", _MainWindow.RepoPath.Text },
            { "ModVersion", _MainWindow.ModVersion.Text },
            { "IsMapModified", _MainWindow.IsMapModified.IsChecked.ToString() },
            { "Author", _MainWindow.Author.Text }
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

        _MainWindow.ModName.Text = Data["ModName"];
        _MainWindow.GamePath.Text = Data["GamePath"];
        _MainWindow.RepoPath.Text = Data["RepoPath"];
        _MainWindow.ModVersion.Text = Data["ModVersion"];
        _MainWindow.IsMapModified.IsChecked = bool.Parse(Data["IsMapModified"]);
        _MainWindow.Author.Text = Data["Author"];
    }
}