using System.Collections.Generic;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveManager
{
    private const int CURRENT_SAVE_VERSION = 1;
    private const string STARTINFO_FILE_NAME = "STARTINFO_FILE_NAME.json";
    private const string CHECKSUM_SALT = "VillageChef_SaveData";

    private static string SavePath => Path.Combine(Application.persistentDataPath, STARTINFO_FILE_NAME);

    [Serializable]
    private class SaveFile
    {
        public string payload;
        public string checksum;
    }

    public static void Save(StartInfoSet data)
    {
        data = Normalize(data);
        data.saveVersion = CURRENT_SAVE_VERSION;

        string payload = JsonUtility.ToJson(data, true);
        SaveFile saveFile = new SaveFile
        {
            payload = payload,
            checksum = CreateChecksum(payload),
        };

        string json = JsonUtility.ToJson(saveFile, true);
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
            SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);
            if (saveFile == null || !IsValidChecksum(saveFile))
            {
                Debug.LogError("Save data checksum mismatch.");
                return new StartInfoSet();
            }

            var data = JsonUtility.FromJson<StartInfoSet>(saveFile.payload);
            data = Migrate(data);
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
        data.inventoryItemSaveInfos ??= new List<InventoryItemSaveInfo>();
        data.productSaveInfos ??= new List<ProductSaveInfo>();
        data.ingredientSupplySaveInfos ??= new List<IngredientSupplySaveInfo>();
        return data;
    }

    private static StartInfoSet Migrate(StartInfoSet data)
    {
        data = Normalize(data);

        if (data.saveVersion >= CURRENT_SAVE_VERSION)
        {
            return data;
        }

        switch (data.saveVersion)
        {
            default:
                data.saveVersion = CURRENT_SAVE_VERSION;
                break;
        }

        return data;
    }

    private static string CreateChecksum(string payload)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes((payload ?? string.Empty) + CHECKSUM_SALT);
            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder(hash.Length * 2);
            foreach (byte value in hash)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }
    }

    private static bool IsValidChecksum(SaveFile saveFile)
    {
        if (string.IsNullOrEmpty(saveFile.payload) || string.IsNullOrEmpty(saveFile.checksum))
        {
            return false;
        }

        string checksum = CreateChecksum(saveFile.payload);
        return string.Equals(checksum, saveFile.checksum, StringComparison.Ordinal);
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
        SaveList(saveInfos, data => data.inventoryItemSaveInfos, info => info.id);
    }

    public static void Save(InventoryItemSaveInfo saveInfo, bool isSave = true)
    {
        SaveItem(saveInfo, data => data.inventoryItemSaveInfos, info => info.id, isSave);
    }

    public static void Save(List<ProductSaveInfo> saveInfos)
    {
        SaveList(saveInfos, data => data.productSaveInfos, info => info.id);
    }

    public static void Save(IngredientSupplySaveInfo saveInfo)
    {
        SaveItem(saveInfo, data => data.ingredientSupplySaveInfos, info => info.id);
    }

    public static void Save(List<IngredientSupplySaveInfo> saveInfos)
    {
        SaveList(saveInfos, data => data.ingredientSupplySaveInfos, info => info.id);
    }
    public static void Save(ProductSaveInfo saveInfo, bool isSave = true)
    {
        SaveItem(saveInfo, data => data.productSaveInfos, info => info.id, isSave);
    }
}
