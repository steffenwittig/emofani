using System.Collections.Generic;

/// <summary>
/// Reads and writes settings and saves them to the disc
/// </summary>
public class EmofaniConfig {

    private bool loaded = false;
    private static EmofaniConfig instance;
    private Dictionary<string, string> loadedData;
    private string path = "emofani.config";

    /// <summary>
    /// Private constructor as this is a singleton
    /// </summary>
    private EmofaniConfig()
    {
        loadedData = new Dictionary<string, string>();
    }

    /// <summary>
    /// Returns the instance of the EmofaniConfig class
    /// </summary>
    public static EmofaniConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EmofaniConfig();
            }
            return instance;
        }
    }

    /// <summary>
    /// Load config from file
    /// </summary>
    /// <param name="path"></param>
    private void Load()
    {
        if (System.IO.File.Exists(path)) {
            string[] contents = System.IO.File.ReadAllLines(path);
            foreach (string entry in contents)
            {
                string[] keyValue = entry.Split('=');
                if (keyValue.Length == 2)
                {
                    if (!loadedData.ContainsKey(keyValue[0])) {
                        loadedData.Add(keyValue[0], keyValue[1]);
                    } else
                    {
                        loadedData[keyValue[0]] = keyValue[1];
                    }
                    
                }
            }
        }
    }

    /// <summary>
    /// Save config to file
    /// </summary>
    /// <param name="path"></param>
    public void Save()
    {
        List<string> contents = new List<string>();
        foreach (KeyValuePair<string, string> entry in loadedData)
        {
            // save every element to a new line in this format: key=value
            contents.Add(entry.Key + "=" + entry.Value);
        }
        System.IO.File.WriteAllLines(path, contents.ToArray());
    }

    /// <summary>
    /// Returns a value or, if it is not set, the defaultValue
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetValue(string name, string defaultValue)
    {
        string value = defaultValue;
        if (!loaded)
        {
            this.Load();
        }
        if (this.loadedData.ContainsKey(name))
        {
            value = this.loadedData[name];
        }
        return value;
    }

    public float GetFloat(string name, float defaultValue)
    {
        return float.Parse(this.GetValue(name, defaultValue.ToString()));
    }

    public int GetInt(string name, int defaultValue)
    {
        return int.Parse(this.GetValue(name, defaultValue.ToString()));
    }

    /// <summary>
    /// Sets a value
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetValue(string name, string value)
    {
        this.loadedData[name] = value;
    }

    public void SetInt(string name, int value)
    {
        this.SetValue(name, value.ToString());
    }

    public void SetFloat(string name, float value)
    {
        this.SetValue(name, value.ToString());
    }
}
