# 🎮 KCD Mod Packer

**KCD Mod Packer** is a C# application designed to simplify the process of converting a mod repository into a format that *Kingdom Come: Deliverance* can read. The app packages mod repository folders into `.pak` files and creates a proper folder structure, making it easier for developers to prepare mods for the game and streamline testing.

## ✨ Features

- **📂 Path Selection:** Users can browse and select paths for the game directory and the mod repository.
- **📦 Data Packaging:** Automatically packages specified directories into `.pak` files.
- **📑 Presets Management:** Saves and loads presets for configurations, allowing faster setups for different mods.
- **✅ Validation:** Ensures that the selected paths and input data are valid.
- **🤫 Silent Mode:** Use the `-silent` parameter to run the app without the UI for a faster process. If no preset is specified, the last used preset will be applied.

## ⚙️ Requirements

- 💻 .NET 9.0 or later
- 🖥️ Windows operating system

## ⬇️ Installation

- Download the latest release

## 🚀 Usage

1. **▶️ Run the Application:** Start the application by running the executable.
2. **🔍 Set Paths:** Use the browse buttons to set the paths for the game directory and your mod repository.
3. **🖊️ Fill in Details:** Enter the mod name, version, and author information.
4. **📑 Preset Selection:** You can easily select any previously saved preset from the dropdown menu for quick access.
5. **🏗️ Create Mod Folder:** Click the "Run" button to create the mod folder and package the files.
6. **🤫 Silent Mode:** For a faster process, you can run the app with the `-silent` parameter and specify a preset. If no preset is specified, the last used preset will be automatically applied.

## 📝 License

This tool is licensed under the **Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)**.  
You are free to:
- **Use**: Use the tool for personal and non-commercial purposes.
- **Share**: Copy and redistribute the tool in any medium or format.
- **Adapt**: Remix, transform, and build upon the tool.

Under the following terms:
- **Attribution**: You must give appropriate credit, provide a link to the license, and indicate if changes were made.
- **NonCommercial**: You may not use the tool for commercial purposes.
- **ShareAlike**: If you remix, transform, or build upon the tool, you must distribute your contributions under the same license as the original.

For more information, see the [full license](https://creativecommons.org/licenses/by-nc-sa/4.0/).  
![CC BY-NC-SA 4.0](https://licensebuttons.net/l/by-nc-sa/4.0/88x31.png)
