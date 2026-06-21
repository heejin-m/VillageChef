using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class PopupEnumGenerator
{
    private const string POPUP_PREFAB_FOLDER = "Assets/AddressableAssets/Prefabs/Popup";
    private const string OUTPUT_PATH = "Assets/Scripts/Enum/PopupEnum.cs";

    [MenuItem("Tools/Generate/Popup Enum")]
    public static void Generate()
    {
        if (!Directory.Exists(POPUP_PREFAB_FOLDER))
        {
            Debug.LogError($"Popup prefab folder not found: {POPUP_PREFAB_FOLDER}");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { POPUP_PREFAB_FOLDER });

        if (!File.Exists(OUTPUT_PATH))
        {
            CreatePopupEnumFile(prefabGuids);
            return;
        }

        string enumText = File.ReadAllText(OUTPUT_PATH, Encoding.UTF8);
        HashSet<string> existingPopups = ParseExistingPopupNames(enumText);
        StringBuilder appendBuilder = new StringBuilder();

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            if (existingPopups.Contains(fileName))
                continue;

            appendBuilder.AppendLine($"    [Description(\"{assetPath}\")]");
            appendBuilder.AppendLine($"    {fileName},");
        }

        if (appendBuilder.Length == 0)
        {
            Debug.Log($"PopupEnum already up to date: {OUTPUT_PATH}");
            return;
        }

        int enumEndIndex = enumText.LastIndexOf('}');
        if (enumEndIndex < 0)
        {
            Debug.LogError($"PopupEnum end brace not found: {OUTPUT_PATH}");
            return;
        }

        string updatedText = enumText.Insert(enumEndIndex, appendBuilder.ToString());
        File.WriteAllText(OUTPUT_PATH, updatedText, Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"PopupEnum updated: {OUTPUT_PATH}");
    }

    private static void CreatePopupEnumFile(string[] prefabGuids)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine();
        sb.AppendLine("public enum ePopup");
        sb.AppendLine("{");

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            sb.AppendLine($"    [Description(\"{assetPath}\")]");
            sb.AppendLine($"    {fileName},");
        }

        sb.AppendLine("}");

        string outputDir = Path.GetDirectoryName(OUTPUT_PATH);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        File.WriteAllText(OUTPUT_PATH, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"PopupEnum created: {OUTPUT_PATH}");
    }

    private static HashSet<string> ParseExistingPopupNames(string enumText)
    {
        HashSet<string> popupNames = new HashSet<string>();
        MatchCollection matches = Regex.Matches(
            enumText,
            @"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*(?:=\s*[^,\r\n]+)?\s*,?\s*(?://.*)?$",
            RegexOptions.Multiline);

        foreach (Match match in matches)
        {
            popupNames.Add(match.Groups["name"].Value);
        }

        return popupNames;
    }
}
