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

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<StartInfoSet>(json);
    }
}