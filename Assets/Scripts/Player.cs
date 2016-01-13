using UnityEngine;
using System.Collections;

// Component of "Player"
public class Player : MonoBehaviour {
    [Header("General Settings")]
    public float power;
    public int pushForce = 200;
    public int velocityCoefficient = 10;
    public static float defeatPoints = -30;

    [Header("Ammunition Settings")]
    public int ammoAmount = 100;
    public float bulletPower = 100;
    [Range(1, 10)]
    public float bulletSpeed = 5;

    private Rigidbody2D _rigidBody;
    private CircleCollider2D circleCollider;
    [HideInInspector]    
    public Transform _transform;
    private float playerZ;
    private Vector3 lastPosition;
    private Vector2 playerSize;
    private bool isDraggingAlowed = true;
    private bool mirrorWalls = false;
    private TrailRenderer _trailRenderer;
    private float trail_time;
    private bool clickOnSelf = false;
    private bool isExploaded = false;
    private bool is_frozen = false;
    private bool is_guarded = false;
    private bool accurate_fire = true;
    private int fingerId = -1;
    private int mirrorFire = 1;

    /* Feature Box */
    [HideInInspector]
    public int ammo_auto_counter;
    [HideInInspector]
    public int ammo_cluster_counter = 0;
    [HideInInspector]
    public int ammo_laser_counter = 0;
    [HideInInspector]
    public bool allowFiring = true;

    void Awake() {
        Game.player = this;
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.sortingLayerName = "Background Image";
        _trailRenderer.sortingOrder = 14;
        circleCollider = GetComponent<CircleCollider2D>();
        _transform = transform;
        trail_time = _trailRenderer.time;
        if (Game.SURVIVAL_MODE) {
            bulletPower = 200;
            ammoAmount = 200;
            defeatPoints = 0;
        } else {
            bulletPower = 100;
            ammoAmount = 100;
            defeatPoints = -30;
        }
    }

	void Start () {
        _rigidBody = GetComponent<Rigidbody2D>();
        playerZ = _transform.position.z;
        playerSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
        changePower(Game.PLAYER_POWER, true);
        ammoAmount = Game.PLAYER_AMMO;
        Game.STATUS_BAR.changeAmmo(ammoAmount);
	}

    void Update() {
        #if UNITY_ANDROID
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.fingerId == fingerId) {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                        if (isDraggingAlowed && !LevelController.LEVEL_PAUSED) {
                            removeForces();

                            if (is_frozen) {
                                isDraggingAlowed = false;
                            } else {
                                lastPosition = _transform.position;

                                Vector3 touch_position = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position);
                                touch_position.z = playerZ;
                                _transform.position = touch_position;
                            }
                        } else if (LevelController.LEVEL_PAUSED) {
                            fingerId = -1;
                        }
                    } else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
                        if (isDraggingAlowed) {
                            float last_distance = Vector2.Distance(_transform.position, lastPosition);
                            if (last_distance > 0) {
                                _rigidBody.AddForce((_transform.position - lastPosition) * (Mathf.Max(last_distance, 0.3f) / last_distance) * velocityCoefficient / Time.deltaTime);
                            }
                        }
                        fingerId = -1;
                    }
                } else if (touch.phase == TouchPhase.Began  && !LevelController.LEVEL_PAUSED) {
                    Vector3 touch_position = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position);
                    Collider2D[] hits = Physics2D.OverlapPointAll(touch_position);
                    if (hits != null) {
                        bool isPlayerTouched = false;
                        foreach (Collider2D hit in hits) {
                            if (hit.tag.Equals(Game.PLAYER_TAG)) {
                                fingerId = touch.fingerId;
                                isPlayerTouched = true;
                                removeForces();
                                isDraggingAlowed = true;
                                mirrorWalls = false;
                                break;
                            }
                        }
                        if (!isPlayerTouched) fire(touch_position);
                    }
                }
            }
        }
        #else
        if (Input.GetMouseButtonDown(0)) {
            if (!clickOnSelf) {
                fire(Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));
            } else {
                clickOnSelf = false;
            }
        }
        #endif

        Vector2 playerPosition = _transform.position;
        
        if (isDraggingAlowed) {
            if (playerPosition != null && playerPosition.x < Game.Instance.world_left + playerSize.x / 2) {
                // Left Side
                if (_rigidBody.velocity.x <= 0) {
                    isDraggingAlowed = false;

                    float last_velocity_x = Mathf.Abs(_rigidBody.velocity.x);
                    if (last_velocity_x < pushForce) {
                        circleCollider.enabled = false;
                        _rigidBody.AddForce(new Vector2(last_velocity_x - pushForce, 0));
                    }

                    createGravityGlow(new Vector2(Game.Instance.world_left, playerPosition.y));
                    createGravityGlow(new Vector2(-Game.Instance.world_left, playerPosition.y));
                }
            } else if (playerPosition.x > -playerSize.x / 2 - Game.Instance.world_left) {
                // Right side
                if (_rigidBody.velocity.x >= 0) {
                    isDraggingAlowed = false;

                    float last_velocity_x = Mathf.Abs(_rigidBody.velocity.x);
                    if (last_velocity_x < pushForce) {
                        circleCollider.enabled = false;
                        if (!Game.Instance.level_pause_btn.activeSelf) Game.Instance.level_pause_btn.SetActive(true);
                        _rigidBody.AddForce(new Vector2(pushForce - last_velocity_x, 0));
                    }

                    createGravityGlow(new Vector2(-Game.Instance.world_left, playerPosition.y));
                    createGravityGlow(new Vector2(Game.Instance.world_left, playerPosition.y));
                }
            }

            if (playerPosition.y < Game.Instance.world_bottom + playerSize.x / 2) {
                // Bottom side
                if (_rigidBody.velocity.y <= 0) {
                    isDraggingAlowed = false;

                    float last_velocity_y = Mathf.Abs(_rigidBody.velocity.y);
                    if (last_velocity_y < pushForce) {
                        circleCollider.enabled = false;
                        _rigidBody.AddForce(new Vector2(0, last_velocity_y - pushForce));
                    }

                    createGravityGlow(new Vector2(playerPosition.x, Game.Instance.world_bottom));
                    createGravityGlow(new Vector2(playerPosition.x, -Game.Instance.world_bottom));
                }
            } else if (playerPosition.y > -Game.Instance.world_bottom - playerSize.x / 2) {
                // Top side
                if (_rigidBody.velocity.y >= 0) {
                    isDraggingAlowed = false;

                    float last_velocity_y = Mathf.Abs(_rigidBody.velocity.y);
                    if (last_velocity_y < pushForce) {
                        circleCollider.enabled = false;
                        if (!Game.Instance.level_pause_btn.activeSelf) Game.Instance.level_pause_btn.SetActive(true);
                        _rigidBody.AddForce(new Vector2(0, pushForce - last_velocity_y));
                    }

                    createGravityGlow(new Vector2(playerPosition.x, -Game.Instance.world_bottom));
                    createGravityGlow(new Vector2(playerPosition.x, Game.Instance.world_bottom));
                }
            }
        } else {
            if (mirrorWalls) {
                // Reflect the player after reaching the wall
                if (playerPosition.x < Game.Instance.world_left + playerSize.x / 2) {
                    // Left side
                    _rigidBody.velocity = new Vector2(Mathf.Abs(_rigidBody.velocity.x), _rigidBody.velocity.y);
                } else if (playerPosition.x > -playerSize.x / 2 - Game.Instance.world_left) {
                    // Right side
                    _rigidBody.velocity = new Vector2(-Mathf.Abs(_rigidBody.velocity.x), _rigidBody.velocity.y);
                }

                if (playerPosition.y < Game.Instance.world_bottom + playerSize.x / 2) {
                    // Bottom side
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, Mathf.Abs(_rigidBody.velocity.y));
                } else if (playerPosition.y > -Game.Instance.world_bottom - playerSize.x / 2) {
                    // Top side
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, -Mathf.Abs(_rigidBody.velocity.y));
                }
            } else {
                if (playerPosition.x < Game.Instance.world_left - playerSize.x / 2) {
                    // Go to Right from Left
                    _trailRenderer.time = 0;
                    playerPosition.x = -Game.Instance.world_left + playerSize.x / 2;
                    mirrorWalls = true;
                    _transform.position = playerPosition;
                    circleCollider.enabled = true;
                    StartCoroutine(enableTrailer());
                } else if (playerPosition.x > playerSize.x / 2 - Game.Instance.world_left) {
                    // Go to Left from Right
                    _trailRenderer.time = 0;
                    playerPosition.x = -playerSize.x / 2 + Game.Instance.world_left;
                    mirrorWalls = true;
                    _transform.position = playerPosition;
                    circleCollider.enabled = true;
                    StartCoroutine(enableTrailer());
                }

                if (playerPosition.y > -Game.Instance.world_bottom + playerSize.x / 2) {
                    // Go to Bottom from Top
                    _trailRenderer.time = 0;
                    playerPosition.y = Game.Instance.world_bottom - playerSize.x / 2;
                    mirrorWalls = true;
                    _transform.position = playerPosition;
                    circleCollider.enabled = true;
                    StartCoroutine(enableTrailer());
                } else if (playerPosition.y < Game.Instance.world_bottom - playerSize.x / 2) {
                    // Go to Top from Bottom
                    _trailRenderer.time = 0;
                    playerPosition.y = -Game.Instance.world_bottom + playerSize.x / 2;
                    mirrorWalls = true;
                    _transform.position = playerPosition;
                    circleCollider.enabled = true;
                    StartCoroutine(enableTrailer());
                }
            }
        }
    }

    #if UNITY_STANDALONE
    void OnMouseDrag() {
        if (isDraggingAlowed && !LevelController.LEVEL_PAUSED) {
            removeForces();

            if (is_frozen) {
                isDraggingAlowed = false;
                return;
            }

            // Get last position of gameObject
            lastPosition = _transform.position;
            
            Vector3 mousePosition = Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = playerZ;
            _transform.position = mousePosition;
        }
    }

    void OnMouseUp() {
        if (isDraggingAlowed) {
            float last_distance = Vector2.Distance(_transform.position, lastPosition);
            if (last_distance > 0) {
                _rigidBody.AddForce((_transform.position - lastPosition) * (Mathf.Max(last_distance, 0.3f) / last_distance) * velocityCoefficient / Time.deltaTime);
            }
        }
    }

    void OnMouseDown() {
        removeForces();
        isDraggingAlowed = true;
        mirrorWalls = false;
        clickOnSelf = true;
    }
    #endif

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag.Equals(Game.EXPLOSION_TAG)) {
            changePower(Game.EXPLOSION_POWER);
        } else if (collider.tag.Equals(Game.BLOCKER_TAG) && Game.CURRENT_FEATURE_BOX != null) {
            Enemy enemy = collider.transform.parent.GetComponent<Enemy>();
            if (enemy.name.Equals(Game.COLLECTOR_ENENMY_NAME)) {
                enemy._rigidBody.drag = 7;
                enemy.allowFiring = enemy.temp_allow_firing;
            }
        } else if (collider.name.Equals("pause_handler")) {
            Game.Instance.level_pause_btn.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.tag.Equals(Game.BLOCKER_TAG) && Game.CURRENT_FEATURE_BOX != null) {
            Enemy enemy = collider.transform.parent.GetComponent<Enemy>();
            if (enemy.name.Equals(Game.COLLECTOR_ENENMY_NAME)) {
                enemy.allowFiring = false;
                enemy.collectBox(70);
            }
        } else if (collider.name.Equals("pause_handler")) {
            Game.Instance.level_pause_btn.SetActive(true);
        }
    }

    private void removeForces() {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.angularVelocity = 0;
    }

    private void createGravityGlow(Vector2 position) {
        GameObject glow = Instantiate(Game.Instance.gravityGlow);
        glow.transform.position = position;
    }

    public void changePower(float value, bool absolute = false) {
        if (LevelController.LEVEL_PAUSED) return;
        if (isExploaded) return;

        if (is_guarded && value < 0) return;

        if (absolute) {
            power = value;
        } else {
            power = Mathf.Clamp(power + value, 0, Game.PLAYER_POWER);
        }

        Game.STATUS_BAR.changeHealth(power);

        if (power <= 0) {
            explode();
        }
    }

    IEnumerator enableTrailer() {
        yield return new WaitForSeconds(trail_time);
        _trailRenderer.time = trail_time;
    }

    private void fire(Vector3 target_position, bool reduce_amount = true) {
        if (LevelController.LEVEL_PAUSED) return;

        if (reduce_amount && ammo_cluster_counter > 0) {
            ammo_cluster_counter--;
            GameObject cluster = (GameObject)Instantiate(Resources.Load("Other/Bullet_Cluster"), _transform.position, Quaternion.Euler(Vector2.zero));
            cluster.GetComponent<Bullet>().fire(gameObject, target_position, bulletSpeed, FeatureBox.AMMO_CLUSTER_POWER, true, 1);
            return;
        }

        if (reduce_amount && ammo_laser_counter > 0) {
            ammo_laser_counter--;
            GameObject laser = (GameObject)Instantiate(Resources.Load("Other/Bullet_Laser"), _transform.position, Quaternion.Euler(Vector2.zero));
            laser.GetComponent<Bullet>().fire(gameObject, target_position, bulletSpeed, FeatureBox.AMMO_LASER_POWER, true, 1, true);
            return;
        }

        if (!reduce_amount || (ammoAmount > 0 && allowFiring)) {
            if (reduce_amount && Game.FEATURES[Game.FEATURE_TYPE_INFINITE_AMMO] != Game.FEATURE_STATE_ACTIVE) {
                ammoAmount--;
                Game.STATUS_BAR.changeAmmo(ammoAmount);
            }
            GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, _transform.position, Quaternion.Euler(Vector2.zero));
            bullet.GetComponent<Bullet>().fire(gameObject, target_position, bulletSpeed * mirrorFire, bulletPower, accurate_fire);
        }
    }

    public void explode() {
        if (LevelController.LEVEL_PAUSED) return;
        Game.Instance.changePoints(defeatPoints);
        isExploaded = true;
        Instantiate(Game.Instance.playerExplosion, _transform.position, Quaternion.Euler(Vector3.zero));
        Game.Instance.shakeCamera();
        Game.LEVEL_CONTROLLER.failLevel();
        Destroy(gameObject);
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

    public void noFire() {
        allowFiring = false;
        StartCoroutine(stopNoFire());
    }

    IEnumerator stopNoFire() {
        yield return new WaitForSeconds(FeatureBox.NO_FIRE_TIME);
        allowFiring = true;
    }

    public void autoFire() {
        ammo_auto_counter = FeatureBox.AMMO_AUTO_COUNTER;
        StartCoroutine(autoAmmoFire());
    }

    IEnumerator autoAmmoFire() {
        while (ammo_auto_counter > 0) {
            if (Game.ALL_ENEMIES.Count > 0 && !LevelController.LEVEL_PAUSED) {
                try {
                    fire(Game.ALL_ENEMIES[0].transform.position, false);
                    ammo_auto_counter--;
                } catch {}
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void guard() {
        GameObject guard_circle = (GameObject)Instantiate(Game.Instance.guardCircle, _transform.position, Quaternion.Euler(Vector3.zero));
        guard_circle.transform.parent = _transform;
        guard_circle.name = gameObject.name + Game.GUARD_TAG;
        is_guarded = true;
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

    public void mirrorFiring() {
        GameObject inaccurate_fire = (GameObject)Instantiate(Game.Instance.inaccurateFiring, _transform.position, Quaternion.Euler(Vector3.zero));
        inaccurate_fire.transform.parent = _transform;
        mirrorFire = -1;
        StartCoroutine(straightFiring());
    }

    IEnumerator straightFiring() {
        yield return new WaitForSeconds(FeatureBox.MIRROR_FIRE_TIME);
        mirrorFire = 1;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}