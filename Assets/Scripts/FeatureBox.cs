using UnityEngine;
using System.Collections;

// Component of "Box"
public class FeatureBox : MonoBehaviour {

    public const int TYPE_FREEZE_PLAYER = 1;
    public const int TYPE_FREEZE_ENEMY = 2;
    public const int TYPE_NO_FIRE_PLAYER = 3;
    public const int TYPE_NO_FIRE_ENEMY = 4;
    public const int TYPE_AMMO_LASER = 5;
    public const int TYPE_AMMO_CLUSTER = 6;
    public const int TYPE_MAD_ENEMY = 7;
    public const int TYPE_POWER_REDUCE = 8;
    public const int TYPE_AMMO_AUTO = 9;
    public const int TYPE_EXPLODE_BOX = 10;
    public const int TYPE_INCREASE_ENEMY_SPEED = 11;
    public const int TYPE_HIDDEN_ENEMY = 12;
    public const int TYPE_GUARD = 13;
    public const int TYPE_INCREASE_POINTS = 14;
    public const int TYPE_REDUCE_POINTS = 15;
    public const int TYPE_EXPLODE_ENEMIES = 16;
    public const int TYPE_INACCURATE_FIRE = 17;
    public const int TYPE_INACCURATE_ENEMY = 18;
    public const int TYPE_MIRROR_FIRE = 19;

    public const int TYPE_INCREASE_AMMO = 100;

    public const int AMMO_AUTO_COUNTER = 50;
    public const int AMMO_CLUSTER_COUNTER = 5;
    public const int AMMO_LASER_COUNTER = 5;
    public const int AMMO_ROCKET_COUNTER = 5;
    public const float AMMO_CLUSTER_POWER = 1000;
    public const float AMMO_LASER_POWER = 1000;
    public const float AMMO_ROCKET_POWER = 1000;
    public const int FREEZE_TIME = 5; // seconds
    public const int MAD_TIME = 5; // seconds
    public const int HIDDEN_TIME = 5; // seconds
    public const int NO_FIRE_TIME = 5; // seconds
    public const int INACCURATE_FIRE_TIME = 10; // seconds
    public const int MIRROR_FIRE_TIME = 10; // seconds
    public const int GUARD_TIME = 5; // seconds
    public const int SPEED_UP_TIME = 5; // seconds
    public const float POINTS = 100;
    public const float REDUCE_POWER = -100;

    private string[] TITLES = new string[19] {
        "Self: Freezing",
        "Enemy: Freezing",
        "Self: No Gun",
        "Enemy: No Gun",
        "Wave Gun",
        "Flame Gun",
        "Crazy Enemy",
        REDUCE_POWER + " Power",
        "Auto Firing",
        "Trap",
        "Speed Up Enemy",
        "Hidden Enemy",
        "Guard",
        "+" + POINTS + " Pts",
        "-" + (POINTS / 2) + " Pts",
        "Enemy Explosion",
        "Self: Inaccurate Firing",
        "Enemy: Inaccurate Firing",
        "Inverse Firing"
    };

    private bool isExploaded = false;
    private int powerUpValue = 0;
    private float explosion_timer_def;

    private float explosionTimer = 15;
    private Transform _transform;

    [Header("General Settings")]
    public bool isPowerUp = false;
    public bool isAmmo = false;
    public int type = 0;

    [Header("Object Settings")]
    public Transform timerBar;

    void Awake() {
        name = System.DateTime.Now.Ticks.ToString();
        explosion_timer_def = explosionTimer;

        if (isPowerUp) {
            powerUpValue = Random.Range(8, 15) * 100;
        } else if (isAmmo) {
            type = TYPE_INCREASE_AMMO;
        } else {
            type = Random.Range(1, 20);
        }
    }

	void Start () {
        _transform = transform;

        if (Game.CURRENT_FEATURE_BOX_NAME == null) {
            Game.CURRENT_FEATURE_BOX_NAME = name;
            Game.CURRENT_FEATURE_BOX = _transform;
            StartCoroutine(setCollector());
        }

        if (!isAmmo && !isPowerUp && Game.FEATURES[Game.FEATURE_TYPE_DISCOVER_BOXES] == Game.FEATURE_STATE_ACTIVE) {
            displayTitle();
        }
	}

    IEnumerator setCollector() {
        yield return new WaitForSeconds(1.5f);
        Game.setCollectorEnemy(true);
    }
	
	void Update () {
        if (LevelController.LEVEL_PAUSED) return;

        if (explosionTimer <= 0) {
            explode();
            enabled = false;
        } else {
            explosionTimer -= Time.deltaTime;
            timerBar.localScale = new Vector3(explosionTimer / explosion_timer_def, 1, 1);
        }
	}

    public void explode(bool collected = false) {
        StopAllCoroutines();
        if (!isExploaded) {
            isExploaded = true;
            if (name.Equals(Game.CURRENT_FEATURE_BOX_NAME)) {
                Game.CURRENT_FEATURE_BOX = null;
                Game.COLLECTOR_ENENMY_NAME = null;
                Game.CURRENT_FEATURE_BOX_NAME = null;
            }
            if (!isAmmo && !isPowerUp && collected) {
                displayTitle();
            }
            Instantiate(collected ? Game.Instance.boxCollected : Game.Instance.boxExplosion, _transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(_transform.parent.gameObject);
        }
    }

    private void displayTitle() {
        ((GameObject)Instantiate(Game.Instance.featureTitle, _transform.position, Quaternion.Euler(Vector3.zero))).GetComponent<FeatureTitle>().displayTitle(TITLES[type - 1]);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag.Equals(Game.PLAYER_TAG)) {
            if (isPowerUp) {
                Game.player.changePower(powerUpValue);
            } else {
                switch (type) {
                    case TYPE_AMMO_AUTO:
                        Game.player.autoFire();
                        break;
                    case TYPE_AMMO_CLUSTER:
                        Game.player.ammo_cluster_counter = AMMO_CLUSTER_COUNTER;
                        break;
                    case TYPE_AMMO_LASER:
                        Game.player.ammo_laser_counter = AMMO_LASER_COUNTER;
                        break;
                    case TYPE_MAD_ENEMY:
                        int length = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length; i++) {
                            Game.ALL_ENEMIES[i].mad();
                        }
                        break;
                    case TYPE_EXPLODE_ENEMIES:
                        int length7 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length7; i++) {
                            Game.ALL_ENEMIES[i].explode();
                        }
                        break;
                    case TYPE_EXPLODE_BOX:
                        Game.player.changePower(Game.EXPLOSION_POWER);
                        explode(false);
                        return;
                    case TYPE_FREEZE_ENEMY:
                        int length2 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length2; i++) {
                            Game.ALL_ENEMIES[i].freeze();
                        }
                        break;
                    case TYPE_FREEZE_PLAYER:
                        Game.player.freeze();
                        break;
                    case TYPE_GUARD:
                        Game.player.guard();
                        break;
                    case TYPE_HIDDEN_ENEMY:
                        int length3 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length3; i++) {
                            Game.ALL_ENEMIES[i].hide();
                        }
                        break;
                    case TYPE_INCREASE_ENEMY_SPEED:
                        int length4 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length4; i++) {
                            Game.ALL_ENEMIES[i].speedUp();
                        }
                        break;
                    case TYPE_INCREASE_POINTS:
                        Game.Instance.changePoints(POINTS);
                        break;
                    case TYPE_NO_FIRE_ENEMY:
                        int length5 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length5; i++) {
                            Game.ALL_ENEMIES[i].noFire();
                        }
                        break;
                    case TYPE_NO_FIRE_PLAYER:
                        Game.player.noFire();
                        break;
                    case TYPE_POWER_REDUCE:
                        Game.player.changePower(REDUCE_POWER);
                        break;
                    case TYPE_REDUCE_POINTS:
                        Game.Instance.changePoints(-POINTS / 2);
                        break;
                    case TYPE_INACCURATE_FIRE:
                        Game.player.inaccurate();
                        break;
                    case TYPE_MIRROR_FIRE:
                        Game.player.mirrorFiring();
                        break;
                    case TYPE_INACCURATE_ENEMY:
                        int length6 = Game.ALL_ENEMIES.Count;
                        for (int i = 0; i < length6; i++) {
                            Game.ALL_ENEMIES[i].inaccurate();
                        }
                        break;
                    case TYPE_INCREASE_AMMO:
                        Game.player.ammoAmount = Mathf.Min(Game.player.ammoAmount + 30, Game.PLAYER_AMMO);
                        Game.STATUS_BAR.changeAmmo(Game.player.ammoAmount);
                        break;
                }
            }
            explode(true);
        } else if (collider.tag.Equals(Game.ENEMY_HIT_TAG)) {
            Enemy enemy = collider.transform.parent.GetComponent<Enemy>();
            if (enemy.isCollector) {
                if (isPowerUp) {
                    enemy.changePower(powerUpValue);
                } else {
                    switch (type) {
                        case TYPE_AMMO_CLUSTER:
                            enemy.ammo_cluster_counter = AMMO_CLUSTER_COUNTER;
                            break;
                        case TYPE_AMMO_LASER:
                            enemy.ammo_laser_counter = AMMO_LASER_COUNTER;
                            break;
                        case TYPE_MAD_ENEMY:
                            enemy.mad();
                            break;
                        case TYPE_EXPLODE_ENEMIES:
                            if (Game.player != null)
                                Game.player.explode();
                            break;
                        case TYPE_EXPLODE_BOX:
                            enemy.changePower(Game.EXPLOSION_POWER);
                            explode(false);
                            return;
                        case TYPE_FREEZE_ENEMY:
                            if (Game.player != null)
                                Game.player.freeze();
                            break;
                        case TYPE_FREEZE_PLAYER:
                            enemy.freeze();
                            break;
                        case TYPE_GUARD:
                            enemy.guard();
                            break;
                        case TYPE_HIDDEN_ENEMY:
                            enemy.hide();
                            break;
                        case TYPE_MIRROR_FIRE:
                            if (Game.player != null)
                                Game.player.mirrorFiring();
                            break;
                        case TYPE_INCREASE_ENEMY_SPEED:
                            enemy.speedUp();
                            break;
                        case TYPE_NO_FIRE_ENEMY:
                            if (Game.player != null)
                                Game.player.noFire();
                            break;
                        case TYPE_NO_FIRE_PLAYER:
                            enemy.noFire();
                            break;
                        case TYPE_POWER_REDUCE:
                            enemy.changePower(REDUCE_POWER);
                            break;
                        case TYPE_INACCURATE_FIRE:
                            enemy.inaccurate();
                            break;
                        case TYPE_INACCURATE_ENEMY:
                            if (Game.player != null)
                                Game.player.inaccurate();
                            break;
                    }
                }
                explode(true);
            } else {
                if (!enemy.isDuplicator) enemy.explode();
                explode();
            }
        }
    }

    void OnDestroy() {
        StopAllCoroutines();
    }

}