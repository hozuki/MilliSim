using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor {
    public abstract class ProjectReader {

        /// <summary>
        /// Reads a project and create a <see cref="Project"/> instance.
        /// </summary>
        /// <param name="fileName">Path to project file.</param>
        /// <param name="state">User state object.</param>
        /// <returns>Read project.</returns>
        [NotNull]
        public abstract Project ReadProject([NotNull] string fileName, [CanBeNull] object state);

        public static int CheckFormatVersion([NotNull] string fileName) {
            // major*100+minor
            // 0 = unknown
            var fileInfo = new FileInfo(fileName);

            if (!fileInfo.Exists) {
                return 0;
            }

            var csb = new SQLiteConnectionStringBuilder {
                DataSource = fileInfo.FullName
            };

            object versionObject;

            try {
                using (var db = new SQLiteConnection(csb.ToString())) {
                    db.Open();

                    using (var queryVersion = new SQLiteCommand("SELECT value FROM main WHERE key = @key;", db)) {
                        queryVersion.Parameters.Add("key", DbType.AnsiString).Value = SldprojDbNames.Field_Version;
                        versionObject = queryVersion.ExecuteScalar();

                        if (versionObject == DBNull.Value || versionObject == null) {
                            // v0.2, 'vesion'
                            queryVersion.Parameters["key"].Value = SldprojDbNames.Field_Vesion;
                            versionObject = queryVersion.ExecuteScalar();
                            if (versionObject == DBNull.Value) {
                                versionObject = "0.2";
                            }
                        }
                    }

                    db.Close();
                }
            } catch (SQLiteException ex) {
                Debug.Print(ex.ToString());

                return 0;
            }
            // Record not found
            if (versionObject == null) {
                return 0;
            }

            var versionString = versionObject != DBNull.Value ? (string)versionObject : null;

            if (versionString == null) {
                return 0;
            }

            var fpVersion = double.Parse(versionString);
            int version;

            if (fpVersion < 1) {
                version = (int)(fpVersion * 1000);
            } else {
                version = (int)fpVersion;
            }

            switch (version) {
                case ProjectVersion.V0_2:
                case ProjectVersion.V0_3:
                case ProjectVersion.V0_3_1:
                case ProjectVersion.V0_4:
                    return version;
                default:
                    return 0;
            }
        }

    }
}
