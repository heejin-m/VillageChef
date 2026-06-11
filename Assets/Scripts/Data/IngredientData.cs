using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class IngredientDatabase
{
    public List<Ingredient> rows;
}

// 제이슨 파일을 가져와서 읽고 dictionary 형태의 자료구조로 정리.
public class IngredientData : IData
{
    public Dictionary<ushort, Ingredient> Ingredients { get; private set; } = new();

    private const string ADDRESS = "IngedientData.json";
    private AsyncOperationHandle<TextAsset> _handle;

    public async Task Initialize()
    {
        await LoadR();
    }

    public void Release()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    private async Task LoadR()
    {
        Release();

        _handle = Addressables.LoadAssetAsync<TextAsset>(ADDRESS);
        TextAsset jsonFile = await _handle.Task;

        if (jsonFile == null)
        {
            Debug.LogError("Ingredient.json 없음");
            return;
        }

        IngredientDatabase database = JsonUtility.FromJson<IngredientDatabase>(jsonFile.text);

        Ingredients.Clear();

        List<Ingredient> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("Ingredient.json 데이터 없음");
            return;
        }

        foreach (var recipe in rows)
        {
            if (!Ingredients.ContainsKey(recipe.id))
            {
                Ingredients.Add(recipe.id, recipe);
            }
        }

        Debug.Log($"{Ingredients.Count}개 로드");
    }
}
