using UnityEngine;
using System.Collections;

public class PushNotification {
    #if UNITY_ANDROID
    private AndroidJavaObject plugin = null;

    public PushNotification() {
        plugin = new AndroidJavaClass("yaa110.unityplugins.notification.Pushify").CallStatic<AndroidJavaObject>("getInstance");
    }

    public void register(AndroidJavaObject activity, string url) {
        plugin.Call("register", activity, url);
    }
    #else
    public PushNotification() {}
    #endif
}
