using System.IO;
using System.Text;
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
            Debug.LogError($"폴더 없음: {POPUP_PREFAB_FOLDER}");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { POPUP_PREFAB_FOLDER });

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine();
        sb.AppendLine("public enum ePopup");
        sb.AppendLine("{");

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            string resourcePath = assetPath;

            sb.AppendLine($"    [Description(\"{resourcePath}\")]");
            sb.AppendLine($"    {fileName},");
        }

        sb.AppendLine("}");

        string outputDir = Path.GetDirectoryName(OUTPUT_PATH);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        File.WriteAllText(OUTPUT_PATH, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"PopupEnum 생성 완료: {OUTPUT_PATH}");
    }
}