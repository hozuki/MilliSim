const path = require("path");
const fs = require("fs");
const glob = require("glob");
const chalk = require("chalk");

runAssemblyPatcher();

function runAssemblyPatcher() {
    // Supports C# source files. VB probably works but not tested.

    const AlienProjectPattern = /thirdparty/i;

    const AssemblyVersionPattern = /AssemblyVersion\("([^"]+)"\)/;
    const AssemblyFileVersionPattern = /AssemblyFileVersion\("([^"]+)"\)/;

    const welcomeScreen = `
    |===================================|
    |=== AssemblyInfo Patcher by MIC ===|
    |===  for Travis CI environment  ===|
    |===================================|
    `;
    console.info(chalk.yellow(welcomeScreen));

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
        fs.writeFileSync(fileName, fileContent);
    }
}
