# MilliSim

A simulator for [THE iDOLM@STER Million Live Theater Days](https://millionlive.idolmaster.jp/theaterdays/).

**Stage:** pre-alpha

## Usage

**Requirements:**

- Windows 7 or later
- [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642)

Theoratically, MilliSim can also run on a UNIX-compliant machine using [Wine](https://www.winehq.org/download)
and [Mono](http://www.mono-project.com/download/), though this is not tested.

## Building

**Requirements:**

- Visual Studio 2017 Community (optional if you are familiar with MSBuild)
- .NET Framework 4.5 Toolchain

Step 1: Clone this repo:

```bash
git clone https://github.com/hozuki/MilliSim.git --recursive
```

Step 2: Restore dependencies using [NuGet](https://www.nuget.org/downloads) CLI, or Visual Studio.

```cmd
nuget restore MilliSim.sln
```

If you are using Visual Studio, you can restore dependencies by right-clicking the solution node
and select `Restore NuGet packages`.

Step 3: Build the solution.

## Notes

### Naming

The name is an abbreviation of **Milli**on Live **Sim**ulator.

According to English traditions, it should be written and read as *MilliSim*.
Corresponding Japanese version should be *ミリシミュ* (MilliSimu/*mirisimyu*).
Both versions are acceptable.

## License

MIT
