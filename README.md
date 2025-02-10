# KCD Mod Packer

KCD Mod Packer is a tool designed to convert your mod repository into a format that *Kingdom Come: Deliverance* can read. It packages your folders into `.pak` files and sets up the folder structure.

[![GitHub release](https://img.shields.io/github/release/Antstar609/KCD-Mod-Packer.svg)](https://github.com/Antstar609/KCD-Mod-Packer/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/Antstar609/KCD-Mod-Packer/total.svg)](https://github.com/Antstar609/KCD-Mod-Packer/releases/latest)

## ⚙️ Requirements

- Windows 10/11

## 🚀 Installation

1. **Download:** Grab the latest release from the [releases page](https://github.com/Antstar609/KCD-Mod-Packer/releases).
2. **Extract:** Unzip the downloaded file.
3. **Run:** Launch the `KCDModPacker.exe` file.

## 🎮 Usage

- **Preset Selection:** Presets are saved automatically when a mod is successfully packed. You can quickly pick a saved preset from the dropdown to load your mod’s configuration.
- **Silent Mode:** Run the tool with the `-silent` parameter in the terminal. Use one of the following syntaxes:
  - `KCDModPacker.exe -silent PresetName` – to run with a specific preset.
  - `KCDModPacker.exe -silent` – to run with the last preset used.

## Repository Structure

For your mod to work with KCD Mod Packer, your repository should follow this structure:

**Note:** The tool only works with the `Data`, `Libs`, and `Localization` folders.
If your project has only the `Data` folder or any combination of the three, they will be compressed.
Any other folders won't be included.

### Exemple:
```plaintext
Data
├── Entities
│   └── entity.ent
└── Scripts
    └── script.lua

Libs
└── Tables
    └── quest
        └── table.xml

Objects
└── characters
    └── humans
        └── cloth
            └── material.mtl

Localization
└── English
    └── english.xml
```

## Screenshot

![KCD-Mod-Packer-screenshot](https://github.com/user-attachments/assets/73f43715-46eb-480a-aee3-f437a89cbdbd)
