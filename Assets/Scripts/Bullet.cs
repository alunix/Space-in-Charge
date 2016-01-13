using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    private float power;
    private float destroy_time;
    private Rigidbody2D _rigidbody;
    private bool isExploaded = false;
    private bool isLaser;

    private GameObject owner;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        enabled = false;
    }

    void OnBecameInvisible() {
        explode(false);
    }

    public void fire(GameObject owner, Vector3 target_position, float speed, float power, bool is_accurate, float destroy_time = 0, bool isLaser = false) {
        this.owner = owner;
        this.power = power;
        this.destroy_time = destroy_time;
        this.isLaser = isLaser;

        try {
            Vector2 delta = target_position - transform.position + (is_accurate ? Vector3.zero : new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0));
            int velocity_coefficient = 200;

            float max_delta = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

            if (max_delta < velocity_coefficient) {
                delta *= velocity_coefficient / max_delta;
            }

            _rigidbody.AddForce(delta * speed);
        } catch { }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (isExploaded || collider.gameObject == owner) return;

        try {
            if (collider.tag.Equals(Game.PLAYER_TAG)) {
                if (Game.FEATURES[Game.FEATURE_TYPE_EASY_MODE] == Game.FEATURE_STATE_ACTIVE) {
                    Game.player.changePower(-power / 2);
                } else {
                    Game.player.changePower(-power);
                }
                explode(true);
                if (destroy_time > 0) {
                    Game.Instance.shakeCamera();
                }
            } else if (collider.tag.Equals(Game.ENEMY_HIT_TAG)) {
                if (Game.FEATURES[Game.FEATURE_TYPE_EASY_MODE] == Game.FEATURE_STATE_ACTIVE) {
                    collider.transform.parent.GetComponent<Enemy>().explode();
                } else {
                    collider.transform.parent.GetComponent<Enemy>().changePower(-power, true);
                }
                explode(true);
            } else if (collider.tag.Equals(Game.FEATURE_BOX_TAG)) {
                collider.GetComponent<FeatureBox>().explode();
                explode(true);
            } else if (collider.tag.Equals(Game.GUARD_TAG) && !collider.name.Equals(owner.name + Game.GUARD_TAG)) {
                explode(true);
            }
        } catch { }
    }

    private void explode(bool explosion) {
        if (isExploaded || isLaser) return;

        isExploaded = true;

        if (explosion) {
            Instantiate(destroy_time > 0 ? Game.Instance.boxExplosion : Game.Instance.bulletExplosion, transform.position, Quaternion.Euler(Vector3.zero));
        }

        if (destroy_time > 0) {
            _rigidbody.velocity = Vector2.zero;
            Destroy(gameObject, destroy_time);
        } else {
            Destroy(gameObject);
        }
    }
}
