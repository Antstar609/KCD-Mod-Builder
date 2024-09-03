# 🎮 KCD Mod Packer

## 📝 Overview
**KCD Mod Packer** is a C# application designed to simplify the process of converting a mod repository into a format that *Kingdom Come: Deliverance* can read. The app packages mod repository folders into `.pak` files and creates a proper folder structure, making it easier for developers to prepare mods for the game and streamline testing.

## ✨ Features

- **📂 Path Selection:** Users can browse and select paths for the game directory and the mod repository.
- **📦 Data Packaging:** Automatically packages specified directories into `.pak` files.
- **💾 User Data Management:** Saves and loads user data to and from a JSON file.
- **✅ Validation:** Ensures that the selected paths and input data are valid.
- **🤫 Silent Mode:** Use the `--silent` parameter to run the app without the UI for a faster process. Note: This requires that the app be used with the UI at least once for initial setup.

## ⚙️ Requirements

- 💻 .NET 9.0 or later
- 🖥️ Windows operating system

## ⬇️ Installation

- Download the latest release

## 🚀 Usage

1. **▶️ Run the Application:** Start the application by running the executable.
2. **🔍 Set Paths:** Use the browse buttons to set the paths for the game directory and your mod repository.
3. **🖊️ Fill in Details:** Enter the mod name, version, and author information.
4. **🏗️ Create Mod Folder:** Click the "Run" button to create the mod folder and package the files.
5. **🤫 Silent Mode:** For a faster process, you can run the app with the `--silent` parameter. Ensure you have used the UI version of the app at least once to set it up.