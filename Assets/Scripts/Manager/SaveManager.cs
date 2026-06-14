using System.Collections.Generic;
using System.IO;
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
        data.recipeSaveInfos ??= new List<RecipeSaveInfo>();
        data.inventoryItemSaveInfo ??= new List<InventoryItemSaveInfo>();
        return data;
    }

    public static void Save(List<RecipeSaveInfo> saveInfos)
    {
        ModelCenter.StartInfoSetData ??= new();

        foreach (var info in saveInfos)
        {
            Save(info, false);
        }

        Save(ModelCenter.StartInfoSetData);
    }

    public static void Save(RecipeSaveInfo saveInfo, bool isSave = true)
    {
        ModelCenter.StartInfoSetData ??= new();

        bool isExists = ModelCenter.StartInfoSetData.recipeSaveInfos.Exists(d => d.id == saveInfo.id);
        if (!isExists)
        {
            ModelCenter.StartInfoSetData.recipeSaveInfos.Add(saveInfo);
        }

        if (isSave)
        {
            Save(ModelCenter.StartInfoSetData);
        }
    }

    public static void Save(List<InventoryItemSaveInfo> saveInfos)
    {
        ModelCenter.StartInfoSetData ??= new();

        foreach (var info in saveInfos)
        {
            Save(info, false);
        }

        Save(ModelCenter.StartInfoSetData);
    }

    public static void Save(InventoryItemSaveInfo saveInfo, bool isSave = true)
    {
        ModelCenter.StartInfoSetData ??= new();

        bool isExists = ModelCenter.StartInfoSetData.inventoryItemSaveInfo.Exists(d => d.id == saveInfo.id);
        if (!isExists)
        {
            ModelCenter.StartInfoSetData.inventoryItemSaveInfo.Add(saveInfo);
        }

        if (isSave)
        {
            Save(ModelCenter.StartInfoSetData);
        }
    }
}