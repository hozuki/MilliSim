"use strict";

const fs = require("fs");
const path = require("path");
const mkdirp = require("mkdirp");

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

for (const from in mapping) {
    const to = mapping[from];
    console.info(`Copying ${from} to ${to}...`);
    const dir = path.dirname(to);
    mkdirp.sync(dir);
    copyFileAsync(from, to, err => {
        if (err) {
            console.error(err);
            process.exit(1);
        }
    });
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

// https://stackoverflow.com/a/14387791
/**
 * @param {string} source 
 * @param {string} target 
 * @param {(err: Error) => void} [cb] 
 */
function copyFileAsync(source, target, cb) {
    let cbCalled = false;

    const rd = fs.createReadStream(source);
    rd.on("error", function (err) {
        done(err);
    });
    const wr = fs.createWriteStream(target);
    wr.on("error", function (err) {
        done(err);
    });
    wr.on("close", function (ex) {
        done();
    });
    rd.pipe(wr);

    function done(err) {
        if (rd) {
            rd.destroy();
        }
        if (wr) {
            wr.destroy();
        }
        if (!cbCalled && cb instanceof Function) {
            cb(err);
            cbCalled = true;
        }
    }
}
