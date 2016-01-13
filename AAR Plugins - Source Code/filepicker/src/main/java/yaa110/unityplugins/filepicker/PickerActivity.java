package yaa110.unityplugins.filepicker;

import android.app.Activity;
import android.os.Bundle;
import android.os.Environment;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;

import java.io.File;
import java.io.FileFilter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Locale;

public class PickerActivity extends Activity {

    private ArrayList<File> files;
    private FileAdapter adapter;
    private String root;
    private File current_file;
    private View back_btn;
    private TextView back_txt;
    private String filter_extension;

    private FileFilter filter = new FileFilter() {
        @Override
        public boolean accept(File pathname) {
            return filter_extension.equals("*") || pathname.isDirectory() || pathname.getName().toLowerCase(Locale.ENGLISH).endsWith(filter_extension);
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_picker);

        files = new ArrayList<>();
        root = Environment.getExternalStorageDirectory().getAbsolutePath();
        ListView list = (ListView) findViewById(R.id.list);
        back_btn = findViewById(R.id.back_btn);
        back_txt = (TextView) findViewById(R.id.back_path);

        adapter = new FileAdapter(getApplicationContext(), files);

        try {
            filter_extension = getIntent().getStringExtra("filter");
        } catch (Exception ignored) {
        }

        list.setAdapter(adapter);

        list.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                File file = files.get(position);

                if (file.isDirectory()) {
                    loadFiles(file);
                } else {
                    FilePicker.mInstance.dispatch(file.getAbsolutePath());
                    finish();
                }
            }
        });

        loadFiles(Environment.getExternalStorageDirectory());
    }

    private void loadFiles(File file) {
        current_file = file;
        files.clear();

        if (root.equals(current_file.getAbsolutePath())) {
            back_btn.setVisibility(View.GONE);
        } else {
            back_txt.setText(current_file.getParentFile().getAbsolutePath());
            back_btn.setVisibility(View.VISIBLE);
        }

        File[] file_names = file.listFiles(filter);
        Collections.addAll(files, file_names);

        adapter.notifyDataSetChanged();
    }

    @Override
    public void onBackPressed() {
        if (root.equals(current_file.getAbsolutePath())) {
            finish();
        } else {
            loadFiles(current_file.getParentFile());
        }
    }

    public void onClickBack(View view) {
        onBackPressed();
    }
}
