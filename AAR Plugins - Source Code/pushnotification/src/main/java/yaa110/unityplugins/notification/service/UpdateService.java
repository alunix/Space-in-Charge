package yaa110.unityplugins.notification.service;

import android.annotation.SuppressLint;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.IBinder;
import android.widget.RemoteViews;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;

import yaa110.unityplugins.notification.Pushify;
import yaa110.unityplugins.notification.R;
import yaa110.unityplugins.notification.db.DbController;

public class UpdateService extends Service {

    public UpdateService() {
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @SuppressLint("NewApi")
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        try {
            getURL(intent.getStringExtra("url"));
        } catch (Exception ignored) {
            stopSelf();
        }

        return START_NOT_STICKY;
    }

    public void getURL(final String url) {
        new Thread() {
            @Override
            public void run() {
                StringBuilder content = new StringBuilder();
                BufferedReader bufferedReader = null;
                try {
                    URLConnection urlConnection = new URL(url).openConnection();
                    bufferedReader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));

                    String line;
                    while ((line = bufferedReader.readLine()) != null) {
                        content.append(line).append("\n");
                    }

                    parseResponse(new JSONObject(content.toString()));
                    stopSelf();
                } catch (Exception e) {
                    stopSelf();
                } finally {
                    if (bufferedReader != null) {
                        try {
                            bufferedReader.close();
                        } catch (IOException ignored) {
                        }
                    }
                    interrupt();
                }
            }
        }.start();
    }

    @SuppressLint("NewApi")
    private void parseResponse(JSONObject response) throws JSONException {
        String target = response.getString("link");

        if (! DbController.getInstance(getApplicationContext()).checkTarget(target)) return;

        /* Set Builder */
        Notification.Builder nBuilder = new Notification.Builder(getApplicationContext())
                .setSmallIcon(R.drawable.ic_status_apps)
                .setAutoCancel(true);

        if (Build.VERSION.SDK_INT > 15) {
            nBuilder.setPriority(Notification.PRIORITY_HIGH);
        }

        Intent extraIntent;
        String extra;

        if (target.startsWith("http")) {
            extraIntent = new Intent(Intent.ACTION_VIEW);
            extraIntent.setData(Uri.parse(target));
            extra = "برای مشاهده کلیک کنید";
        } else {
            if (Pushify.isAppInstalled(getApplicationContext(), target)) return;

            if (Pushify.isAppInstalled(getApplicationContext(), "com.farsitel.bazaar")) {
                extraIntent = new Intent(Intent.ACTION_VIEW);
                extraIntent.setData(Uri.parse("bazaar://details?id=" + target));
                extraIntent.setPackage("com.farsitel.bazaar");
                extra = "برای نصب اپلیکیشن کلیک کنید";
            } else {
                extraIntent = new Intent(Intent.ACTION_VIEW);
                extraIntent.setData(Uri.parse("https://cafebazaar.ir/app/" + target));
                extra = "برای مشاهده اپلیکیشن کلیک کنید";
            }
        }

        PendingIntent pIntent = PendingIntent.getActivity(
                getApplicationContext(),
                0,
                extraIntent,
                PendingIntent.FLAG_UPDATE_CURRENT
        );

        nBuilder.setContentIntent(pIntent);

        /* Change the UI */
        RemoteViews view = new RemoteViews(
                getPackageName(),
                R.layout.notification_small
        );

        view.setTextViewText(R.id.header, response.getString("title").replace("&zwnj;", "\u200C"));
        view.setTextViewText(R.id.value, response.getString("description").replace("&zwnj;", "\u200C").replace("<br>", "\n"));

        view.setImageViewResource(R.id.icon, R.drawable.ic_play);

        nBuilder.setContent(view);
        nBuilder.setDefaults(Notification.DEFAULT_ALL);

        Notification notification = nBuilder.build();

        if (Build.VERSION.SDK_INT > 15) {
            RemoteViews big_view = new RemoteViews(
                    getPackageName(),
                    R.layout.notification
            );

            big_view.setTextViewText(R.id.header, response.getString("title").replace("&zwnj;", "\u200C"));
            big_view.setTextViewText(R.id.value, response.getString("description").replace("&zwnj;", "\u200C").replace("<br>", "\n"));
            big_view.setTextViewText(R.id.extra, extra);
            big_view.setImageViewResource(R.id.icon, R.drawable.ic_play);

            notification.bigContentView = big_view;
        }

        NotificationManager nManager = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
        nManager.notify(
                121,
                notification
        );
    }
}
