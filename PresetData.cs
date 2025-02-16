using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace KCDModBuilder;

public class PresetData()
{
	private readonly MainWindow m_mainWindow;
	private readonly static string m_presetsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KCD Mod Builder");
	private readonly string m_lastPresetPath;

	public PresetData(MainWindow _mainWindow) : this()
	{
		m_mainWindow = _mainWindow;
		m_lastPresetPath = Path.Combine(m_presetsDirectory, "lastPreset.json");
		Directory.CreateDirectory(m_presetsDirectory);
	}

	public void LoadAllPresets()
	{
		string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
		foreach (var file in jsonFiles)
		{
			string json = File.ReadAllText(file);
			var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			if (data != null && data.TryGetValue("ModName", out string? value) && !m_mainWindow.xPresets.Items.Contains(value))
			{
				m_mainWindow.xPresets.Items.Add(value);
			}
		}

		if (jsonFiles.Length <= 0)
		{
			m_mainWindow.xPresets.Items.Add("No presets created");
			m_mainWindow.xPresets.SelectedIndex = 0;
		}
		else
		{
			m_mainWindow.xPresets.Items.Remove("No presets created");
		}
	}

	public bool PresetExists(string _presetName)
	{
		string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
		foreach (var file in jsonFiles)
		{
			string json = File.ReadAllText(file);
			var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			if (data != null && data.TryGetValue("ModName", out string? value) && value == _presetName)
			{
				return true;
			}
		}

		return false;
	}

	public void LoadLastPreset()
	{
		if (!File.Exists(m_lastPresetPath)) return;

		string json = File.ReadAllText(m_lastPresetPath);
		string? presetName = JsonConvert.DeserializeObject<string>(json);
		m_mainWindow.xPresets.SelectedItem = presetName;
		LoadPresetData(presetName);
	}

	public void LoadPresetData(string? _presetName)
	{
		string[] jsonFiles = Directory.GetFiles(m_presetsDirectory, "*.preset.json");
		foreach (var file in jsonFiles)
		{
			string json = File.ReadAllText(file);
			var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			if (data != null && data.TryGetValue("ModName", out string? value) && value == _presetName)
			{
				m_mainWindow.xModName.Text = data["ModName"];
				m_mainWindow.xGamePath.Text = data["GamePath"];
				m_mainWindow.xProjectPath.Text = data["ProjectPath"];
				m_mainWindow.xModVersion.Text = data["ModVersion"];
				m_mainWindow.xIsMapModified.IsChecked = bool.Parse(data["IsMapModified"]);
				m_mainWindow.xAuthor.Text = data["Author"];
				break;
			}
		}
	}

	public void StorePresetData()
	{
		var data = new Dictionary<string, string>
		{
			{
				"ModName", m_mainWindow.xModName.Text
			},
			{
				"GamePath", m_mainWindow.xGamePath.Text
			},
			{
				"ProjectPath", m_mainWindow.xProjectPath.Text
			},
			{
				"ModVersion", m_mainWindow.xModVersion.Text
			},
			{
				"IsMapModified", m_mainWindow.xIsMapModified.IsChecked.ToString()
			},
			{
				"Author", m_mainWindow.xAuthor.Text
			}
		};

		string json = JsonConvert.SerializeObject(data, Formatting.Indented);
		string path = Path.Combine(m_presetsDirectory, m_mainWindow.xModName.Text + ".preset.json");
		File.WriteAllText(path, json);

		json = JsonConvert.SerializeObject(m_mainWindow.xModName.Text, Formatting.Indented);
		File.WriteAllText(m_lastPresetPath, json);
	}

	public void DeletePreset(string? _presetName)
	{
		string presetFilePath = Path.Combine(m_presetsDirectory, _presetName + ".preset.json");

		if (!File.Exists(presetFilePath)) return;

		File.Delete(presetFilePath);
		m_mainWindow.xPresets.Items.Remove(_presetName);

		if (m_mainWindow.xPresets.SelectedItem?.ToString() == _presetName)
		{
			File.Delete(m_lastPresetPath);
		}

		if (m_mainWindow.xPresets.Items.Count == 0)
		{
			m_mainWindow.xModName.Text = string.Empty;
			m_mainWindow.xGamePath.Text = string.Empty;
			m_mainWindow.xProjectPath.Text = string.Empty;
			m_mainWindow.xModVersion.Text = string.Empty;
			m_mainWindow.xIsMapModified.IsChecked = false;
			m_mainWindow.xAuthor.Text = string.Empty;
			
			m_mainWindow.xPresets.Items.Add("No presets created");
			m_mainWindow.xPresets.SelectedIndex = 0;
		}
		else
		{
			var test = m_mainWindow.xPresets.Items[0]?.ToString();
			var json = JsonConvert.SerializeObject(test, Formatting.Indented);
			File.WriteAllText(m_lastPresetPath, json);
			LoadLastPreset();
		}
	}
}