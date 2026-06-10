using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipeDatabase
{
    public List<Recipe> recipes;
}

// 제이슨 파일을 가져와서 읽고 dictionary 형태의 자료구조로 정리.
public class RecipeData : IData
{
    public Dictionary<ushort, Recipe> Recipes { get; private set; } = new();

    public void Initialize()
    {
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("RecipeData");

        if (jsonFile == null)
        {
            Debug.LogError("RecipeData.json 없음");
            return;
        }

        RecipeDatabase database = JsonUtility.FromJson<RecipeDatabase>(jsonFile.text);

        Recipes.Clear();
        foreach (var recipe in database.recipes)
        {
            if (!Recipes.ContainsKey(recipe.id))
            {
                Recipes.Add(recipe.id, recipe);
            }
        }

        Debug.Log($"{Recipes.Count}개 로드");
    }
}
