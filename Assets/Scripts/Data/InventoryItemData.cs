using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class InventoryItemDatabase
{
    public List<InventoryItem> rows;
}

// Load data file and store rows in a dictionary.
public partial class InventoryItemData : IData
{
    public Dictionary<int, InventoryItem> Datas { get; private set; } = null;

    private const string ADDRESS = "InventoryItemData.json";
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
            Debug.LogError("InventoryItemData.json not found");
            return;
        }

        InventoryItemDatabase database = JsonUtility.FromJson<InventoryItemDatabase>(jsonFile.text);

        Datas ??= new();
        Datas.Clear();

        List<InventoryItem> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("InventoryItemData.json data not found");
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
    public InventoryItem GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}
