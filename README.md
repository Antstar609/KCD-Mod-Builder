# KCD Mod Builder

KCD Mod Builder is a tool designed to convert your mod repository into a format that *Kingdom Come: Deliverance* can read. It packages your folders into `.pak` files and sets up the folder structure.

[![GitHub release](https://img.shields.io/github/release/Antstar609/KCD-Mod-Builder.svg)](https://github.com/Antstar609/KCD-Mod-Builder/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/Antstar609/KCD-Mod-Builder/total.svg)](https://github.com/Antstar609/KCD-Mod-Builder/releases/latest)

## ⚙️ Requirements

- Windows 10/11

## 🚀 Installation

1. **Download:** Grab the latest release from the [releases page](https://github.com/Antstar609/KCD-Mod-Builder/releases).
2. **Extract:** Unzip the downloaded file.
3. **Run:** Launch the `KCDModBuilder.exe` file.

## 🎮 Usage

- **Preset Selection:** Presets are saved automatically when a mod is successfully packed. You can quickly pick a saved preset from the dropdown to load your mod’s configuration.
- **Silent Mode:** Run the tool with the `-silent` parameter in the terminal. Use one of the following syntaxes:
  - `KCDModBuilder.exe -silent PresetName` – to run with a specific preset.
  - `KCDModBuilder.exe -silent` – to run with the last preset used.

## Repository Structure

To ensure compatibility with KCD Mod Builder, organize your mod's repository to include any of the following folders: `Scripts`, `Libs`, `Entities`, `Objects`, and `Localization`. The tool will compress any combination of these folders present in your project. Folders not listed above will be excluded from the compression process.

**Note:** The provided example below demonstrates a possible folder structure. However, your project does not need to include all these folders; having just one is sufficient for the tool to function correctly.

### Exemple:
```plaintext

Entities
└── entity.ent

Scripts
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

![KCD-Mod-Builder-screenshot](https://github.com/user-attachments/assets/1939dc45-5a04-4bcd-99a0-6923af5ef509)
