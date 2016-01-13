package yaa110.unityplugins.notification.db;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

public class DbHelper extends SQLiteOpenHelper {

    public DbHelper(Context context) {
        super(context, "db", null, 4);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        db.execSQL(
                "CREATE TABLE IF NOT EXISTS updates (" +
                        "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "target TEXT, " +
                        "date TEXT" +
                        ")"
        );
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int old_version, int new_version) {
        db.execSQL("DROP TABLE IF EXISTS updates");
        onCreate(db);
    }
}
