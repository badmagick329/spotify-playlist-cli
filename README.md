# Spotify Playlist CLI

A command-line tool to create filtered Spotify playlists from your existing playlists. Filter by release date range and/or specific artists to create more targeted playlists from your larger collections.

## Features

- Select one or more source playlists (including your Saved Tracks library)
- Filter tracks by release date range (`yyyy`, `yyyy-mm`, or `yyyy-mm-dd`)
- Filter tracks by specific artists
- Combine filters (AND logic, tracks must match all applied filters)

## Requirements

- A Spotify account
- A Spotify Developer App

## Setup

### 1. Create a Spotify Developer App

1. Go to the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Create a new app
3. Add `http://localhost:8000/callback` as a Redirect URI in your app settings
4. Copy your **Client ID**

### 2. Configure the Application

1. Copy `spotifycli-settings.json.sample` to `spotifycli-settings.json` (in the same directory as the binary)
2. Replace the `ClientId` value with your Client ID from the Spotify Developer Dashboard

```json
{
  "ClientId": "your-client-id-here",
  "CredentialsPath": "credentials.json",
  "CallbackUrl": "http://localhost:8000/callback",
  "Port": "8000"
}
```

### 3. First Run

On first run, the app will:

1. Start a local server on port 8000
2. Open your browser to Spotify's login page
3. After you authorize, credentials are saved to `credentials.json`

Subsequent runs will use the saved credentials automatically.

### Permissions Requested

The app requests the following Spotify permissions:

| Permission                    | Purpose                         |
| ----------------------------- | ------------------------------- |
| `playlist-read-private`       | Read your private playlists     |
| `playlist-read-collaborative` | Read collaborative playlists    |
| `playlist-modify-private`     | Create/modify private playlists |
| `playlist-modify-public`      | Create/modify public playlists  |
| `user-library-read`           | Read your Saved Tracks          |
| `user-library-modify`         | Modify your library             |
| `user-read-private`           | Read your user profile          |
| `user-top-read`               | Read your top artists/tracks    |

## Usage

Run the executable and follow the interactive prompts:

1. Enter a name for your new playlist
2. Select source playlists (space to select, enter to confirm)
3. Choose filter types (date range and/or artists)
4. Apply your filters
5. The filtered playlist is created on Spotify

## Building from Source

Requires .NET 9.0 SDK.

```bash
# Run directly
dotnet run --project SpotifyCli.Console

# Build self-contained executables for any platform
# Windows
dotnet publish SpotifyCli.Console -c Release -r win-x64 --output ./publish/windows --self-contained -p:PublishSingleFile=true

# macOS
dotnet publish SpotifyCli.Console -c Release -r osx-x64 --output ./publish/osx --self-contained -p:PublishSingleFile=true

# Linux
dotnet publish SpotifyCli.Console -c Release -r linux-x64 --output ./publish/linux --self-contained -p:PublishSingleFile=true
```

Or use the Makefile:

```bash
make all    # Build for all platforms
make win    # Windows only
make osx    # macOS only
make linux  # Linux only
```
