# Building MilliSim

**Requirements:**

- OS: Windows, macOS or Linux
- Compiler and Toolchain:
    - Windows:
        - Visual Studio 2017 Community
        - .NET Framework 4.5 Toolchain
    - macOS/Linux: 
        - [Mono](http://www.mono-project.com/download/) (≥ 5.0)
- [Node.js](https://nodejs.org/en/download/) (≥ 5.0)
- [NuGet CLI](https://www.nuget.org/downloads) (≥ 4.3.0)
- [OpenAL](http://kcat.strangesoft.net/openal.html#download) (OpenAL-Soft): Unzip the ZIP file, rename `soft_oal.dll` in `Win32` to `openal32.dll`, and place it in the directory of MilliSim executable.

> **Remember** to [update your Mono version](http://www.mono-project.com/download/#download-lin) and
> [update your NuGet version](https://docs.microsoft.com/en-us/nuget/guides/install-nuget) before
> building. Otherwise you are very likely to see errors like "Too many projects specified".

**Step 1**: Clone this repo:

```bash
git clone https://github.com/hozuki/MilliSim.git
```

**Step 2**: Prepare dependencies:

First, install [MonoGame](http://www.monogame.net/downloads/) (version 3.6 or later).

Then, on Windows:

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
