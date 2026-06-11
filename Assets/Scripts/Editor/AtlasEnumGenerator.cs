using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AtlasEnumGenerator
{
    private const string ATLAS_PREFAB_FOLDER = "Assets/AddressableAssets/Atlas";
    private const string OUTPUT_PATH = "Assets/Scripts/Enum/AtlasEnum.cs";
    private const string ATLAS_EXTENSION = ".spriteatlasv2";

    [MenuItem("Tools/Generate/Atlas Enum")]
    public static void Generate()
    {
        if (!Directory.Exists(ATLAS_PREFAB_FOLDER))
        {
            Debug.LogError($"폴더 없음: {ATLAS_PREFAB_FOLDER}");
            return;
        }

        string[] atlasPaths = Directory.GetFiles(ATLAS_PREFAB_FOLDER, $"*{ATLAS_EXTENSION}", SearchOption.TopDirectoryOnly);
        System.Array.Sort(atlasPaths);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine();
        sb.AppendLine("public class AtlasEnum");
        sb.AppendLine("{");
        sb.AppendLine("    public enum eAtlas");
        sb.AppendLine("    {");

        foreach (string assetPath in atlasPaths)
        {
            string fileName = SanitizeEnumName(Path.GetFileNameWithoutExtension(assetPath));

            string resourcePath = assetPath.Replace("\\", "/");

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

        Debug.Log($"AtlasEnum 생성 완료: {OUTPUT_PATH}");
    }

    private static string SanitizeEnumName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "_";

        StringBuilder sb = new StringBuilder();

        if (!char.IsLetter(value[0]) && value[0] != '_')
            sb.Append('_');

        foreach (char c in value)
        {
            sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        }

        return sb.ToString();
    }
}
