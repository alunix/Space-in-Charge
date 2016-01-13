package yaa110.unityplugins.notification;

import android.annotation.SuppressLint;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.SystemClock;

import yaa110.unityplugins.notification.service.UpdateService;

public class Pushify {
    private static Pushify mInstance = null;

    private Pushify() {}

    public static Pushify getInstance(){
        if(mInstance == null) {
            mInstance = new Pushify();
        }
        return mInstance;
    }

    @SuppressLint("NewApi")
    public void register(Context context, String url) {
        AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);

        Intent intent = new Intent(context.getApplicationContext(), UpdateService.class);
        intent.putExtra("url", url);

        PendingIntent pintent = PendingIntent.getService(
                context.getApplicationContext(),
                0,
                intent,
                PendingIntent.FLAG_UPDATE_CURRENT
        );

        alarmManager.cancel(pintent);

        alarmManager.setInexactRepeating(
                AlarmManager.ELAPSED_REALTIME,
                SystemClock.elapsedRealtime(),
                AlarmManager.INTERVAL_HOUR,
                pintent
        );
    }

    public static boolean isAppInstalled(Context context, String packageName) {
        try {
            context.getPackageManager().getApplicationInfo(packageName, 0);
            return true;
        }
        catch (PackageManager.NameNotFoundException e) {
            return false;
        }
    }
}
