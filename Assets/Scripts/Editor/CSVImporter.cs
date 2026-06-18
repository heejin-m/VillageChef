using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSVImporter
{
    private const string CsvDirectory = "Assets/CSV";
    private const string JsonDirectory = "Assets/AddressableAssets/Json";

    [MenuItem("Tools/Import CSV")]
    public static void Import()
    {
        if (!Directory.Exists(CsvDirectory))
        {
            Debug.LogError($"CSV 폴더를 찾을 수 없습니다.\n{CsvDirectory}");
            return;
        }

        string[] csvPaths = Directory.GetFiles(CsvDirectory, "*.csv", SearchOption.TopDirectoryOnly);
        Array.Sort(csvPaths);

        if (csvPaths.Length == 0)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다.\n{CsvDirectory}");
            return;
        }

        Directory.CreateDirectory(JsonDirectory);
        int totalImportedCount = 0;
        int failedCount = 0;

        foreach (string csvPath in csvPaths)
        {
            if (!TryImportCsv(csvPath, out string json, out int importedCount))
            {
                failedCount++;
                continue;
            }

            string jsonPath = Path.Combine(JsonDirectory, $"{Path.GetFileNameWithoutExtension(csvPath)}.json");

            File.WriteAllText(jsonPath, json, Encoding.UTF8);
            totalImportedCount += importedCount;

            Debug.Log($"{csvPath} -> {jsonPath} : {importedCount}개 Import 완료");
        }

        AssetDatabase.Refresh();

        if (failedCount > 0)
        {
            Debug.LogError($"{failedCount}개 CSV Import 실패. schema 파일과 타입을 확인해주세요.");
            return;
        }

        Debug.Log($"{csvPaths.Length}개 CSV 파일에서 {totalImportedCount}개 Import 완료");
    }

    private static bool TryImportCsv(string csvPath, out string json, out int importedCount)
    {
        json = CreateEmptyJson();
        importedCount = 0;

        string schemaPath = GetSchemaPath(csvPath);
        if (!File.Exists(schemaPath))
        {
            Debug.LogError($"schema 파일을 찾을 수 없습니다. CSV마다 schema가 필요합니다.\nCSV: {csvPath}\nschema: {schemaPath}");
            return false;
        }

        if (!DataSchemaUtility.TryLoad(schemaPath, out DataSchema schema))
        {
            return false;
        }

        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length == 0)
        {
            Debug.LogWarning($"{csvPath} 파일이 비어 있습니다.");
            return true;
        }

        List<string> headers = ParseCsvLine(lines[0]);
        if (headers.Count == 0)
        {
            Debug.LogWarning($"{csvPath} 헤더가 비어 있습니다.");
            return true;
        }

        headers[0] = headers[0].TrimStart('\uFEFF');
        Dictionary<string, int> headerIndexes = CreateHeaderIndexes(headers);

        if (!ValidateHeaders(csvPath, schema, headerIndexes))
        {
            return false;
        }

        StringBuilder builder = new();
        builder.AppendLine("{");
        builder.AppendLine("    \"rows\": [");
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

            if (!TryAppendRow(builder, csvPath, i + 1, values, schema, headerIndexes, hasImportedRow))
            {
                return false;
            }

            importedCount++;
            hasImportedRow = true;
        }

        builder.AppendLine();
        builder.AppendLine("    ]");
        builder.AppendLine("}");

        json = builder.ToString();
        return true;
    }

    private static bool TryAppendRow(
        StringBuilder builder,
        string csvPath,
        int lineNumber,
        List<string> values,
        DataSchema schema,
        Dictionary<string, int> headerIndexes,
        bool appendComma)
    {
        if (appendComma)
            builder.AppendLine(",");

        builder.AppendLine("        {");

        for (int i = 0; i < schema.fields.Count; i++)
        {
            DataSchemaField field = schema.fields[i];
            string csvName = field.GetCsvName();
            string value = values[headerIndexes[csvName]];

            if (!TryConvertToJsonValue(value, field.type, out string jsonValue))
            {
                Debug.LogError($"{csvPath} {lineNumber}번째 줄 '{csvName}' 값을 '{field.type}' 타입으로 변환할 수 없습니다. value: {value}");
                return false;
            }

            builder.Append("            ");
            builder.Append(ToJsonString(field.name));
            builder.Append(": ");
            builder.Append(jsonValue);

            if (i < schema.fields.Count - 1)
                builder.Append(",");

            builder.AppendLine();
        }

        builder.Append("        }");
        return true;
    }

    private static bool ValidateHeaders(string csvPath, DataSchema schema, Dictionary<string, int> headerIndexes)
    {
        foreach (DataSchemaField field in schema.fields)
        {
            string csvName = field.GetCsvName();
            if (!headerIndexes.ContainsKey(csvName))
            {
                Debug.LogError($"{csvPath}에서 schema 컬럼을 찾을 수 없습니다. field: {field.name}, csvName: {csvName}");
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, int> CreateHeaderIndexes(List<string> headers)
    {
        Dictionary<string, int> indexes = new();

        for (int i = 0; i < headers.Count; i++)
        {
            string header = headers[i].Trim();
            if (!indexes.ContainsKey(header))
            {
                indexes.Add(header, i);
            }
        }

        return indexes;
    }

    private static string GetSchemaPath(string csvPath)
    {
        string directory = Path.GetDirectoryName(csvPath);
        string fileName = Path.GetFileNameWithoutExtension(csvPath);
        return Path.Combine(directory, $"{fileName}.schema.json");
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

    private static bool TryConvertToJsonValue(string value, string type, out string jsonValue)
    {
        string trimmedValue = value.Trim();

        switch (DataSchemaUtility.NormalizeType(type))
        {
            case "string":
                jsonValue = ToJsonString(value);
                return true;
            case "bool":
                if (TryParseBool(trimmedValue, out bool boolValue))
                {
                    jsonValue = boolValue ? "true" : "false";
                    return true;
                }
                break;
            case "byte":
                if (byte.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte byteValue))
                {
                    jsonValue = byteValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "sbyte":
                if (sbyte.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte sbyteValue))
                {
                    jsonValue = sbyteValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "short":
                if (short.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out short shortValue))
                {
                    jsonValue = shortValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "ushort":
                if (ushort.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort ushortValue))
                {
                    jsonValue = ushortValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "int":
                if (int.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                {
                    jsonValue = intValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "uint":
                if (uint.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint uintValue))
                {
                    jsonValue = uintValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "long":
                if (long.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue))
                {
                    jsonValue = longValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "ulong":
                if (ulong.TryParse(trimmedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong ulongValue))
                {
                    jsonValue = ulongValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "float":
                if (float.TryParse(trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                {
                    jsonValue = floatValue.ToString("R", CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            case "double":
                if (double.TryParse(trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                {
                    jsonValue = doubleValue.ToString("R", CultureInfo.InvariantCulture);
                    return true;
                }
                break;
            default:
                Debug.LogError($"지원하지 않는 schema 타입입니다. type: {type}");
                break;
        }

        jsonValue = null;
        return false;
    }

    private static bool TryParseBool(string value, out bool result)
    {
        if (bool.TryParse(value, out result))
            return true;

        if (value == "1")
        {
            result = true;
            return true;
        }

        if (value == "0")
        {
            result = false;
            return true;
        }

        return false;
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
