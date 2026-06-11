using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSVImporter
{
    [MenuItem("Tools/Import CSV")]
    public static void Import()
    {
        const string csvDirectory = "Assets/CSV";
        const string jsonDirectory = "Assets/AddressableAssets/Json";

        if (!Directory.Exists(csvDirectory))
        {
            Debug.LogError($"CSV 폴더를 찾을 수 없습니다.\n{csvDirectory}");
            return;
        }

        string[] csvPaths = Directory.GetFiles(csvDirectory, "*.csv", SearchOption.TopDirectoryOnly);
        System.Array.Sort(csvPaths);

        if (csvPaths.Length == 0)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다.\n{csvDirectory}");
            return;
        }

        Directory.CreateDirectory(jsonDirectory);
        int totalImportedCount = 0;

        foreach (string csvPath in csvPaths)
        {
            string json = ImportCsv(csvPath, out int importedCount);
            string jsonPath = Path.Combine(jsonDirectory, $"{Path.GetFileNameWithoutExtension(csvPath)}.json");

            File.WriteAllText(jsonPath, json);
            totalImportedCount += importedCount;

            Debug.Log($"{csvPath} -> {jsonPath} : {importedCount}개 Import 완료");
        }

        AssetDatabase.Refresh();

        Debug.Log($"{csvPaths.Length}개 CSV 파일에서 {totalImportedCount}개 Import 완료");
    }

    private static string ImportCsv(string csvPath, out int importedCount)
    {
        string[] lines = File.ReadAllLines(csvPath);
        importedCount = 0;

        if (lines.Length == 0)
        {
            Debug.LogWarning($"{csvPath} 파일이 비어 있습니다.");
            return CreateEmptyJson();
        }

        List<string> headers = ParseCsvLine(lines[0]);
        if (headers.Count == 0)
        {
            Debug.LogWarning($"{csvPath} 헤더가 비어 있습니다.");
            return CreateEmptyJson();
        }

        headers[0] = headers[0].TrimStart('\uFEFF');
        StringBuilder json = new();
        json.AppendLine("{");
        json.AppendLine("    \"rows\": [");
        bool hasImportedRow = false;

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            List<string> values = ParseCsvLine(lines[i]);

            if (values.Count != headers.Count)
            {
                Debug.LogWarning($"{csvPath} {i + 1}번째 줄 컬럼 수가 헤더와 다릅니다. header:{headers.Count}, value:{values.Count}");
                continue;
            }

            if (hasImportedRow)
                json.AppendLine(",");

            json.AppendLine("        {");

            for (int j = 0; j < headers.Count; j++)
            {
                json.Append("            ");
                json.Append(ToJsonString(headers[j]));
                json.Append(": ");
                json.Append(ToJsonValue(values[j]));

                if (j < headers.Count - 1)
                    json.Append(",");

                json.AppendLine();
            }

            json.Append("        }");
            importedCount++;
            hasImportedRow = true;
        }

        json.AppendLine();
        json.AppendLine("    ]");
        json.AppendLine("}");
        return json.ToString();
    }

    private static string CreateEmptyJson()
    {
        return "{\n    \"rows\": []\n}\n";
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> values = new();
        StringBuilder value = new();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char current = line[i];

            if (current == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    value.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (current == ',' && !inQuotes)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else
            {
                value.Append(current);
            }
        }

        values.Add(value.ToString());
        return values;
    }

    private static string ToJsonValue(string value)
    {
        string trimmedValue = value.Trim();

        if (string.IsNullOrEmpty(trimmedValue))
            return "\"\"";

        if (bool.TryParse(trimmedValue, out bool boolValue))
            return boolValue ? "true" : "false";

        if (long.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            return trimmedValue;

        if (double.TryParse(trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            return trimmedValue;

        return ToJsonString(value);
    }

    private static string ToJsonString(string value)
    {
        StringBuilder builder = new();
        builder.Append('"');

        foreach (char current in value)
        {
            switch (current)
            {
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    builder.Append(current);
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }
}
