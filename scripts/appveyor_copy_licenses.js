"use strict";

const path = require("path");
const fs = require("fs-extra");

const licensesPath = "docs/licenses";

/**
 * @type {{path: string[], prefix: string?}[]}
 */
const licenseFiles = [
    { path: "LICENSE.txt", prefix: "MilliSim" },
    { path: "thirdparty/MimeTypeMap/LICENSE", prefix: "MimeTypeMap" },
    { path: "thirdparty/NAudio/license.txt", prefix: "NAudio" },
    { path: "thirdparty/NAudio.Vorbis/LICENSE", prefix: "NAudio.Vorbis" },
    { path: "thirdparty/NAudio.Wma/license.txt", prefix: "NAudio.Wma" },
    { path: "thirdparty/NLayer/LICENSE", prefix: "NLayer" },
    { path: "thirdparty/NVorbis/LICENSE", prefix: "NVorbis" },
    { path: "thirdparty/UnityStudioLib/LICENSE.txt", prefix: "UnityStudioLib" },
    { path: "thirdparty/UnityStudioLib/tracking/UnityStudio/License.md", prefix: "UnityStudio" }
];

const mapping = getMapping(licenseFiles);

let copying = 1;
for (const from in mapping) {
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
