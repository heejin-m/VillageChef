using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class RecipeDatabase
{
    public List<Recipe> recipes;
}

// 제이슨 파일을 가져와서 읽고 dictionary 형태의 자료구조로 정리.
public class RecipeData : IData
{
    public Dictionary<ushort, Recipe> Recipes { get; private set; } = new();

    private const string ADDRESS = "RecipeData.json";
    private AsyncOperationHandle<TextAsset> _handle;

    public async Task Initialize()
    {
        await LoadRecipes();
    }

    public void Release()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    private async Task LoadRecipes()
    {
        Release();

        _handle = Addressables.LoadAssetAsync<TextAsset>(ADDRESS);
        TextAsset jsonFile = await _handle.Task;

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
