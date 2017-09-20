const path = require("path");
const fs = require("fs");
const glob = require("glob");
const chalk = require("chalk");

runAssemblyPatcher();

function runAssemblyPatcher() {
    // Supports C# source files. VB probably works but not tested.

    const welcomeScreen = `
    |===================================|
    |=== AssemblyInfo Patcher by MIC ===|
    |===  for Travis CI environment  ===|
    |===================================|
    `;
    console.info(chalk.yellow(welcomeScreen));

    const AlienProjectPattern = /thirdparty/i;

    const AssemblyVersionPattern = /AssemblyVersion\("([^"]+)"\)/;
    const AssemblyFileVersionPattern = /AssemblyFileVersion\("([^"]+)"\)/;
    const MilliSimCodeNamePattern = /MilliSimCodeName\(("[^"]+"|[^)]+)\)/;
    const AssemblyBuildTimePattern = /AssemblyBuildTime\(("[^"]+"|[^)]+)\)/;

    /**
     * E.g. "0.1.0"
     * @type {string}
     */
    const MAIN_VER = process.env["MAIN_VER"];
    /**
     * E.g. "5"
     * @type {string}
     */
    const TRAVIS_BUILD_NUMBER = process.env["TRAVIS_BUILD_NUMBER"];

    const VERSION_TO_BE_PATCHED = `${MAIN_VER}.${TRAVIS_BUILD_NUMBER}`;

    const versionStrings = MAIN_VER.split(".");
    const versionObject = {
        major: Number.parseInt(versionStrings[0]),
        minor: Number.parseInt(versionStrings[1]),
        revision: Number.parseInt(versionStrings[2])
    };

    const CODE_NAME_TO_BE_PATCHED = getMilliSimCodeName(versionObject.major, versionObject.minor, versionObject.revision).replace("\"", "\\\"");

    const BUILD_TIME_TO_BE_PATCHED = (new Date()).toUTCString();

    traverseFiles(__dirname, findAssemblyInfo);

    /**
     * Traverse files in the specified source directory.
     * @param {string} base 
     * @param {function (err, string): void} callback 
     */
    function traverseFiles(base, callback) {
        glob(path.join(base, "**/AssemblyInfo.*"), callback);
    }

    /**
     * @param {Error} err 
     * @param {string[]} fileList
     */
    function findAssemblyInfo(err, fileList) {
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

        console.info(chalk.green(`Found ${filesToPatch.length} candidates.`));

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
        let fileContent = fs.readFileSync(fileName, "utf-8");
        fileContent.replace(AssemblyVersionPattern, `AssemblyVersion("${VERSION_TO_BE_PATCHED}")`);
        fileContent.replace(AssemblyFileVersionPattern, `AssemblyFileVersion("${VERSION_TO_BE_PATCHED}")`);
        fileContent.replace(MilliSimCodeNamePattern, `MilliSimCodeName("${CODE_NAME_TO_BE_PATCHED}")`);
        fileContent.replace(AssemblyBuildTimePattern, `AssemblyBuildTime("${BUILD_TIME_TO_BE_PATCHED}")`);
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
            "Momoko", "Julia", "Tsumigi", "Kaori",

            "Kotori", "Misaki",
            // So what is this?
            "Camerawoman"
        ];

        switch (major) {
            case 0:
                if (minor < nameList.length) {
                    return nameList[minor];
                }
                break;
        }

        return "THE iDOLM@STER";
    }
}
