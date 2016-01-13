package yaa110.unityplugins.notification.db;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

public class DbController {

    private static DbController mInstance = null;
    private SQLiteOpenHelper helper;
    private SQLiteDatabase db;

    public static DbController getInstance(Context context) {
        if (mInstance == null) {
            mInstance = new DbController(context);
        }
        return mInstance;
    }

    private DbController(Context context) {
        helper = new DbHelper(context);
    }

    public boolean checkTarget(String target) {
        try {
            open();

            Cursor c = db.query(
                    "updates",
                    null,
                    "target = ?",
                    new String[] { target },
                    null, null, null
            );

            if (c.getCount() > 0) {
                c.moveToFirst();

                long date = c.getLong(c.getColumnIndex("date"));

                if (System.currentTimeMillis() < date + 604800000) {
                    c.close();
                    return false;
                } else {
                    ContentValues values = new ContentValues();
                    values.put("date", System.currentTimeMillis() + "");
                    db.update("updates", values, "target = ?", new String[] { target });
                }

            } else {
                ContentValues values = new ContentValues();
                values.put("target", target);
                values.put("date", System.currentTimeMillis() + "");
                db.insert("updates", null, values);
            }

            c.close();

            return true;
        } finally {
            close();
        }
    }

    public void open() {
        db = helper.getWritableDatabase();
    }

    public void  close() {
        helper.close();
    }

}
