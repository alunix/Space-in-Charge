using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    [Header("Main Settings")]
    public string model = "Meteor";
    public float power = 300;
    [Range(0, 50)]
    public float speed = 15;
    [Range(0, 10)]
    [Tooltip("It works when 'is Looking' behavior is enabled and forces the enemy to stay far from the player.")]
    public float safeDistance;
    public int defeatPoints = 20;
    public bool demoMode = false;

    [Header("Object Settings")]
    public GameObject health;
    public Transform healthBar;
    public GameObject hitObject;
    public GameObject blocker;
    public Transform rightGun;
    public Transform leftGun;
    public bool hasHolder = false;

    [Header("Firing Settings")]
    [Tooltip("A fire interval of 10 equals a rate of 1 bullet per second. Lower numbers make a higher firing rate. In order to disable firing, use the value of zero.")]
    public int fireInterval = 10;
    public float bulletPower = 100;
    [Range(0, 10)]
    public float bulletSpeed = 5;
    public bool doubleGun = false;
    public bool allowFiring = true;

    [Header("Behavior Settings")]
    [Tooltip("If enabled, the enemy would follow the player.")]
    public bool isLooking = false;
    [Tooltip("If enabled, the enemy would be reproduced after explosion.")]
    public bool isReproducible = true;
    [Tooltip("If enabled, the scale and rotation of enemy would be changed before moving to scene.")]
    public bool isChangeable = false;
    [Tooltip("If enabled, the enemy would be duplicated on destorying by bullet. They would be destroyed after some seconds.")]
    public bool isDuplicator = false;
    [Tooltip("If enabled, the enemy could get feature boxes.")]
    public bool isCollector = false;

    [Header("Changeable Settings")]
    [Range(1, 9)]
    [Tooltip("This is applied when 'Change On Reproduce' is enabled")]
    public int minScale = 5;
    [Tooltip("This is applied when 'Change On Reproduce' is enabled")]
    [Range(0, 90)]
    public int maxRotation = 90;
    public bool changeSpeed = true;

    [Header("Duplicate Settings")]
    public float destroySeconds = 20;

    [HideInInspector]
    public Rigidbody2D _rigidBody;
    [HideInInspector]
    public Transform _transform;
    private bool effectiveInvisibility = true;
    private SpriteRenderer _renderer;
    private float invisibleTimer = 1;
    private bool isExploaded = false;
    [HideInInspector]
    public bool temp_allow_firing = true;
    private int fire_interval_def;
    private bool is_hidden = false;
    private bool is_frozen = false;
    private bool is_guarded = false;
    private bool accurate_fire = true;
    private bool is_mad = false;

    /* Default fields */
    private float power_def;
    private float destroy_time_def;
    private float safe_distance_def;
    private float speed_def;

    private const int POSITION_TOP = 0;
    private const int POSITION_RIGHT = 1;
    private const int POSITION_BOTTOM = 2;
    private const int POSITION_LEFT = 3;
    private const float SAFE_BORDER_DISTANCE = 0.5f;

    /* Feature Box */
    [HideInInspector]
    public int ammo_cluster_counter = 0;
    [HideInInspector]
    public int ammo_laser_counter = 0;

	// Use this for initialization
	void Start () {
        if (!isLooking) {
            move(false);
        }

        if (fireInterval > 0) {
            StartCoroutine(fire());
        }
	}

    void Awake() {
        name = System.DateTime.Now.Ticks.ToString();
        setDefaults();
        _renderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _transform = transform;
        if (isCollector) {
            Game.ALL_ENEMIES.Add(this);
        }
    }
    
    void LateUpdate() {
        if (LevelController.LEVEL_PAUSED) return;

        if (isLooking && Game.player != null) {
            Vector3 targetPosition;
            float deltaTime = Time.deltaTime;

            if (Game.CURRENT_FEATURE_BOX != null && name.Equals(Game.COLLECTOR_ENENMY_NAME)) {
                targetPosition = Game.CURRENT_FEATURE_BOX.position;
                if (safeDistance != 0) {
                    safeDistance = 0;
                    temp_allow_firing = allowFiring;
                    allowFiring = false;
                    collectBox();
                }
            } else {
                targetPosition = Game.player._transform.position;
                if (safeDistance == 0 && isCollector) {
                    safeDistance = safe_distance_def;
                    allowFiring = temp_allow_firing;
                    removeForces();
                }
            }

            float deltaSpeed = deltaTime * speed / 10;

            // Look At Player
            Vector3 diff = targetPosition - _transform.position;
            float alpha = Mathf.Atan2(diff.y, diff.x);

            _transform.rotation = Quaternion.Lerp(
                _transform.rotation,
                Quaternion.Euler(0, 0, (alpha * Mathf.Rad2Deg) - 90),
                deltaTime * 10
            );

            if (is_frozen) {
                return;
            }

            if (safeDistance == 0 && isCollector) {
                if (!_renderer.isVisible) {
                    if (invisibleTimer <= 0) {
                        collectBox(70);
                    } else {
                        invisibleTimer -= deltaTime;
                    }
                }
                return;
            }

            // Follow Player or Boxes
            Vector2 position = new Vector2();

            position.x = Mathf.Lerp(_transform.position.x, Mathf.Clamp((is_mad ? 0 : -safeDistance * Mathf.Cos(alpha)) + targetPosition.x, Game.Instance.world_left + SAFE_BORDER_DISTANCE, -SAFE_BORDER_DISTANCE - Game.Instance.world_left), deltaSpeed);
            position.y = Mathf.Lerp(_transform.position.y, Mathf.Clamp((is_mad ? 0 : -safeDistance * Mathf.Sin(alpha)) + targetPosition.y, Game.Instance.world_bottom + SAFE_BORDER_DISTANCE, -SAFE_BORDER_DISTANCE - Game.Instance.world_bottom), deltaSpeed);

            _transform.position = position;

            if (isDuplicator && targetPosition.x < position.x) {
                _transform.localScale = new Vector3(-1, 1, 1);
            } else if (isDuplicator) {
                _transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
	
	void Update () {
        if (LevelController.LEVEL_PAUSED) return;

        float deltaTime = Time.deltaTime;

        if (!isLooking && !_renderer.isVisible) {
            if (invisibleTimer <= 0) {
                onReproduce();
            } else {
                invisibleTimer -= deltaTime;
            }
        }

        if (isDuplicator) {
            destroySeconds -= deltaTime;
            healthBar.localScale = new Vector3(destroySeconds / destroy_time_def, 1, 1);
            if (destroySeconds <= 0) explode();
        }
	}

    private void setDefaults() {
        power_def = power;
        destroy_time_def = destroySeconds;
        safe_distance_def = safeDistance;
        temp_allow_firing = allowFiring;
        speed_def = speed;
        fire_interval_def = fireInterval;
    }

    IEnumerator fire() {
        yield return new WaitForSeconds(1.5f);
        while (true) {
            if (allowFiring && Game.player != null && !LevelController.LEVEL_PAUSED) {
                if (Game.FEATURES[Game.FEATURE_TYPE_STUPID_ENEMIES] == Game.FEATURE_STATE_ACTIVE) {
                    fireToTarget();
                } else {
                    RaycastHit2D hit = Physics2D.Raycast(Game.player._transform.position, _transform.position - Game.player._transform.position, Mathf.Infinity, Game.Instance.enemyMask);
                    if (hit.collider.transform.parent.name.Equals(name)) {
                        fireToTarget();
                    }
                }
            }

            yield return new WaitForSeconds(fireInterval / 10f);
        }
    }

    private void fireToTarget() {
        if (ammo_cluster_counter > 0) {
            ammo_cluster_counter--;
            GameObject cluster = (GameObject)Instantiate(Resources.Load("Other/Bullet_Cluster"), _transform.position, Quaternion.Euler(Vector2.zero));
            cluster.GetComponent<Bullet>().fire(hitObject, Game.player._transform.position, bulletSpeed, FeatureBox.AMMO_CLUSTER_POWER, true, 1);
        } else if (ammo_laser_counter > 0) {
            ammo_laser_counter--;
            GameObject laser = (GameObject)Instantiate(Resources.Load("Other/Bullet_Laser"), _transform.position, Quaternion.Euler(Vector2.zero));
            laser.GetComponent<Bullet>().fire(hitObject, Game.player._transform.position, bulletSpeed, FeatureBox.AMMO_LASER_POWER, true, 1, true);
        } else {
            if (doubleGun) {
                GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, rightGun.position, Quaternion.Euler(Vector2.zero));
                GameObject bullet2 = (GameObject)Instantiate(Game.Instance.bullet, leftGun.position, Quaternion.Euler(Vector2.zero));

                if (is_hidden) {
                    bullet.GetComponent<TrailRenderer>().enabled = false;
                    bullet2.GetComponent<TrailRenderer>().enabled = false;
                }

                bullet.GetComponent<Bullet>().fire(hitObject, Game.player._transform.position, bulletSpeed, bulletPower, accurate_fire);
                bullet2.GetComponent<Bullet>().fire(hitObject, Game.player._transform.position, bulletSpeed, bulletPower, accurate_fire);
            } else {
                GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, _transform.position, Quaternion.Euler(Vector2.zero));

                if (is_hidden) {
                    bullet.GetComponent<TrailRenderer>().enabled = false;
                }

                bullet.GetComponent<Bullet>().fire(hitObject, Game.player._transform.position, bulletSpeed, bulletPower, accurate_fire);
            }
        }
    }

    private void move(bool reset_position = true) {
        if (isChangeable) {
            float scale = Random.Range(minScale, 11) / 10f;
            if (hasHolder) {
                _transform.parent.localScale = new Vector2(scale, scale);
                if (maxRotation > 0) {
                    _transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, maxRotation)));
                }
            } else {
                _transform.localScale = new Vector2(scale, scale);
                if (maxRotation > 0) {
                    _transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, maxRotation)));
                }
            }
        }

        if (reset_position) {
            Vector2 position = new Vector2();
            switch (Random.Range(0, 4)) {
                case POSITION_TOP:
                    position.y = 1 - Game.Instance.world_bottom;
                    position.x = Random.Range(Game.Instance.world_left, -Game.Instance.world_left);
                    break;
                case POSITION_RIGHT:
                    position.x = 1 - Game.Instance.world_left;
                    position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                    break;
                case POSITION_BOTTOM:
                    position.y = Game.Instance.world_bottom - 1;
                    position.x = Random.Range(Game.Instance.world_left, -Game.Instance.world_left);
                    break;
                case POSITION_LEFT:
                    position.x = Game.Instance.world_left - 1;
                    position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                    break;
            }

            _transform.position = position;
        }

        try {
            if (speed > 0) {
                Vector2 playerPosition;

                if (demoMode) {
                    playerPosition = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                } else {
                    playerPosition = Game.player._transform.position;
                }

                if (playerPosition.x < Game.Instance.world_left || playerPosition.x > -Game.Instance.world_left) {
                    playerPosition.x = 0;
                }
                if (playerPosition.y < Game.Instance.world_bottom || playerPosition.y > -Game.Instance.world_bottom) {
                    playerPosition.y = 0;
                }

                Vector2 delta = playerPosition - (Vector2) _transform.position;
                int velocity_coefficient = 100;

                float max_delta = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

                if (max_delta < velocity_coefficient) {
                    delta *= velocity_coefficient / max_delta;
                }

                _rigidBody.AddForce(delta * (changeSpeed ? Random.Range(speed, speed + 5) : speed));
            }
        } catch { }
    }

    void OnBecameInvisible() {
        if (!isLooking) {
            if (effectiveInvisibility) {
                onReproduce();
            } else {
                effectiveInvisibility = true;
            }
        }
    }

    private void removeForces() {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.angularVelocity = 0;
    }

    public void collectBox(int multiplier = 40) {
        invisibleTimer = 1;
        _rigidBody.drag = 0;
        removeForces();
        _rigidBody.AddForce((Game.CURRENT_FEATURE_BOX.position - _transform.position) * multiplier);
    }

    private void onReproduce() {
        if (isReproducible) {
            removeForces();
            power = power_def;
            invisibleTimer = 1;
            isExploaded = false;
            move();
        } else {
            if (!hasHolder) {
                Destroy(gameObject);
            } else {
                Destroy(_transform.parent.gameObject);
            }
        }
    }

    public void changePower(float value, bool by_bullet = false) {
        if (isExploaded) return;
        if (is_guarded && value < 0) return;


        if (isDuplicator && by_bullet) {
            destroySeconds = destroy_time_def;
            Instantiate(gameObject, _transform.position + new Vector3(Random.Range(5, 20) / 10f, Random.Range(5, 20) / 10f, 0), Quaternion.Euler(Vector3.zero));
        } else if (!isDuplicator) {
            if (power == power_def && health != null && !is_hidden) {
                health.SetActive(true);
            }

            power += value;
            power = Mathf.Clamp(power, 0, power_def);

            if (healthBar != null) {
                healthBar.localScale = new Vector3(power / power_def, 1, 1);
            }

            if (power <= 0) {
                explode(true);
            }
        }
    }

    public void explode(bool give_points = false, bool by_hit = false) {
        if (is_guarded) return;
        if (isCollector) Game.LEVEL_CONTROLLER.enemyDefeated();

        if (!isDuplicator || !by_hit) {
            if (give_points) Game.Instance.changePoints(defeatPoints);
            isExploaded = true;
            Instantiate(Game.Instance.defaultExplosion, _transform.position, Quaternion.Euler(Vector3.zero));
            effectiveInvisibility = false;
            onReproduce();
        }
    }

    public void freeze() {
        is_frozen = true;
        GameObject freeze_smoke = (GameObject)Instantiate(Game.Instance.freezeSmoke, _transform.position, Quaternion.Euler(Vector3.zero));
        freeze_smoke.transform.parent = _transform;
        StartCoroutine(unfreeze());
    }

    IEnumerator unfreeze() {
        yield return new WaitForSeconds(FeatureBox.FREEZE_TIME);
        is_frozen = false;
    }

    public void hide() {
        is_hidden = true;
        _renderer.enabled = false;
        Instantiate(Game.Instance.boxCollected, _transform.position, Quaternion.Euler(Vector3.zero));
        foreach (Transform child in _transform) {
            if (child.tag.Equals(Game.ENEMY_JET_FIRE_TAG)) child.gameObject.SetActive(false);
        }
        health.SetActive(false);
        StartCoroutine(unhide());
    }

    IEnumerator unhide() {
        yield return new WaitForSeconds(FeatureBox.HIDDEN_TIME);
        Instantiate(Game.Instance.boxCollected, _transform.position, Quaternion.Euler(Vector3.zero));
        _renderer.enabled = true;
        foreach (Transform child in _transform) {
            if (child.tag.Equals(Game.ENEMY_JET_FIRE_TAG)) child.gameObject.SetActive(true);
        }
        if (power < power_def && health != null) {
            health.SetActive(true);
        }
        is_hidden = false;
    }

    public void noFire() {
        temp_allow_firing = false;
        allowFiring = false;
        StartCoroutine(stopNoFire());
    }

    IEnumerator stopNoFire() {
        yield return new WaitForSeconds(FeatureBox.NO_FIRE_TIME);
        if (!name.Equals(Game.COLLECTOR_ENENMY_NAME)) {
            temp_allow_firing = true;
            allowFiring = true;
        }
    } 

    public void speedUp() {
        speed *= 2;
        fireInterval /= 2;
        StartCoroutine(stopSpeedUp());
    }

    IEnumerator stopSpeedUp() {
        yield return new WaitForSeconds(FeatureBox.SPEED_UP_TIME);
        speed = speed_def;
        fireInterval = fire_interval_def;
    }

    public void guard() {
        is_guarded = true;
        GameObject guard_circle = (GameObject)Instantiate(Game.Instance.guardCircle, _transform.position, Quaternion.Euler(Vector3.zero));
        guard_circle.transform.parent = _transform;
        guard_circle.name = hitObject.name + Game.GUARD_TAG;
        StartCoroutine(stopGuard());
    }

    IEnumerator stopGuard() {
        yield return new WaitForSeconds(FeatureBox.GUARD_TIME);
        is_guarded = false;
    }

    public void inaccurate() {
        GameObject inaccurate_fire = (GameObject)Instantiate(Game.Instance.inaccurateFiring, _transform.position, Quaternion.Euler(Vector3.zero));
        inaccurate_fire.transform.parent = _transform;
        accurate_fire = false;
        StartCoroutine(accurate());
    }

    IEnumerator accurate() {
        yield return new WaitForSeconds(FeatureBox.INACCURATE_FIRE_TIME);
        accurate_fire = true;
    }

    public void mad() {
        is_mad = true;
        speed *= 2.5f;
        if (blocker != null) blocker.SetActive(false);
        StartCoroutine(stopMad());
    }

    IEnumerator stopMad() {
        yield return new WaitForSeconds(FeatureBox.MAD_TIME);
        is_mad = false;
        speed = speed_def;
        if (blocker != null) blocker.SetActive(true);
    }

    void OnDestroy() {
        StopAllCoroutines();
        if (isCollector) {
            Game.ALL_ENEMIES.Remove(this);
            if (name.Equals(Game.COLLECTOR_ENENMY_NAME)) {
                Game.setCollectorEnemy();
            }
        }
    }
}