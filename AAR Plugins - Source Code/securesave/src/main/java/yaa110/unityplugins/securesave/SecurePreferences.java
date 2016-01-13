package yaa110.unityplugins.securesave;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Build;
import android.util.Base64;

import javax.crypto.Cipher;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.SecretKeySpec;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.KeySpec;
import java.util.UUID;

public class SecurePreferences {
    private static SecurePreferences mInstance = null;
    private SharedPreferences prefs;
    private SecretKeySpec key;
    private String unique_code;

    private SecurePreferences() {}

    public static SecurePreferences getInstance(){
        if(mInstance == null) {
            mInstance = new SecurePreferences();
        }
        return mInstance;
    }

    public void regsiter(final Context context, final String app_name) {
        prefs = context.getSharedPreferences(app_name, Context.MODE_PRIVATE);
        unique_code = generateUniqueID();
        try {
            key = generateKey(unique_code);
        } catch (Exception ignored) {
        }
    }

    public void write(final String name, final String value) {
        new Thread() {
            @Override
            public void run() {
                try {
                    setPriority(MAX_PRIORITY);

                    Cipher cipher = Cipher.getInstance("AES");
                    cipher.init(Cipher.ENCRYPT_MODE, key);
                    byte[] encrypted = cipher.doFinal(value.getBytes("UTF8"));
                    prefs.edit().putString(name, Base64.encodeToString(encrypted, Base64.DEFAULT)).apply();
                } catch(Exception ignored){
                } finally {
                    interrupt();
                }
            }
        }.start();
    }

    public void writeAll(final String[] names, final String[] values) {
        new Thread() {
            @Override
            public void run() {
                try {
                    setPriority(MAX_PRIORITY);

                    int length = names.length;
                    for (int i = 0; i < length; i++) {
                        Cipher cipher = Cipher.getInstance("AES");
                        cipher.init(Cipher.ENCRYPT_MODE, key);
                        byte[] encrypted = cipher.doFinal(values[i].getBytes("UTF8"));
                        prefs.edit().putString(names[i], Base64.encodeToString(encrypted, Base64.DEFAULT)).apply();
                    }
                } catch(Exception ignored){
                } finally {
                    interrupt();
                }
            }
        }.start();

    }

    public void writeBoolean(String name, boolean value) {
        prefs.edit().putBoolean(name, value).apply();
    }

    public String readString(String name, String default_value) {
        String hashed = prefs.getString(name, null);

        if (hashed == null) return default_value;
        try {
            Cipher cipher = Cipher.getInstance("AES");
            cipher.init(Cipher.DECRYPT_MODE, key);
            return new String(cipher.doFinal(Base64.decode(hashed, Base64.DEFAULT)), "UTF8");
        } catch (Exception ignored) {}
        return default_value;
    }

    public int readInt(String name, int default_value) {
        String hashed = prefs.getString(name, null);

        if (hashed == null) return default_value;
        try {
            Cipher cipher = Cipher.getInstance("AES");
            cipher.init(Cipher.DECRYPT_MODE, key);
            return Integer.parseInt(new String(cipher.doFinal(Base64.decode(hashed, Base64.DEFAULT)), "UTF8"));
        } catch (Exception ignored) {}
        return default_value;
    }

    public float readFloat(String name, float default_value) {
        String hashed = prefs.getString(name, null);

        if (hashed == null) return default_value;
        try {
            Cipher cipher = Cipher.getInstance("AES");
            cipher.init(Cipher.DECRYPT_MODE, key);
            return Float.parseFloat(new String(cipher.doFinal(Base64.decode(hashed, Base64.DEFAULT)), "UTF8"));
        } catch (Exception ignored) {}
        return default_value;
    }

    public boolean readBoolean(String name, boolean default_value) {
        return prefs.getBoolean(name, default_value);
    }

    public String getUniqueCode() {
        return unique_code;
    }

    private SecretKeySpec generateKey(String password) throws NoSuchAlgorithmException, InvalidKeySpecException {
        SecretKeyFactory f = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
        KeySpec ks = new PBEKeySpec(
                password.toCharArray(),
                "Namak".getBytes(),
                1024,
                256
        );
        return new SecretKeySpec(new SecretKeySpec(f.generateSecret(ks).getEncoded(), "AES").getEncoded(), "AES");
    }

    @SuppressLint("InlinedApi")
    private int getCpuAbiLength() {
        if (Build.VERSION.SDK_INT > 20) {
            return Build.SUPPORTED_ABIS[0].length();
        } else {
            //noinspection deprecation
            return Build.CPU_ABI.length();
        }
    }

    private String generateUniqueID() {
        @SuppressLint("InlinedApi")
        String m_szDevIDShort = "35" + (Build.BOARD.length() % 10) + (Build.BRAND.length() % 10) + (getCpuAbiLength() % 10) + (Build.DEVICE.length() % 10) + (Build.MANUFACTURER.length() % 10) + (Build.MODEL.length() % 10) + (Build.PRODUCT.length() % 10);

        String serial;
        try {
            serial = android.os.Build.class.getField("SERIAL").get(null).toString();
        } catch (Exception exception) {
            serial = "serial";
        }

        return new UUID(m_szDevIDShort.hashCode(), serial.hashCode()).toString();
    }
}