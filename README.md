# MilliSim

A simulator for [THE iDOLM@STER Million Live Theater Days](https://millionlive.idolmaster.jp/theaterdays/).


[![AppVeyor](https://img.shields.io/travis/hozuki/MilliSim.svg)](https://travis-ci.org/hozuki/MilliSim)
[![GitHub contributors](https://img.shields.io/github/contributors/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim/graphs/contributors)
[![Libraries.io for GitHub](https://img.shields.io/librariesio/github/hozuki/MilliSim.svg)](https://github.com/hozuki/MilliSim)
[![Github All Releases](https://img.shields.io/github/downloads/hozuki/MilliSim/total.svg)](https://github.com/hozuki/MilliSim/releases)

**Stage:** pre-alpha

## Usage

**Requirements:**

- Windows 7 or later
- [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642)
- Direct3D 11 and Direct2D

<del>Theoratically, MilliSim can also run on a UNIX-compliant machine using [Wine](https://www.winehq.org/download)
and [Mono](http://www.mono-project.com/download/), though this is not tested.</del> Tested
on Wine 2.0.2. Media Foundation APIs and WAS APIs are still having tons of errors. Even after
bypassing OS version check (Wine seems to always return an OS major version less than 6),
`MMDeviceEnumerator` still fails to initialize due to COM object creation failure. `MFCreateDxgiDeviceManager()`
is just a stub. So up till September 2017, MilliSim is only able to run on Windows.

## Building

**Requirements:**

- Visual Studio 2017 Community (optional if using MSBuild CLI)
- .NET Framework 4.5 Toolchain
- [Mono](http://www.mono-project.com/download/) (required if you are using macOS or Linux) (≥ 5.0)
- [Node.js](https://nodejs.org/en/download/) (≥ 5.0)
- [NuGet CLI](https://www.nuget.org/downloads) (≥ 2.8.12)
- Windows, macOS, or Linux

> **Remember** to [update your Mono version](http://www.mono-project.com/download/#download-lin) and
> [update your NuGet version](https://docs.microsoft.com/en-us/nuget/guides/install-nuget) before
> building. Otherwise you are very likely to see errors like "Too many projects specified".

**Step 1**: Clone this repo:

```bash
git clone https://github.com/hozuki/MilliSim.git
cd MillSim
git submodule update --init --recursive
cd ..
```

**Step 2**: Restore dependencies using NuGet CLI.

```bash
npm install glob chalk --save
sudo node before_script-nuget_restore.js
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
git submodule foreach --recursive git pull origin master
```

## Extensions

MilliSim is designed to support extensions. Please check out examples:

- [`OpenMLTD.MilliSim.Extension.Imports.Unity3D.Unity3DScoreFormat`](OpenMLTD.MilliSim.Extension.Imports.Unity3D/Unity3DScoreFormat.cs) (custom score format)
- [`OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Vorbis.OggVorbisAudioFormat`](OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats/Vorbis/OggVorbisAudioFormat.cs) (custom audio format)

Plugin assemblies should be placed at:

- `$WORK_DIR`
- `$WORK_DIR/plugins`

where `$WORK_DIR` is MilliSim's working directory.

## Notes

### Naming

The name is an abbreviation of **Milli**on Live **Sim**ulator.

According to English traditions, it should be written and read as *MilliSim*.
Corresponding Japanese version should be *ミリシミュ* (MilliSimu/*mirisimyu*).
Both versions are acceptable.

## License

MIT
