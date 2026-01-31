# poopooVR Better Leaderboard

A BepInEx mod for Gorilla Tag that enhances the in-game leaderboard to display FPS (frames per second) and ping/latency metrics alongside existing player information.

## Features

- Display FPS for all players on the leaderboard
- Display ping/latency in milliseconds for all players
- Maintains all original leaderboard elements (player names, color boxes)
- Optimized for VR readability
- Minimal performance impact

## Requirements

- Gorilla Tag (Steam or Quest version)
- BepInEx 5.x installed in Gorilla Tag
- .NET Framework 4.7.2 or higher

## Installation

1. Ensure BepInEx is installed in your Gorilla Tag directory
2. Download the latest release of `poopooVRBetterLeaderboard.dll`
3. Copy the DLL to `Gorilla Tag\BepInEx\plugins\`
4. Launch Gorilla Tag

## Building from Source

### Prerequisites

- .NET SDK 6.0 or higher
- Visual Studio 2022 or Rider (optional, for IDE support)
- Gorilla Tag installed

### Build Steps

1. Clone this repository
2. Place Gorilla Tag DLL dependencies in the `dependencies` folder (see dependencies folder structure)
3. Build the project:
   ```bash
   dotnet build
   ```
4. The compiled DLL will be in `bin/Debug/net472/poopooVRBetterLeaderboard.dll` or `bin/Release/net472/poopooVRBetterLeaderboard.dll`

## How It Works

This mod uses Harmony to patch the `GorillaPlayerScoreboardLine.UpdatePlayerText()` method at runtime. It reads the built-in FPS property from each player's VRRig and displays it alongside their ping information on the leaderboard.

## Compatibility

- Compatible with BepInEx 5.x
- Designed to work alongside other Gorilla Tag mods
- May require updates when Gorilla Tag's leaderboard system changes

## License

MIT License - Feel free to modify and distribute

## Credits

Created by poopooVR
