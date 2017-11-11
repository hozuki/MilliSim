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
| Travis | [![Travis](https://img.shields.io/travis/hozuki/MilliSim.svg)](https://travis-ci.org/hozuki/MilliSim) |
| AppVeyor | [![AppVeyor](https://img.shields.io/appveyor/ci/hozuki/MilliSim.svg)](https://ci.appveyor.com/project/hozuki/MilliSim) |

**Stage:** alpha

**Miscellaneous:**

[![GitHub contributors](https://img.shields.io/github/contributors/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim/graphs/contributors)
[![Libraries.io for GitHub](https://img.shields.io/librariesio/github/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim)
[![license](https://img.shields.io/github/license/hozuki/MilliSim.svg)](LICENSE.txt)

## Usage

**Requirements:**

- Windows 7 SP1 or later <sup>1 2</sup>
- [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642)
- [Visual C++ 2015 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=53587)
- Direct3D 11 and Direct2D
- For Windows 7 SP1 users:
  - [Platform Update for Windows 7](https://www.microsoft.com/en-us/download/details.aspx?id=36805)
- [OpenAL](https://www.openal.org/downloads/)

After downloading, run `OpenMLTD.MilliSim.Theater.exe` and enjoy.

<sup>1</sup> <del>Theoratically, MilliSim can also run on a UNIX-compliant machine using [Wine](https://www.winehq.org/download)
and [Mono](http://www.mono-project.com/download/), though this is not tested.</del> Tested
on Wine 2.0.2. Media Foundation APIs and WAS APIs are still having tons of errors. Even after
bypassing OS version check (Wine seems to always return an OS major version less than 6),
`MMDeviceEnumerator` still fails to initialize due to COM object creation failure. `MFCreateDxgiDeviceManager()`
is just a stub. So up till September 2017, MilliSim is only able to run on Windows.

<sup>2</sup> Playing a background video is not yet (version: v0.2) supported on Windows 7. It is
supported on Windows 8 or later.

## Building

**Requirements:**

- OS: Windows, macOS or Linux
- Compiler and Toolchain:
    - Windows:
        - Visual Studio 2017 Community
        - .NET Framework 4.5 Toolchain
    - macOS/Linux: 
        - [Mono](http://www.mono-project.com/download/) (≥ 5.0)
- [Node.js](https://nodejs.org/en/download/) (≥ 5.0)
- [NuGet CLI](https://www.nuget.org/downloads) (≥ 2.8.12)

> **Remember** to [update your Mono version](http://www.mono-project.com/download/#download-lin) and
> [update your NuGet version](https://docs.microsoft.com/en-us/nuget/guides/install-nuget) before
> building. Otherwise you are very likely to see errors like "Too many projects specified".

**Step 1**: Clone this repo:

```bash
git clone https://github.com/hozuki/MilliSim.git
```

**Step 2**: Prepare dependencies: 

On Windows:

```cmd
cd MilliSim
init
```

On macOS and Linux:

```bash
cd MilliSim
./init.sh
```

**Step 3**: Build the solution:

```bash
msbuild MilliSim.sln /p:Configuration=Release
```

If you are using non-Windows machines, you should
manually specify the concurrent process count, or
there will be strange compile errors:

```bash
msbuild MilliSim.sln /p:Configuration=Release /m:1
```

Although the builds by Travis seem unable to bootstrap on Windows,
manual builds on an Ubuntu 16.04 machine are verified to function
normally.

## Developing Plugins

MilliSim is designed to support plugins. Please read the [wiki page](https://github.com/hozuki/MilliSim/wiki/Creating-Plugins) for more information.

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
