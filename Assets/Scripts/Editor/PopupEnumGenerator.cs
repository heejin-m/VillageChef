using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class PopupEnumGenerator
{
    private const string PopupPrefabFolder = "Assets/Resources/Prefabs/Popup";
    private const string OutputPath = "Assets/Scripts/Enum/PopupEnum.cs";

    [MenuItem("Tools/Generate/Popup Enum")]
    public static void Generate()
    {
        if (!Directory.Exists(PopupPrefabFolder))
        {
            Debug.LogError($"폴더 없음: {PopupPrefabFolder}");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PopupPrefabFolder });

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine();
        sb.AppendLine("public class PopupEnum");
        sb.AppendLine("{");
        sb.AppendLine("    public enum ePopup");
        sb.AppendLine("    {");

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            string resourcePath = assetPath
                .Replace("Assets/Resources/", "")
                .Replace(".prefab", "");

            sb.AppendLine($"        [Description(\"{resourcePath}\")]");
            sb.AppendLine($"        {fileName},");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        string outputDir = Path.GetDirectoryName(OutputPath);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        File.WriteAllText(OutputPath, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"PopupEnum 생성 완료: {OutputPath}");
    }
}