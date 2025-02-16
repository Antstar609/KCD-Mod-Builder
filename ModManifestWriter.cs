using System;
using System.IO;
using System.Xml;

namespace KCDModBuilder;

public class ModManifestWriter(MainWindow _mainWindow)
{
	public void WriteModManifest()
	{
		XmlWriterSettings settings = new()
		{
			Indent = true,
			IndentChars = "\t",
			NewLineOnAttributes = true
		};

		using var writer = XmlWriter.Create(Path.Combine(_mainWindow.xGamePath.Text, "Mods", _mainWindow.xModName.Text, "mod.manifest"), settings);

		writer.WriteStartDocument();
		writer.WriteStartElement("kcd_mod"); // kcd_mod
		writer.WriteStartElement("info"); // info
		writer.WriteStartElement("name"); // name
		writer.WriteValue(_mainWindow.xModName.Text);
		writer.WriteEndElement(); // /name
		writer.WriteStartElement("modid"); // modid
		writer.WriteValue(_mainWindow.xModName.Text.Replace(" ", "_").ToLower());
		writer.WriteEndElement(); // /modid
		writer.WriteStartElement("description"); // description
		writer.WriteValue("Packed with KCD Mod Builder");
		writer.WriteEndElement(); // /description
		writer.WriteStartElement("author"); // author
		writer.WriteValue(_mainWindow.xAuthor.Text);
		writer.WriteEndElement(); // /author
		writer.WriteStartElement("version"); // version
		writer.WriteValue(_mainWindow.xModVersion.Text);
		writer.WriteEndElement(); // /version
		writer.WriteStartElement("created_on"); // created_on
		writer.WriteValue(DateTime.Now.ToString("dd.MM.yyyy"));
		writer.WriteEndElement(); // /created_on
		writer.WriteStartElement("modifies_level"); // modifies_level
		writer.WriteValue(_mainWindow.xIsMapModified.IsChecked.ToString()?.ToLower());
		writer.WriteEndElement(); // /modifies_level
		writer.WriteEndElement(); // /info
		writer.WriteEndElement(); // /kcd_mod
		writer.WriteEndDocument();
	}
}