using UnityEngine;

/// <summary>
/// Класс, хранит начальные настройки.
/// Может быть расширен до загрузки настроек с хмл или загрузки с сервера
/// </summary>
public class ConfigDictionary: MonoBehaviour
{
    private static ConfigDictionary _singleton = null;
    private static ConfigDictionary singleton
    {
        get
        {
            if (_singleton == null)
            {
                GameObject go = new GameObject("ConfigDictionary");
                _singleton = go.AddComponent<ConfigDictionary>();
                _singleton.config = new ConfigDataHolder();
            }
            return _singleton;
        }
    }

    private ConfigDataHolder config;

    public static ConfigDataHolder Config
    {
        get
        {
            return singleton.config;
        }
    }

    
}

