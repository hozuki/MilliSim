# MilliSim

A simulator for [THE iDOLM@STER Million Live! Theater Days](https://millionlive.idolmaster.jp/theaterdays/).

| Build Status | |
|--|--|
| Travis | [![Travis](https://img.shields.io/travis/hozuki/MilliSim.svg)](https://travis-ci.org/hozuki/MilliSim) |
| AppVeyor | [![AppVeyor](https://img.shields.io/appveyor/ci/hozuki/MilliSim.svg)](https://ci.appveyor.com/project/hozuki/MilliSim) |

| Downloads| |
|--|--|
| GitHub Releases | [![GitHub (pre-)release](https://img.shields.io/github/release/hozuki/MilliSim/all.svg) ![Github All Releases](https://img.shields.io/github/downloads/hozuki/MilliSim/total.svg)](https://github.com/hozuki/MilliSim/releases) |
| Latest Development Build | [AppVeyor](https://ci.appveyor.com/api/projects/hozuki/MilliSim/artifacts/millisim-appveyor-latest.zip) |

**Stage:** alpha

**Miscellaneous:**

[![GitHub contributors](https://img.shields.io/github/contributors/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim/graphs/contributors)
[![Libraries.io for GitHub](https://img.shields.io/librariesio/github/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim)
[![license](https://img.shields.io/github/license/hozuki/MilliSim.svg)](LICENSE.txt)

## Usage

**Requirements:**

- Windows 7 or later <sup>1</sup>
- [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642)
- Visual C++ 2010 Runtime ([32-bit](https://www.microsoft.com/en-us/download/details.aspx?id=8328) | [64-bit](https://www.microsoft.com/en-us/download/details.aspx?id=13523))
- [Visual C++ 2015 Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=53587)
- Direct3D 11 and Direct2D
- For Windows 7 users:
  - [Platform Update for Windows 7](https://www.microsoft.com/en-us/download/details.aspx?id=36805)

After downloading, run `OpenMLTD.MilliSim.Theater.exe` and enjoy.

<sup>1</sup> <del>Theoratically, MilliSim can also run on a UNIX-compliant machine using [Wine](https://www.winehq.org/download)
and [Mono](http://www.mono-project.com/download/), though this is not tested.</del> Tested
on Wine 2.0.2. Media Foundation APIs and WAS APIs are still having tons of errors. Even after
bypassing OS version check (Wine seems to always return an OS major version less than 6),
`MMDeviceEnumerator` still fails to initialize due to COM object creation failure. `MFCreateDxgiDeviceManager()`
is just a stub. So up till September 2017, MilliSim is only able to run on Windows.

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

With newer versions of Git:

```bash
git clone https://github.com/hozuki/MilliSim.git --recursive
cd MilliSim
```

With older versions of Git:

```bash
git clone https://github.com/hozuki/MilliSim.git
cd MillSim
git submodule update --init --recursive
```

**Step 2**: Restore dependencies using NuGet CLI.

```bash
npm install glob chalk --save
node before_script-nuget_restore.js
```

(Optional) Patch `Assembly.cs`:

```cmd
REM On Windows
set MAIN_VER=0.0.0
set BUILD_NUMBER=0
node before_script-patch_asminfo.js
```

```bash
# On macOS/Linux
MAIN_VER=0.0.0 BUILD_NUMBER=0 node before_script-patch_asminfo.js
```

**Step 3**: Build the solution.

```bash
msbuild MilliSim.sln /p:Configuration=Release
```

If you are using non-Windows machines, you should
manually specify the concurrent process count, or
there will be strange compile errors:

```bash
msbuild MilliSim.sln /p:Configuration=Release /m:1
```

To update external projects, use this command:

```bash
git submodule update --recursive
```

Although the builds by Travis seem unable to bootstrap on Windows,
manual builds on an Ubuntu 16.04 machine are verified to function
normally.

## Developing Plugins

MilliSim is designed to support plugins. Please read the [wiki page](https://github.com/hozuki/MilliSim/wiki/Creating-Plugins) for more information.

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
