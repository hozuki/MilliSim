"use strict";

const path = require("path");
const child_process = require("child_process");
const chalk = require("chalk");
const glob = require("glob");

runNuGetRestore();

function runNuGetRestore() {
    const welcomeScreen = `
    |=======================================|
    |=== Recursive NuGet Restorer by MIC ===|
    |=======================================|
    `;
    console.info(chalk.yellow(welcomeScreen));

    glob(path.join(process.cwd(), "**/*.csproj"), restoreSolutions);

    /**
     * @param {Error} err 
     * @param {string[]} fileList
     */
    function restoreSolutions(err, fileList) {
        if (err) {
            console.error(chalk.red(err));
            process.exit(1);
        }

        console.info(chalk.green("Finding solution files..."));

        let i = 0;
        const total = fileList.length;
        for (const projPath of fileList) {
            console.info(`Restoring ${chalk.magenta(projPath)} ... (${i + 1}/${total})`);

            const projDir = path.dirname(projPath);
            const projFile = path.basename(projPath);

            child_process.execFileSync("nuget", ["restore", projFile, "-recursive"], {
                cwd: projDir,
                env: process.env
            }, (proc_err) => {
                if (proc_err) {
                    console.error(chalk.red(proc_err));
                }
            });

            ++i;
        }

        console.info(chalk.green("Completed restoring packages."));
    }
}
