using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class RecipeDatabase
{
    public List<Recipe> rows;
}

// Load data file and store rows in a dictionary.
public partial class RecipeData : IData
{
    public Dictionary<int, Recipe> Datas { get; private set; } = null;

    private const string ADDRESS = "RecipeData.json";
    private AsyncOperationHandle<TextAsset> _handle;

    public async Task Initialize()
    {
        await Load();
    }

    public void Release()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    private async Task Load()
    {
        Release();

        _handle = Addressables.LoadAssetAsync<TextAsset>(ADDRESS);
        TextAsset jsonFile = await _handle.Task;

        if (jsonFile == null)
        {
            Debug.LogError("RecipeData.json not found");
            return;
        }

        RecipeDatabase database = JsonUtility.FromJson<RecipeDatabase>(jsonFile.text);

        Datas ??= new();
        Datas.Clear();

        List<Recipe> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("RecipeData.json data not found");
            return;
        }

        foreach (var row in rows)
        {
            if (!Datas.ContainsKey(row.id))
            {
                Datas.Add(row.id, row);
            }
        }

        Debug.Log($"{Datas.Count} loaded");

        SetDictionaryData();
    }

    /// <summary>
    /// Get data
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns></returns>
    public Recipe GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}
