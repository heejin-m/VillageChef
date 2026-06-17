using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class SaveManager
{
    private const string STARTINFO_FILE_NAME = "STARTINFO_FILE_NAME.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, STARTINFO_FILE_NAME);

    public static void Save(StartInfoSet data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static StartInfoSet Load()
    {
        if (!File.Exists(SavePath))
        {
            return new StartInfoSet();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<StartInfoSet>(json);
            return Normalize(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load save data: {e}");
            return new StartInfoSet();
        }
    }

    private static StartInfoSet Normalize(StartInfoSet data)
    {
        data ??= new StartInfoSet();
        data.playerSaveInfo ??= new PlayerSaveInfo();
        data.recipeSaveInfos ??= new List<RecipeSaveInfo>();
        data.inventoryItemSaveInfo ??= new List<InventoryItemSaveInfo>();
        data.productSaveInfo ??= new List<ProductSaveInfo>();
        return data;
    }

    private static void SaveList<T>(List<T> saveInfos, Func<StartInfoSet, List<T>> getList, Func<T, int> getId)
    {
        ModelCenter.StartInfoSetData ??= new();

        foreach (var info in saveInfos)
        {
            SaveItem(info, getList, getId, false);
        }

        Save(ModelCenter.StartInfoSetData);
    }

    private static void SaveItem<T>(T saveInfo, Func<StartInfoSet, List<T>> getList, Func<T, int> getId, bool isSave = true)
    {
        ModelCenter.StartInfoSetData ??= new();

        var list = getList(ModelCenter.StartInfoSetData);
        bool isExists = list.Exists(d => getId(d) == getId(saveInfo));
        if (!isExists)
        {
            list.Add(saveInfo);
        }

        if (isSave)
        {
            Save(ModelCenter.StartInfoSetData);
        }
    }

    public static void Save(PlayerInfo Info, bool isSave = true)
    {
        ModelCenter.StartInfoSetData ??= new();
        ModelCenter.StartInfoSetData.playerSaveInfo.gold = Info.Gold;

        if (isSave)
        {
            Save(ModelCenter.StartInfoSetData);
        }
    }

    public static void Save(List<RecipeSaveInfo> saveInfos)
    {
        SaveList(saveInfos, data => data.recipeSaveInfos, info => info.id);
    }

    public static void Save(RecipeSaveInfo saveInfo, bool isSave = true)
    {
        SaveItem(saveInfo, data => data.recipeSaveInfos, info => info.id, isSave);
    }

    public static void Save(List<InventoryItemSaveInfo> saveInfos)
    {
        SaveList(saveInfos, data => data.inventoryItemSaveInfo, info => info.id);
    }

    public static void Save(InventoryItemSaveInfo saveInfo, bool isSave = true)
    {
        SaveItem(saveInfo, data => data.inventoryItemSaveInfo, info => info.id, isSave);
    }

    public static void Save(List<ProductSaveInfo> saveInfos)
    {
        SaveList(saveInfos, data => data.productSaveInfo, info => info.id);
    }

    public static void Save(ProductSaveInfo saveInfo, bool isSave = true)
    {
        SaveItem(saveInfo, data => data.productSaveInfo, info => info.id, isSave);
    }
}
