using System.IO;
using UnityEngine;

public class SaveManager
{
    private const string FILE_NAME = "save.json";

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, FILE_NAME);

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

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<StartInfoSet>(json);
    }
}