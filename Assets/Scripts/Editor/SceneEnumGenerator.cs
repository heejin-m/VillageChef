using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SceneEnumGenerator : MonoBehaviour
{
    private const string POPUP_PREFAB_FOLDER = "Assets/AddressableAssets/Scenes";
    private const string OUTPUT_PATH = "Assets/Scripts/Enum/SceneEnum.cs";

    [MenuItem("Tools/Generate/Scene Enum")]
    public static void Generate()
    {
        if (!Directory.Exists(POPUP_PREFAB_FOLDER))
        {
            Debug.LogError($"폴더 없음: {POPUP_PREFAB_FOLDER}");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Scene", new[] { POPUP_PREFAB_FOLDER });

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine();
        sb.AppendLine("public class SceneEnum");
        sb.AppendLine("{");
        sb.AppendLine("    public enum eScene");
        sb.AppendLine("    {");

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            string resourcePath = assetPath
                            .Replace("Assets/AddressableAssets/Scenes/", "");

            sb.AppendLine($"        [Description(\"{resourcePath}\")]");
            sb.AppendLine($"        {fileName},");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        string outputDir = Path.GetDirectoryName(OUTPUT_PATH);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        File.WriteAllText(OUTPUT_PATH, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"PopupEnum 생성 완료: {OUTPUT_PATH}");
    }
}