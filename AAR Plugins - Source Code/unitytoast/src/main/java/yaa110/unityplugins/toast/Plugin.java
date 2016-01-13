package yaa110.unityplugins.toast;

import android.content.Context;
import android.widget.Toast;

public class Plugin {

    public static void showMessage(Context context, String text, boolean take_long) {
        Toast.makeText(
                context.getApplicationContext(),
                text,
                take_long ? Toast.LENGTH_LONG : Toast.LENGTH_SHORT
        ).show();
    }

}
