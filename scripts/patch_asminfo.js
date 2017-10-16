"use strict";

const path = require("path");
const fs = require("fs");
const os = require("os");
const glob = require("glob");
const chalk = require("chalk");

runAssemblyPatcher();

function runAssemblyPatcher() {
    // Supports C# source files. VB probably works but not tested.

    const welcomeScreen = `
    |===============================================|
    |========= AssemblyInfo Patcher by MIC =========|
    |==== for Travis / AppVeyor CI environments ====|
    |===============================================|
    `;
    console.info(chalk.yellow(welcomeScreen));

    const forCustomBuildHelp = `
In supported CI environments, the variables are detected automatically.
For custom build environments, you must set these environment variables:

    - MAIN_VER: like "0.5.0", the format is "major.minor.revision".
    - BUILD_NUMBER: like "1", an integer.

`;
    console.info(forCustomBuildHelp);
    
    const AlienProjectPattern = /thirdparty/i;

    const CiEnvironment = {
        invalid: 0,
        travis: 1,
        appveyor: 2
    };

    let thisCi;
    if (process.env["TRAVIS"]) {
        thisCi = CiEnvironment.travis;
    } else if (process.env["APPVEYOR"]) {
        thisCi = CiEnvironment.appveyor;
    } else {
        const platform = os.platform();
        if (platform === "win32") {
            thisCi = CiEnvironment.appveyor;
        } else {
            thisCi = CiEnvironment.travis;
        }
    }

    /**
     * E.g. "0.1.0"
     * @type {string}
     */
    let MAIN_VER;
    switch (thisCi) {
        case CiEnvironment.travis:
            // Used together with .travis.yml.
            MAIN_VER = process.env["MAIN_VER"];
            if (MAIN_VER === void (0)) {
                console.error(chalk.red("Unable to detect MAIN_VER."));
                process.exit(1);
            }
        case CiEnvironment.appveyor:
            MAIN_VER = process.env["MAIN_VER"];
            if (MAIN_VER === void (0)) {
                const buildVersion = process.env["APPVEYOR_BUILD_VERSION"];
                if (buildVersion === void (0)) {
                    console.error(chalk.red("Unable to detect MAIN_VER or APPVEYOR_BUILD_VERSION."));
                    process.exit(1);
                }
                const bvs = buildVersion.split(".");
                // Remove the "build number" part.
                bvs.pop();
                MAIN_VER = bvs.join(".");
            }
            break;
        default:
            break;
    }
    if (MAIN_VER === void (0)) {
        console.error(chalk.red("Unable to handle MAIN_VER."));
        process.exit(1);
    }

    /**
     * E.g. "5"
     * @type {string}
     */
    let BUILD_NUMBER;
    switch (thisCi) {
        case CiEnvironment.travis:
            BUILD_NUMBER = process.env["TRAVIS_BUILD_NUMBER"];
            if (BUILD_NUMBER === void (0)) {
                BUILD_NUMBER = process.env["BUILD_NUMBER"];
            }
            break;
        case CiEnvironment.appveyor:
            BUILD_NUMBER = process.env["APPVEYOR_BUILD_NUMBER"];
            if (BUILD_NUMBER === void (0)) {
                BUILD_NUMBER = process.env["BUILD_NUMBER"];
            }
            break;
        default:
            break;
    }
    if (BUILD_NUMBER === void (0)) {
        console.error(chalk.red("Unable to handle BUILD_NUMBER."));
        process.exit(1);
    }

    const VERSION_TO_BE_PATCHED = `${MAIN_VER}.${BUILD_NUMBER}`;
    const SEMVER_TO_BE_PATCHED = MAIN_VER;

    const versionStrings = MAIN_VER.split(".");
    const versionObject = {
        major: Number.parseInt(versionStrings[0]),
        minor: Number.parseInt(versionStrings[1]),
        revision: Number.parseInt(versionStrings[2])
    };

    const CODE_NAME_TO_BE_PATCHED = getMilliSimCodeName(versionObject.major, versionObject.minor, versionObject.revision).replace("\"", "\\\"");

    const BUILD_TIME_TO_BE_PATCHED = (new Date()).toUTCString();

    glob(path.join(process.cwd(), "**/AssemblyInfo.*"), findAndPatchAssemblyInfo);
    glob(path.join(process.cwd(), "**/*.nuspec"), findAndPatchNuspec);

    /**
     * @param {Error} err 
     * @param {string[]} fileList
     */
    function findAndPatchAssemblyInfo(err, fileList) {
        if (err) {
            console.error(chalk.red(err));
            process.exit(1);
        }

        /**
         * @type {string[]}
         */
        const filesToPatch = [];

        console.info(chalk.green("Finding AssemblyInfo files..."));
        
        for (const fileName of fileList) {
            if (!AlienProjectPattern.test(fileName)) {
                filesToPatch.push(fileName);
            }
        }

        let patched = 0;
        for (const fileName of filesToPatch) {
            console.info(`Patching ${chalk.magenta(fileName)} ... (${patched + 1}/${filesToPatch.length})`);
            patchAssemblyInfo(fileName);
            ++patched;
        }

        console.info(chalk.green(`Done. Patched ${patched} AssemblyInfo file(s).`));
    }

    /**
     * @param {string} fileName 
     */
    function patchAssemblyInfo(fileName) {
        const AssemblyVersionPattern = /AssemblyVersion\("[^"]+"\)/g;
        const AssemblyFileVersionPattern = /AssemblyFileVersion\("[^"]+"\)/g;
        const MilliSimCodeNamePattern = /MilliSimCodeName\((?:"[^"]+"|[^)]+)\)/g;
        const AssemblyBuildTimePattern = /AssemblyBuildTime\((?:"[^"]+"|[^)]+)\)/g;
        
        let fileContent = fs.readFileSync(fileName, "utf-8");
        fileContent = fileContent.replace(AssemblyVersionPattern, `AssemblyVersion("${VERSION_TO_BE_PATCHED}")`);
        fileContent = fileContent.replace(AssemblyFileVersionPattern, `AssemblyFileVersion("${VERSION_TO_BE_PATCHED}")`);
        fileContent = fileContent.replace(MilliSimCodeNamePattern, `MilliSimCodeName("${CODE_NAME_TO_BE_PATCHED}")`);
        fileContent = fileContent.replace(AssemblyBuildTimePattern, `AssemblyBuildTime("${BUILD_TIME_TO_BE_PATCHED}")`);
        fs.writeFileSync(fileName, fileContent);
    }
    
    /**
     * @param {Error} err 
     * @param {string[]} fileList
     */
    function findAndPatchNuspec(err, fileList) {
        if (err) {
            console.error(chalk.red(err));
            process.exit(1);
        }

        /**
         * @type {string[]}
         */
        const filesToPatch = [];

        console.info(chalk.green("Finding NuGet spec files..."));
        
        for (const fileName of fileList) {
            if (!AlienProjectPattern.test(fileName)) {
                filesToPatch.push(fileName);
            }
        }

        let patched = 0;
        for (const fileName of filesToPatch) {
            console.info(`Patching ${chalk.magenta(fileName)} ... (${patched + 1}/${filesToPatch.length})`);
            patchNuspec(fileName);
            ++patched;
        }

        console.info(chalk.green(`Done. Patched ${patched} nuspec file(s).`));
    }

    /**
     * @param {string} fileName 
     */
    function patchNuspec(fileName) {
        const NuspecVersionPattern = /\<version\>[^<]+\<\/version\>/g;
        
        let fileContent = fs.readFileSync(fileName, "utf-8");
        fileContent = fileContent.replace(NuspecVersionPattern, `<version>${SEMVER_TO_BE_PATCHED}</version>`);
        fs.writeFileSync(fileName, fileContent);
    }

    /**
     * @param {number} major 
     * @param {number} minor 
     * @param {number} revision 
     */
    function getMilliSimCodeName(major, minor, revision) {
        /**
         * @type {string[]}
         */
        const nameList = [
            "Haruka", "Chihaya", "Miki", "Yukiho", "Yayoi", "Makoto",
            "Iori", "Takane", "Ritsuko", "Azusa", "Ami", "Mami",
            "Hibiki", "Haruka", "Shizuka", "Tsubasa", "Kotoha", "Elena",
            "Minako", "Megumi", "Matsuri", "Serika", "Akane", "Anna",
            "Roco", "Yuriko", "Sayoko", "Arisa", "Umi", "Iku",
            "Tomoka", "Emily", "Shiho", "Ayumu", "Hinata", "Kana",
            "Nao", "Chizuru", "Konomi", "Tamaki", "Fuuka", "Miya",
            "Noriko", "Mizuki", "Karen", "Rio", "Subaru", "Reika",
            "Momoko", "Julia", "Tsumugi", "Kaori",

            "Kotori", "Misaki",
            // So what is this?
            "Camerawoman"
        ];

        switch (major) {
            case 0:
                if (minor <= nameList.length) {
                    return nameList[minor - 1];
                }
                break;
        }

        return "THE iDOLM@STER";
    }
}
