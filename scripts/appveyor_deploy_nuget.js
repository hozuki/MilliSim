"use strict";

// http://help.appveyor.com/discussions/problems/8215-cannot-publish-to-nuget-get-400-bad-request-error

const path = require("path");
const os = require("os");
const child_process = require("child_process");
const glob = require("glob");
const chalk = require("chalk");

const welcomeScreen = `
    |===========================================|
    |========= NuGet Deployment by MIC =========|
    |============= for AppVeyor CI =============|
    |===========================================|
    `;
console.info(chalk.yellow(welcomeScreen));

if (!process.env["appveyor_repo_tag"] || process.env["appveyor_repo_tag"].toLowerCase() !== "true") {
    console.info("APPVEYOR_REPO_TAG is not \"True\". Exiting.");
    process.exit(0);
}

const AlienProjectPattern = /thirdparty|packages/i;

glob(path.join(process.cwd(), "*.nupkg"), packPackages);

function packPackages(err, fileList) {
    if (err) {
        console.error(chalk.red(err));
        process.exit(1);
    }

    console.info(chalk.green("Finding NuGet packages..."));

    const filesToUpload = [];

    for (const fileName of fileList) {
        if (!AlienProjectPattern.test(fileName)) {
            filesToUpload.push(winpath(fileName));
        }
    }

    let uploaded = 0;
    for (const fileName of filesToUpload) {
        console.info(`Uploading ${chalk.magenta(fileName)} ... (${uploaded + 1}/${filesToUpload.length})`);

        const params = [
            "push", fileName,
            "-Source", "https://www.nuget.org/api/v2/package",
            "-ApiKey", process.env["NuGet_API_Key"]
        ];
        const stdout = child_process.execFileSync("nuget", params, {
            cwd: process.cwd(),
            env: process.env
        }, (proc_err) => {
            if (proc_err) {
                console.error(chalk.red(proc_err));
            }
        });
        console.log(chalk.grey(stdout));

        ++uploaded;
    }

    /**
     * @param {string} p 
     */
    function winpath(p) {
        return os.platform() === "win32" ? p.replace(/\//g, "\\") : p;
    }
}
