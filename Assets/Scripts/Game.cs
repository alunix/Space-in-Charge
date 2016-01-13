// In the name of Allah
// -----------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Component of "Game"
public class Game : MonoBehaviour {
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string ENEMY_HIT_TAG = "EnemyHit";
    public const string BULLET_TAG = "Bullet";
    public const string EXPLOSION_TAG = "Explosion";
    public const string FEATURE_BOX_TAG = "FeatureBox";
    public const string ENEMY_JET_FIRE_TAG = "EnemyJetFire";
    public const string INERT_TAG = "Inert";
    public const string GUARD_TAG = "Guard";
    public const string BLOCKER_TAG = "Blocker";
    public const string LEVEL_TAG = "Level";

    /* FEATURES */
    public const int FEATURE_STATE_LOCK = 0;
    public const int FEATURE_STATE_ACTIVE = 1;
    public const int FEATURE_STATE_DISABLE = 2;

    public const int FEATURE_TYPE_EASY_MODE = 0;
    public const int FEATURE_TYPE_INFINITE_AMMO = 1;
    public const int FEATURE_TYPE_DISCOVER_BOXES = 2;
    public const int FEATURE_TYPE_STUPID_ENEMIES = 3;
    public const int FEATURE_TYPE_TWO_TIMES_POWER = 4;

    /* Preferences: Keys */
    private const string PREF_POINTS = "p1";
    private const string PREF_LEVELS = "p2";
    private const string PREF_CURRENT_LEVEL = "p3";
    private const string PREF_FEATURES = "p6";

    /* Preferences: Default values */
    private const int PREF_POINTS_DEFAULT = 100;
    private const int PREF_CURRENT_LEVEL_DEFAULT = 0;
    private const string PREF_LEVELS_DEFAULT = "1";
    private const string PREF_FEATURES_DEFAULT = "0,0,0,0,0";
    public const float EXPLOSION_POWER = -50;
    public const int LEVEL_CLEAR_POINT = 110;

    public float points = 0;
    private static Game instance = null;

    /* Android Plugins */
    private SecurePreferences prefs;
    #if UNITY_ANDROID
    private AndroidJavaObject activity;
    private Toast toast;
    #endif

    public static Game Instance {
        get {
            return instance;
        }
    }

    public Camera mainCamera;
    public CameraShake cameraShake;

    public float world_left = 0;
    public float world_bottom = 0;
    public float world_width = 0;
    public float world_height = 0;

    /* Resources */
    public GameObject gravityGlow;
    public GameObject menuGlow;
    public GameObject bullet;
    public GameObject playerExplosion;
    public GameObject defaultExplosion;
    public GameObject bulletExplosion;
    public GameObject boxExplosion;
    public GameObject boxCollected;
    public GameObject freezeSmoke;
    public GameObject guardCircle;
    public GameObject inaccurateFiring;
    public GameObject featureTitle;

    /* Layer Masks */
    public int enemyMask;

    /* Scene Controller */
    public static string CURRENT_FEATURE_BOX_NAME = null;
    public static Transform CURRENT_FEATURE_BOX = null;
    public static string COLLECTOR_ENENMY_NAME = null;
    public static List<Enemy> ALL_ENEMIES = new List<Enemy>();
    public static bool ON_BOOTSTRAP = true;
    public static StatusBar STATUS_BAR = null;
    public static LevelController LEVEL_CONTROLLER = null;
    public static ExtraController EXTRA_CONTROLLER = null;
    public static LevelsController LEVELS_CONTROLLER = null;

    /* Player Controller */
    public static float PLAYER_POWER = 5000;
    public const int PLAYER_AMMO = 100;
    public static Player player = null;

    /* Levels Controller */
    public static bool SURVIVAL_MODE = false;
    public static int[] LEVELS_STATUS = new int[24];
    public static int[] FEATURES = new int[5];
    public static int CURRENT_LEVEL = 0;
    public static int GOLDS = 0;

    /* Temp Objects */
    [HideInInspector]
    public GameObject level_pause_btn;

    void Awake() {
        enabled = false;

        if (instance == null) {
            Application.targetFrameRate = 300;
            instance = this;
            setBounds();
            enemyMask = LayerMask.GetMask("Enemy");

            #if UNITY_ANDROID
            activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            toast = new Toast();
            prefs = new SecurePreferences(activity, "resonance.spaceincharge");
            new PushNotification().register(activity, "https://raw.githubusercontent.com/yaa110/Effects-Pro/master/bin/spaceincharge.txt");
            #else
            prefs = new SecurePreferences();
            #endif

            points = prefs.readInt(PREF_POINTS, PREF_POINTS_DEFAULT);
            setLevels();
            setFeatures();
            DontDestroyOnLoad(gameObject);
        } else {
            instance.mainCamera = Camera.main;
            instance.cameraShake = instance.mainCamera.GetComponent<CameraShake>();

            DestroyImmediate(gameObject);
        }
    }

    private void setLevels() {
        GOLDS = 0;
        CURRENT_LEVEL = prefs.readInt(PREF_CURRENT_LEVEL, PREF_CURRENT_LEVEL_DEFAULT);

        string[] levels_string = prefs.readString(PREF_LEVELS, PREF_LEVELS_DEFAULT).Split(',');
        int length = levels_string.Length;

        for (int i = 0; i < length; i++) {
            int level_status = int.Parse(levels_string[i]);
            if (level_status == 0) break;
            LEVELS_STATUS[i] = level_status;
            GOLDS += level_status - 1;
        }
    }

    private void setFeatures() {
        string[] features_string = prefs.readString(PREF_FEATURES, PREF_FEATURES_DEFAULT).Split(',');
        int length = features_string.Length;

        for (int i = 0; i < length; i++) {
            FEATURES[i] = int.Parse(features_string[i]);
        }

        if (FEATURES[FEATURE_TYPE_TWO_TIMES_POWER] == FEATURE_STATE_ACTIVE) {
            PLAYER_POWER = 10000;
        } else {
            PLAYER_POWER = 5000;
        }
    }

    private void setBounds() {
        mainCamera = Camera.main;
        cameraShake = mainCamera.GetComponent<CameraShake>();

        Vector2 bounds = mainCamera.ScreenToWorldPoint(Vector2.zero);

        world_left = bounds.x;
        world_bottom = bounds.y;
        world_width = Mathf.Abs(world_left * 2);
        world_height = Mathf.Abs(world_bottom * 2);
    }

    public void shakeCamera() {
        cameraShake.shake();
    }

    public void stopShakingCamera() {
        cameraShake.stopShake();
    }

    public static void setCollectorEnemy(bool force = false) {
        try {
            if (Game.FEATURES[Game.FEATURE_TYPE_STUPID_ENEMIES] == Game.FEATURE_STATE_ACTIVE) return;

            if (CURRENT_FEATURE_BOX != null || force) {
                float closestDistance = Mathf.Infinity;
                string closestName = null;
                Vector3 box_position = CURRENT_FEATURE_BOX.position;

                int length = Game.ALL_ENEMIES.Count;
                for (int i = 0; i < length; i++) {
                    Enemy e = Game.ALL_ENEMIES[i];
                    float distance = Vector2.Distance(box_position, e._transform.position);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestName = e.name;
                    }
                }

                Game.COLLECTOR_ENENMY_NAME = closestName;
            }
        } catch { }
    }

    public void changePoints(float value) {
        points += value;

        if (points < 0) {
            points = 0;
        }
    }

    public int countGolds() {
        int length = LEVELS_STATUS.Length;
        GOLDS = 0;
        for (int i = 0; i < length; i++) {
            if (LEVELS_STATUS[i] == 0) break;
            GOLDS += LEVELS_STATUS[i] - 1;
        }
        return GOLDS;
    }

    public void showMessage(string message, bool take_long = true) {
        #if UNITY_ANDROID
        toast.show(activity, message, take_long);
        #else
        print(message);
        #endif
    }

    public void savePoints() {
        prefs.write(PREF_POINTS, points.ToString());
    }

    public void ceasefire() {
        if (LEVELS_STATUS[CURRENT_LEVEL] < 2) {
            changePoints(Player.defeatPoints);
            savePoints();
        }
    }

    public void setCurrentLevel(int level) {
        CURRENT_LEVEL = level;
        prefs.write(PREF_CURRENT_LEVEL, level.ToString());
    }

    public void saveFeatures(bool save_points) {
        int length = FEATURES.Length;
        string features_string = "";
        for (int i = 0; i < length; i++) {
            if (i > 0) features_string += ",";

            features_string += FEATURES[i].ToString();
        }

        if (save_points) {
            prefs.writeAll(
                new string[] { PREF_POINTS, PREF_FEATURES },
                new string[] { points.ToString(), features_string }
            );
        } else {
            prefs.write(PREF_FEATURES, features_string);
        }
    }

    public void saveLevel(int level, int value, bool save_points) {
        if (LEVELS_STATUS[level] < value) {
            if (LEVELS_STATUS[level] < 2) {
                points += LEVEL_CLEAR_POINT;
            }

            LEVELS_STATUS[level] = value;
            int length = LEVELS_STATUS.Length;

            if (level < length - 1 && LEVELS_STATUS[level + 1] == 0) LEVELS_STATUS[level + 1] = 1;
            
            string levels_string = "";
            for (int i = 0; i < length; i++) {
                if (LEVELS_STATUS[i] == 0) break;

                if (i > 0) levels_string += ",";

                levels_string += LEVELS_STATUS[i].ToString();
            }

            if (save_points) {
                prefs.writeAll(
                    new string[] {PREF_POINTS, PREF_LEVELS},
                    new string[] {points.ToString(), levels_string}
                );
            } else {
                prefs.write(PREF_LEVELS, levels_string);
            }
        } else if (save_points) {
            savePoints();
        }
    }
}