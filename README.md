# MilliSim

More than a simulator for [THE iDOLM@STER Million Live! Theater Days](https://millionlive.idolmaster.jp/theaterdays/).

Demo videos: 

  - [video 1](https://www.bilibili.com/video/av15612246/) (v0.2 branch)
  - [video 2](https://www.bilibili.com/video/av16069466/) (v0.2 branch)

| Downloads| |
|--|--|
| [GitHub Releases](https://github.com/hozuki/MilliSim/releases) | ![GitHub (pre-)release](https://img.shields.io/github/release/hozuki/MilliSim/all.svg) ![Github All Releases](https://img.shields.io/github/downloads/hozuki/MilliSim/total.svg) |
| [AppVeyor](https://ci.appveyor.com/api/projects/hozuki/MilliSim/artifacts/millisim-appveyor-latest.zip) | (latest development build) |

| Build Status | |
|--|--|
| <del>Travis</del> (not maintained) | [![Travis](https://img.shields.io/travis/hozuki/MilliSim.svg)](https://travis-ci.org/hozuki/MilliSim) |
| AppVeyor | [![AppVeyor](https://img.shields.io/appveyor/ci/hozuki/MilliSim.svg)](https://ci.appveyor.com/project/hozuki/MilliSim) |

**Stage:** alpha

**Miscellaneous:**

[![GitHub contributors](https://img.shields.io/github/contributors/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim/graphs/contributors)
[![Libraries.io for GitHub](https://img.shields.io/librariesio/github/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim)
[![license](https://img.shields.io/github/license/hozuki/MilliSim.svg)](LICENSE.txt)

## Usage

**Requirements:**

- Operating System:
  - Windows: Windows 7 SP1 or later
    - [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642)
  - macOS and Linux: macOS 10.8 / Ubuntu 16.04 or later
    - and latest [Wine](https://wiki.winehq.org/Download) (will download `wine-mono` on launch)
- [Visual C++ 2015 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=53587)
- [OpenAL](https://www.openal.org/downloads/)

OpenGL builds (`TheaterDays.OpenGL.exe`) can run on all platforms, but it is problematic.
DirectX builds (`TheaterDays.Direct3D11.exe`) can only run on Windows.

### Note: Video Playback Support

If you want to play videos as the background, you must download FFmpeg binaries (version 3.4.1 or later)
from [here](https://ffmpeg.zeranoe.com/builds/), and place the binaries into `x64` and `x86` directories
correspondingly. Due to license restrictions, MilliSim builds cannot include FFmpeg binaries, so you have
to download them yourself.

Without FFmpeg, you must either:

1. disable background video (comment out `plugin.component_factory.background_video` in `Contents/plugins.yml`), or
2. leave video file path as empty (`data`-`video` section in `Contents/config/background_video.yml`).

## Building

Please read [Building.md](docs/Building.md).

## Developing Plugins

MilliSim is designed to support plugins. Please read the [wiki page](https://github.com/hozuki/MilliSim/wiki/Creating-Plugins) for more information.
(Note: this is mainly about version 0.2 so it may be deprecated. Proceed with caution. Help is appreciated!)

You can find precompiled NuGet packages for plugin development [here](https://www.nuget.org/packages?q=MilliSim).

## Other

### Wiki

MilliSim has a [wiki](https://github.com/hozuki/MilliSim/wiki) on GitHub. It is still under construction but you may find some useful information there.

### Naming

The name is an abbreviation of **Milli**on Live **Sim**ulator.

According to English traditions, it should be written and read as *MilliSim*.
Corresponding Japanese version should be *ミリシミュ* (MilliSimu/*mirisimyu*).
Both versions are acceptable.

## License

MIT
