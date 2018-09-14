using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.TheaterDays.Configuration;
using OpenMLTD.TheaterDays.Globalization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMLTD.TheaterDays.Subsystems.Globalization {
    internal static class CultureSpecificInfoHelper {

        [NotNull]
        internal static CultureSpecificInfo CreateCultureSpecificInfo() {
            var info = new TheaterDaysCultureSpecificInfo(CultureInfo.CurrentUICulture);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            var globalizationConfigFileInfo = new FileInfo(GlobalizationConfigFile);
            GlobalizationConfig config;

            using (var fileStream = File.Open(globalizationConfigFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream)) {
                    config = deserializer.Deserialize<GlobalizationConfig>(reader);
                }
            }

            var globalizationConfigBaseDirectory = globalizationConfigFileInfo.Directory;

            if (globalizationConfigBaseDirectory == null) {
                throw new ApplicationException("Unexpected: base directory for globalization files is null!");
            }

            foreach (var translationFileGlob in config.TranslationFiles) {
                var translationManager = info.TranslationManager as TheaterDaysTranslationManager;

                if (translationManager == null) {
                    throw new InvalidCastException();
                }

                translationManager.AddTranslationsFromGlob(globalizationConfigBaseDirectory, translationFileGlob);
            }

            return info;
        }

        private static readonly string GlobalizationConfigFile = "Contents/globalization.yml";

    }
}
