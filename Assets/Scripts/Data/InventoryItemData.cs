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

// 제이슨 파일을 가져와서 읽고 dictionary 형태의 자료구조로 정리.
public partial class InventoryItemData : IData
{
    public Dictionary<int, InventoryItem> Datas { get; private set; } = new();

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
            Debug.LogError("InventoryItem.json 없음");
            return;
        }

        InventoryItemDatabase database = JsonUtility.FromJson<InventoryItemDatabase>(jsonFile.text);

        Datas.Clear();

        List<InventoryItem> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("InventoryItem.json 데이터 없음");
            return;
        }

        foreach (var row in rows)
        {
            if (!Datas.ContainsKey(row.id))
            {
                Datas.Add(row.id, row);
            }
        }

        Debug.Log($"{Datas.Count}개 로드");
    }
}
