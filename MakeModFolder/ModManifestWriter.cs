using System;
using System.Xml;

namespace MakeModFolder;

public class ModManifestWriter(MainWindow _MainWindow)
{
    public void WriteModManifest()
    {
        XmlWriterSettings Settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };

        using var Writer = XmlWriter.Create(_MainWindow.GamePath.Text + "\\Mods\\" + _MainWindow.ModName.Text + "\\mod.manifest", Settings);

        Writer.WriteStartDocument();
        Writer.WriteStartElement("kcd_mod"); // kcd_mod
        Writer.WriteStartElement("info"); // info
        Writer.WriteStartElement("name"); // name
        Writer.WriteValue(_MainWindow.ModName.Text);
        Writer.WriteEndElement(); // /name
        Writer.WriteStartElement("modid"); // modid
        Writer.WriteValue(_MainWindow.ModName.Text.Replace(" ", "").ToLower());
        Writer.WriteEndElement(); // /modid
        Writer.WriteStartElement("description"); // description
        Writer.WriteValue("A mod for Kingdom Come: Deliverance");
        Writer.WriteEndElement(); // /description
        Writer.WriteStartElement("author"); // author
        Writer.WriteValue(_MainWindow.Author.Text);
        Writer.WriteEndElement(); // /author
        Writer.WriteStartElement("version"); // version
        Writer.WriteValue(_MainWindow.ModVersion.Text);
        Writer.WriteEndElement(); // /version
        Writer.WriteStartElement("created_on"); // created_on
        Writer.WriteValue(DateTime.Now.ToString("dd.MM.yyyy"));
        Writer.WriteEndElement(); // /created_on
        Writer.WriteStartElement("modifies_level"); // modifies_level
        Writer.WriteValue(_MainWindow.IsMapModified.IsChecked.ToString()?.ToLower());
        Writer.WriteEndElement(); // /modifies_level
        Writer.WriteEndElement(); // /info
        Writer.WriteEndElement(); // /kcd_mod
        Writer.WriteEndDocument();
    }
}