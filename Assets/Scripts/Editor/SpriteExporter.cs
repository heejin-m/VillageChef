using System.IO;
using UnityEditor;
using UnityEngine;

public class SpriteExporter
{
    [MenuItem("Tools/Export Selected Sprites To PNG")]
    public static void ExportSelectedSprites()
    {
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("선택된 스프라이트가 없습니다.");
            return;
        }

        string folderPath = EditorUtility.OpenFolderPanel(
            "PNG 저장할 폴더 선택",
            Application.dataPath,
            ""
        );

        if (string.IsNullOrEmpty(folderPath))
            return;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            foreach (Object asset in assets)
            {
                Sprite sprite = asset as Sprite;
                if (sprite == null)
                    continue;

                Texture2D croppedTexture = GetSpriteTexture(sprite);

                byte[] pngData = croppedTexture.EncodeToPNG();

                string safeName = sprite.name.Replace("/", "_").Replace("\\", "_");
                string savePath = Path.Combine(folderPath, safeName + ".png");

                File.WriteAllBytes(savePath, pngData);

                Object.DestroyImmediate(croppedTexture);
            }
        }

        Debug.Log("스프라이트 PNG 추출 완료!");
    }

    private static Texture2D GetSpriteTexture(Sprite sprite)
    {
        Rect rect = sprite.rect;

        Texture2D sourceTexture = sprite.texture;

        Texture2D newTexture = new Texture2D(
            (int)rect.width,
            (int)rect.height,
            TextureFormat.RGBA32,
            false
        );

        Color[] pixels = sourceTexture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );

        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }
}