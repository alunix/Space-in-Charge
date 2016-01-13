using UnityEngine;
using System.Collections;

public class SecurePreferences {
    #if UNITY_ANDROID
    private AndroidJavaObject plugin = null;
    

    public SecurePreferences(AndroidJavaObject activity, string app_name) {
        plugin = new AndroidJavaClass("yaa110.unityplugins.securesave.SecurePreferences").CallStatic<AndroidJavaObject>("getInstance");
        plugin.Call("regsiter", activity, app_name);
    }
    #else
    public SecurePreferences() {}
    #endif

    public void write(string name, string value) {
        #if UNITY_ANDROID
        plugin.Call("write", name, value);
        #else
        PlayerPrefs.SetString(name, value);        
        #endif
    }

    public void writeAll(string[] names, string[] values) {
        #if UNITY_ANDROID
        plugin.Call("writeAll", names, values);
        #else
        for (int i = 0; i < names.Length; i++) {
            PlayerPrefs.SetString(names[i], values[i]);
        }    
        #endif
    }

    public void writeBoolean(string name, bool value) {
        #if UNITY_ANDROID
        plugin.Call("writeBoolean", name, value);
        #else
        PlayerPrefs.SetInt(name, value ? 1 : 0);
        #endif
    }

    public string readString(string name, string default_value) {
        #if UNITY_ANDROID
        return plugin.Call<string>("readString", name, default_value);
        #else
        try {
            string value = PlayerPrefs.GetString(name);
            return (value.Length == 0 ? default_value : value);
        } catch {
            return default_value;
        }
        #endif
    }

    public int readInt(string name, int default_value) {
        #if UNITY_ANDROID
        return plugin.Call<int>("readInt", name, default_value);
        #else
        try {
            return int.Parse(PlayerPrefs.GetString(name));
        } catch {
            return default_value;
        }
        #endif
    }

    public float readFloat(string name, float default_value) {
        #if UNITY_ANDROID
        return plugin.Call<float>("readFloat", name, default_value);
        #else
        try {
            return float.Parse(PlayerPrefs.GetString(name));
        } catch {
            return default_value;
        }
        #endif
    }

    public bool readBoolean(string name, bool default_value) {
        #if UNITY_ANDROID
        return plugin.Call<bool>("readBoolean", name, default_value);
        #else
        try {
            return (PlayerPrefs.GetInt(name) == 1 ? true : false);
        } catch {
            return default_value;
        }
        #endif
    }

    public string getUniqueCode() {
        #if UNITY_ANDROID
        return plugin.Call<string>("getUniqueCode");
        #else
        return "Only-Works-On-Android";
        #endif
    }
}
