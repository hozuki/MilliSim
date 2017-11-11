"use strict";

const path = require("path");
const fs = require("fs-extra");
const chalk = require("chalk");

const licensesPath = "docs/licenses";

/**
 * @type {{path: string[], prefix: string?}[]}
 */
const licenseFiles = [
    { path: "LICENSE.txt", prefix: "MilliSim" },
    { path: "thirdparty/UnityStudioLib/LICENSE.txt", prefix: "UnityStudioLib" },
    { path: "thirdparty/UnityStudioLib/tracking/UnityStudio/License.md", prefix: "UnityStudio" }
];

const mapping = getMapping(licenseFiles);

let copying = 1;
for (const from in mapping) {
    if (!fs.existsSync(from)) {
        console.warn(chalk.yellow(`Warning: license file '${from}' is not found. Skipping.`));
        continue;
    }

    const to = mapping[from];
    console.info(`Copying ${from} to ${to}... (${copying}/${licenseFiles.length})`);
    const dir = path.dirname(to);
    fs.mkdirsSync(dir);
    fs.copySync(from, to);
    ++copying;
}

/**
 * Get source-destination mapping.
 * @param {{path: string[], prefix: string?}[]} files 
 * @returns {[string]: string}
 */
function getMapping(files) {
    const obj = Object.create(null);
    for (const entry of files) {
        const key = entry.path;
        const fileName = path.basename(key);
        const translatedFileName = entry.prefix ? entry.prefix + "." + fileName : fileName;
        const value = path.join(licensesPath, translatedFileName);
        obj[key] = value;
    }
    return obj;
}
