package yaa110.unityplugins.filepicker;

import android.content.Context;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import java.io.File;
import java.util.ArrayList;

public class FileAdapter extends ArrayAdapter<File> {

    public FileAdapter(Context context, ArrayList<File> files) {
        super(context, R.layout.list_item, files);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        ViewHolder holder;

        if (convertView == null) {
            convertView = View.inflate(getContext(),  R.layout.list_item, null);
            holder = new ViewHolder();
            holder.title = (TextView) convertView.findViewById(R.id.title);
            convertView.setTag(holder);
        } else {
            holder = (ViewHolder) convertView.getTag();
        }

        File file = getItem(position);

        holder.title.setText(file.getName());

        if (file.isDirectory()) {
            holder.title.setCompoundDrawablesWithIntrinsicBounds(
                    R.drawable.ic_file_folder,
                    0, 0, 0
            );
        } else {
            holder.title.setCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
        }

        return convertView;
    }

    class ViewHolder {
        public TextView title;
    }
}
