using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class IngredientsSupplyDatabase
{
    public List<IngredientsSupply> rows;
}

// Load data file and store rows in a dictionary.
public partial class IngredientsSupplyData : IData
{
    public Dictionary<int, IngredientsSupply> Datas { get; private set; } = null;

    private const string ADDRESS = "IngredientsSupplyData.json";
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
            Debug.LogError("IngredientsSupplyData.json not found");
            return;
        }

        IngredientsSupplyDatabase database = JsonUtility.FromJson<IngredientsSupplyDatabase>(jsonFile.text);

        Datas ??= new();
        Datas.Clear();

        List<IngredientsSupply> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("IngredientsSupplyData.json data not found");
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
    public IngredientsSupply GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}
