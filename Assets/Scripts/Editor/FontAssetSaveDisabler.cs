using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 참고
/// https://discussions.unity.com/t/tmpro-dynamic-font-asset-constantly-changes-in-source-control/868941
/// </summary>
[CustomEditor(typeof(TMP_FontAsset))]
public class FontAssetSaveDisabler : TMP_FontAssetEditor
{
    private static bool FontAssetsLocked
    {
        // 폰트 에셋이 잠겨 있는지를 나타내는 정적 속성
        // 기본값은 true => 아트쪽 제외한 나머지가 SDF.asset 올리는것 방지.
        // 아트 쪽(UI)에는 해당 Lock 해제하고 수정하면 된다고 공유함 

        get => EditorPrefs.GetBool("TMPFontsLocked", true);
        set => EditorPrefs.SetBool("TMPFontsLocked", value);
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button(FontAssetsLocked ? "Unlock Font Assets" : "Lock Font Assets"))
        {
            FontAssetsLocked = !FontAssetsLocked;
            GUIUtility.ExitGUI();
        }

        if (!FontAssetsLocked)
        {
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }

    private class SaveHandler : AssetModificationProcessor
    {
        /// <summary>
        /// 에셋이 저장되기 전에 호출
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static string[] OnWillSaveAssets(string[] paths)
        {
            // 잠긴 상태이면 에셋 경로만 남기고 나머지는 제거
            if (FontAssetsLocked)
            {
                int index = 0;
                foreach (string path in paths)
                {
                    if (!typeof(TMP_FontAsset).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(path)))
                        paths[index++] = path;
                }

                Array.Resize(ref paths, index);
            }

            return paths;
        }
    }
}