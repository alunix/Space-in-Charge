using UnityEngine;
using System.Collections;

public class Toast {
    #if UNITY_ANDROID
    private AndroidJavaClass plugin = null;

    public Toast() {
        plugin = new AndroidJavaClass("yaa110.unityplugins.toast.Plugin");
    }

    public void show(AndroidJavaObject activity, string message, bool take_long = true) {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.CallStatic("showMessage", activity, message, take_long);
        }));
    }
    #else
    public Toast() { }
    #endif
}
