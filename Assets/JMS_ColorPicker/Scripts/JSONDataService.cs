using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

//JSON for IDataService for interface ex
public class JsonDataService : IDataService
{
    public bool SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath + RelativePath;

        try
        {
            if (File.Exists(path))
            {
                //data exists, deleting old file and writing a new one
                File.Delete(path);
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data, error: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            T data;
            data = JsonConvert.DeserializeObject<T>("");
            return data;
        }
        else
        {
            try
            {
                T data;
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data, error: {e.Message} {e.StackTrace}");
                throw e;
            }
        }
    }
}