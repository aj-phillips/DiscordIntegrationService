# 🎮 DiscordIntegrationService

A lightweight Windows desktop app that automatically updates your Discord Rich Presence based on your active window — with full customisation, smart exclusions, and a sleek modern UI.

## ✨ Features

✅ Rich Presence — Show what you’re working on or playing by pulling details from your active window.

✅ Presence Templates — Fully customise how your Rich Presence looks using templates with placeholders.

✅ Exclude List — Easily add apps or processes you don’t want to appear in your status.

✅ Launch on Startup — Optionally start automatically with Windows.

✅ Material-inspired UI — Clean, modern WPF design that’s easy to use.

✅ Tray Icon — Runs quietly in the system tray; open settings anytime.

## ⚙️ Installation

- Download the latest release from Releases (Add your link)
- Extract the zip somewhere safe
- Run DiscordIntegrationService.exe

ℹ️ First launch will generate a settings.json in the app directory.

## 🛠️ Getting Started

- Open Settings
  - Right click the DIS icon in your taskbar tray and click "Open Settings"
- Get your Discord Application ID:
  - Create an application at Discord Developer Portal
  - Copy the Application ID and paste it into the app’s Discord Application ID field.
- Customise your Presence:
  - Use the Presence Templates to adjust how your status appears.
  - Use {ProcessName} and {Title} placeholders to show your active window info.
- Manage Exclusions:
  - Add any process names you want ignored.
- Save and Go!
  - Click Save Settings — changes apply automatically!

## 🚀 Development

Built with:
- .NET 8
- WPF
- DiscordRPC.NET
- CommunityToolkit.Mvvm
- Microsoft.Extensions.Hosting

## 🧑‍💻 Contributing

Pull requests welcome!
For major changes, please open an issue first to discuss what you’d like to change.
