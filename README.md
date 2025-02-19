# KCD Mod Builder

KCD Mod Builder is a tool designed to convert your mod project folder into a format that Kingdom Come: Deliverance 1/2 can read. It packages your folders into yourmodname.pak, sets up the folder structure, and all you have to do is start the game to try it!

[![GitHub release](https://img.shields.io/github/release/Antstar609/KCD-Mod-Builder.svg)](https://github.com/Antstar609/KCD-Mod-Builder/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/Antstar609/KCD-Mod-Builder/total.svg)](https://github.com/Antstar609/KCD-Mod-Builder/releases/latest)

## :gear: Requirements

- Windows 10/11

## :rocket: Installation

1. **Download:** Grab the latest release from the [releases page](https://github.com/Antstar609/KCD-Mod-Builder/releases).
2. **Extract:** Unzip the downloaded file.
3. **Run:** Launch the `KCDModBuilder.exe` file.

## :video_game: Features

- **Preset Selection:** Presets are saved automatically when a mod is successfully packed. You can quickly pick a saved preset from the dropdown to load your mod’s configuration.
- **Silent Mode:** Run the tool with the `-silent` parameter in the terminal. Use one of the following syntaxes:
  - `KCDModBuilder.exe -silent PresetName` – to run with a specific preset.
  - `KCDModBuilder.exe -silent` – to run with the last preset used.
- **Manifest File Creation:** The tool generates a `mod.manifest` file.
- **EULA File Copying:** The tool copies the `modding_eula.txt` file, ensuring compliance with the game's End User License Agreement.

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
