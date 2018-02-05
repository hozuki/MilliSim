"use strict";

const path = require("path");
const os = require("os");
const child_process = require("child_process");
const glob = require("glob");
const chalk = require("chalk");

const welcomeScreen = `
    |=======================================================|
    |========= Recursive NuGet Pack Invoker by MIC =========|
    |=======================================================|
    `;
console.info(chalk.yellow(welcomeScreen));

const AlienProjectPattern = /thirdparty/i;

const appveyorRepoTag = process.env.APPVEYOR_REPO_TAG;

if (appveyorRepoTag === void(0) || appveyorRepoTag.toLowerCase() !== "true") {
    console.info(`Not a deployment, exiting. (APPVEYOR_REPO_TAG is "${appveyorRepoTag}", should be "true")`);
} else {
    glob(path.join(process.cwd(), "**/*.nuspec"), packPackages);
}

function packPackages(err, fileList) {
    if (err) {
        console.error(chalk.red(err));
        process.exit(1);
    }

    console.info(chalk.green("Finding NuGet spec files..."));

    const filesToPack = [];

    for (const fileName of fileList) {
        if (!AlienProjectPattern.test(fileName)) {
            /**
             * @type {string}
             */
            let f = fileName.replace(/\.nuspec/i, ".csproj");
            filesToPack.push(winpath(f));
        }
    }

    let packed = 0;
    for (const fileName of filesToPack) {
        console.info(`Packing ${chalk.magenta(fileName)} ... (${packed + 1}/${filesToPack.length})`);

        const name = path.basename(fileName);
        const dir = path.dirname(fileName);

        const params = [
            "pack", name,
            //"-Build",
            "-IncludeReferencedProjects",
            "-OutputDirectory", winpath(process.cwd()),
            "-Prop", `Configuration=${process.env["CONFIGURATION"]};Platform=AnyCPU;OutputPath=${winpath(getOutputPath(dir))}`
        ];
        //if (process.env["Release_Suffix"]) {
        //    params.push("-Suffix", process.env["Release_Suffix"].replace("-", ""));
        //}
        const stdout = child_process.execFileSync("nuget", params, {
            cwd: dir,
            env: process.env
        }, (proc_err) => {
            if (proc_err) {
                console.error(chalk.red(proc_err));
            }
        });
        console.log(chalk.grey(stdout));

        ++packed;
    }

    /**
     * @param {string} p 
     */
    function winpath(p) {
        return os.platform() === "win32" ? p.replace(/\//g, "\\") : p;
    }

    /**
     * @param {string} basePath 
     */
    function getOutputPath(basePath) {
        const configuration = process.env["CONFIGURATION"];
        const platform = process.env["PLATFORM"];
        const plat = platform.toLowerCase();

        if (plat === "anycpu" || plat === "any cpu") {
            return path.join(basePath, "bin", configuration);
        } else {
            return path.join(basePath, "bin", platform, configuration);
        }
    }
}
