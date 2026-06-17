using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class ProductDatabase
{
    public List<Product> rows;
}

// 제이슨 파일을 가져와서 읽고 dictionary 형태의 자료구조로 정리.
public partial class ProductData : IData
{
    public Dictionary<int, Product> Datas { get; private set; } = null;

    private const string ADDRESS = "ProductData.json";
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
            Debug.LogError("ProductData.json 없음");
            return;
        }

        ProductDatabase database = JsonUtility.FromJson<ProductDatabase>(jsonFile.text);

        Datas ??= new();
        Datas.Clear();

        List<Product> rows = database.rows;

        if (rows == null)
        {
            Debug.LogError("ProductData.json 데이터 없음");
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

        SetDictionaryData();
    }
}
