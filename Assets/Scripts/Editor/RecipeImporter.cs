using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RecipeImporter
{
    [MenuItem("Tools/Import Recipe CSV")]
    public static void Import()
    {
        string csvPath = "Assets/Datas/RecipeData.csv";

        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다.\n{csvPath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);

        List<Recipe> recipes = new();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            if (values.Length < 3)
            {
                Debug.LogWarning($"{i + 1}번째 줄 데이터가 부족합니다.");
                continue;
            }

            Recipe recipe = new()
            {
                id = ushort.Parse(values[0]),
                name = values[1],
                description = values[2]
            };

            recipes.Add(recipe);
        }

        RecipeDatabase database = new()
        {
            recipes = recipes
        };

        string json = JsonUtility.ToJson(database, true);

        Directory.CreateDirectory("Assets/Resources");

        File.WriteAllText(
            "Assets/Resources/RecipeData.json",
            json);

        AssetDatabase.Refresh();

        Debug.Log($"{recipes.Count}개 Import 완료");
    }
}