Swytch provides a collection of official starter templates to help you get up and running with your projects faster.
These templates are designed to eliminate the need for repetitive boilerplate setup, letting you jump straight into
building your application. Whether you're creating a lean API, a more structured backend, or a server-rendered web
application, Swytch's starter templates offer flexible starting points suited to your development needs.

Currently, Swytch has a [template pack](https://www.nuget.org/packages/Swytch.Template.Pack/) with three templates:

- `swytch-api-lite` – A lightweight API template with all routing and logic defined inline in the `Server.cs` file.
  Ideal for small projects and quick prototypes.
- `swytch-api` – A structured API template that separates routing and handler logic, organizing handlers in an
  `Actions/` directory.
- `swytch-web` – A web application template pre-configured with Swytch's templating engine and routing capabilities.
  Perfect for building dynamic server-rendered sites.

Each template reflects a different architectural approach using a playlist application while maintaining Swytch’s simplicity and developer-first
philosophy. You can choose the one that fits your use case and evolve your app from there.

Now let's explore them in more detail.

## Installation

### Installing the Swytch Template Pack

To start using Swytch templates, you first need to install the Swytch template pack
from [NuGet](https://www.nuget.org/packages/Swytch.Template.Pack/). This gives you access to
all available Swytch starter templates directly through the .NET CLI.

Run the following command:

```bash
dotnet new install Swytch.Templates
```

## Usage
#### creating / startig projects with the templates

#### Swytch API
```bash
dotnet new swytch-api -n MyApi
```

##### Sample output 📦
```
MySwytchApi/
├── Actions/PlaylistAction.cs
├── DTOs/AddPlaylist.cs, AddSong.cs
├── Helpers/DatabaseHelpers.cs
├── Models/Playlist.cs, Song.cs
├── Services/
│   ├── Interfaces/IPlaylistService.cs
│   └── Implementation/PlaylistService.cs
├── Statics/index.html, logo-5.png
├── Server.cs
└── Swytch-Api-Template.csproj
```

#### Swytch API Lite
```bash
dotnet new swytch-api-lite -n MyApi
```
##### Sample output 📦
```
MySwytchApi/
├── DTOs/AddPlaylist.cs, AddSong.cs
├── Helpers/DatabaseHelpers.cs
├── Models/Playlist.cs, Song.cs
├── Services/
│   ├── Interfaces/IPlaylistService.cs
│   └── Implementation/PlaylistService.cs
├── Statics/index.html, logo-5.png
├── Server.cs
└── Swytch-Api-Lite-Template.csproj
```

#### Swytch Web

```bash
dotnet new swytch-web -n MyWebApp
```
##### Sample output 📦
```
MySwytchApi/
├── Actions/PlaylistAction.cs
├── DTOs/AddPlaylist.cs, AddSong.cs
├── Helpers/DatabaseHelpers.cs
├── Models/Playlist.cs, Song.cs,  ViewList.cs
├── Services/
│   ├── Interfaces/IPlaylistService.cs
│   └── Implementation/PlaylistService.cs
├── Statics/logo-5.png, Style.css
├── Templates/AddSong.cshtml,BrowsePlaylist.cshtml,CreatePlaylist.cshtml,DeletePlaylist.cshtml,Layout.cshtml,PlaylistOperations.cshtml,ViewPlaylist.cshtml
├── Server.cs
└── Swytch-Web-Template.csproj
```


