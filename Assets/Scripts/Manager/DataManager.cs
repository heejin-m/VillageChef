using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class DataManager : SingletonBehaviour<DataManager>
{
    private readonly Dictionary<Type, IData> dataMap = new();

    public void Initialize()
    {
        GenerateData();
    }

    private void GenerateData()
    {
        AddData<RecipeData>();
    }

    private void AddData<T>() where T : IData, new()
    {
        T data = new();
        data.Initialize();
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
