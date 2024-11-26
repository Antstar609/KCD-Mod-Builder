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

1. Download the latest release from the [releases page](https://github.com/Antstar609/KCD-Mod-Packer/releases).
2. Extract the contents of the zip file.
3. Ensure you have .NET 9.0 installed. If not, download it from [Microsoft's official site](https://dotnet.microsoft.com/download/dotnet/9.0).
4. Run the `KCDModPacker.exe` file.

## 🚀 Usage

1. **▶️ Run the Application:** Start the application by running the executable.
2. **🔍 Set Paths:** Use the browse buttons to set the paths for the game directory and your mod repository.
3. **🖊️ Fill in Details:** Enter the mod name, version, and author information.
4. **📑 Preset Selection:** You can easily select any previously saved preset from the dropdown menu for quick access.
5. **🏗️ Create Mod Folder:** Click the "Run" button to create the mod folder and package the files.
6. **🤫 Silent Mode:** For a faster process, you can run the app with the `-silent` parameter and specify a preset. If no preset is specified, the last used preset will be automatically applied.