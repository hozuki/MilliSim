using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization {
    partial class SldprojV4Reader {

        private static class SQLiteHelper {

            public static bool DoesTableExist(SQLiteConnection connection, string tableName) {
                using (var command = connection.CreateCommand()) {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = @tableName;";
                    command.Parameters.Add("tableName", DbType.AnsiString).Value = tableName;
                    var value = command.ExecuteScalar();
                    return value != null;
                }
            }

            public static StringDictionary GetValues(SQLiteConnection connection, string tableName, ref SQLiteCommand command) {
                if (command == null) {
                    command = connection.CreateCommand();
                }
                command.CommandText = $"SELECT key, value FROM {tableName};";
                var result = new StringDictionary();
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var row = reader.GetValues();
                        result.Add(row[0], row[1]);
                    }
                }
                return result;
            }

            public static void ReadNotesTable(SQLiteConnection connection, Difficulty difficulty, DataTable table) {
                using (var command = connection.CreateCommand()) {
                    command.CommandText = $"SELECT * FROM {SldprojDbNames.Table_Notes} WHERE {SldprojDbNames.Column_Difficulty} = @difficulty;";
                    command.Parameters.Add("difficulty", DbType.Int32).Value = (int)difficulty;
                    using (var adapter = new SQLiteDataAdapter(command)) {
                        adapter.Fill(table);
                    }
                }
            }

            public static void ReadBarParamsTable(SQLiteConnection connection, Difficulty difficulty, DataTable dataTable) {
                using (var command = connection.CreateCommand()) {
                    command.CommandText = $"SELECT * FROM {SldprojDbNames.Table_BarParams} WHERE {SldprojDbNames.Column_Difficulty} = @difficulty;";
                    command.Parameters.Add("difficulty", DbType.Int32).Value = (int)difficulty;
                    using (var adapter = new SQLiteDataAdapter(command)) {
                        adapter.Fill(dataTable);
                    }
                }
            }

            public static void ReadSpecialNotesTable(SQLiteConnection connection, Difficulty difficulty, DataTable dataTable) {
                using (var command = connection.CreateCommand()) {
                    command.CommandText = $"SELECT * FROM {SldprojDbNames.Table_SpecialNotes} WHERE {SldprojDbNames.Column_Difficulty} = @difficulty;";
                    command.Parameters.Add("difficulty", DbType.Int32).Value = (int)difficulty;
                    using (var adapter = new SQLiteDataAdapter(command)) {
                        adapter.Fill(dataTable);
                    }
                }
            }

        }

    }
}
