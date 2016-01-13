package yaa110.unityplugins.filepicker;

import android.content.Context;
import android.content.Intent;

public class FilePicker {
    public static FilePicker mInstance = null;

    private OpenListener listener;
    private Context context;

    private FilePicker(Context context) {
        this.context = context;
    }

    public static FilePicker getInstance(Context context){
        if(mInstance == null) {
            mInstance = new FilePicker(context);
        }
        return mInstance;
    }

    public void open(String filter, OpenListener listener) {
        this.listener = listener;
        Intent intent = new Intent(context.getApplicationContext(), PickerActivity.class);
        intent.putExtra("filter", filter);
        context.startActivity(intent);
    }

    public void dispatch(String path) {
        listener.onPick(path);
    }
}
