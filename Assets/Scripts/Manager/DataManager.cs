using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : SingletonBehaviour<DataManager>
{
    private readonly Dictionary<Type, IData> dataMap = new();

    public async Task Initialize()
    {
        await AddData<RecipeData>();
        await AddData<IngredientData>();
        await AddData<InventoryItemData>();
    }

    private async Task AddData<T>() where T : IData, new()
    {
        T data = new();
        await data.Initialize();
        dataMap.Add(typeof(T), data);
    }

    public T GetData<T>() where T : class, IData
    {
        if (dataMap.TryGetValue(typeof(T), out IData data))
        {
            return data as T;
        }

        Debug.LogError($"{typeof(T).Name} data is not loaded.");
        return null;
    }
}
