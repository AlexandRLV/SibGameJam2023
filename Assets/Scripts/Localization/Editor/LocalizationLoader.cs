using System.Collections.Generic;
using Google;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEditor;
using UnityEngine;
// ReSharper disable StringLiteralTypo

namespace Localization.Editor
{
    public static class LocalizationLoader
    {
        private const string ApiKey = "AIzaSyCZI5Pn9pI1BED70ywfOMSSHR8j2b7ySUo";
        private const string AppName = "MSA-sequrity-agensy";
        private const string SpreadSheetId = "1q2CI3IYGznEP1AKMLLmxrHzF5zo7hGpUODzDzAhCX1I";
        private const string Range = "Main!A2:Z";
        
        [MenuItem("Tools/Load Localization")]
        public static void Load()
        {
            var data = Resources.Load<LocalizationData>("Localization/LocalizationData");
            var config = Resources.Load<LocalizationConfig>("Localization/LocalizationConfig");

            if (data == null)
            {
                Debug.LogError("Нет файла локализации! Создайте файл с локализацией по пути Resources/Localization/LocalizationData");
                return;
            }
            
            if (config == null)
            {
                Debug.LogError("Нет настроек языков локализации! Создайте настройки языков для локализациет по пути Resources/Localization/LocalizationConfig");
                return;
            }
            
            var service = new SheetsService(new BaseClientService.Initializer
            {
                ApiKey = ApiKey,
                ApplicationName = AppName
            });

            var request = service.Spreadsheets.Values.Get(SpreadSheetId, Range);

            ValueRange result;
            try
            {
                result = request.Execute();
            }
            catch (GoogleApiException e)
            {
                Debug.LogError("Не удаётся подключиться к гугл таблице с локализацией!");
                Debug.LogException(e);
                return;
            }

            var rows = result.Values;
            data.data = new List<LocalizationDataContainer>();
            
            foreach (var languageConfig in config.languages)
            {
                if (!TryReadLocalization(rows, languageConfig.columnId, out var localization))
                {
                    Debug.LogError($"Не получилось прочитать локализацию для языка {languageConfig.language}! Проверьте таблицу!");
                    return;
                }
                
                data.data.Add(new LocalizationDataContainer
                {
                    language = languageConfig.language,
                    values = localization,
                });
            }
            
            Debug.Log($"Успешно найдены {data.data.Count} языков!");
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssetIfDirty(data);
        }

        private static bool TryReadLocalization(IList<IList<object>> rows, int columnId, out List<LanguageContainer> localization)
        {
            localization = null;
            if (rows[0].Count <= columnId)
                return false;

            localization = new List<LanguageContainer>();
            var temp = new Dictionary<string, string>();
            foreach (var row in rows)
            {
                string key = row[0].ToString();
                string value = row[columnId].ToString();
                if (!temp.TryAdd(key, value))
                {
                    Debug.LogError(
                        $"В локализации есть повторяющийся ключ: {key}! Уберите повторы, либо измените ключи");
                }
            }

            foreach (var localizationPair in temp)
            {
                localization.Add(new LanguageContainer
                {
                    key = localizationPair.Key,
                    value = localizationPair.Value
                });
            }

            return true;
        }
    }
}