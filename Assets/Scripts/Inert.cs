using UnityEngine;
using System.Collections;

public class Inert : MonoBehaviour {

    [Header("Main Settings")]
    public bool resize = true;
    public float minScale = 0.5f;
    public float speed = 1;
    private float invisibleTimer = 3;

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _renderer;
    private Transform _transform;
    private float positionZ;

    void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start () {
        _transform = transform;
        positionZ = _transform.position.z;
        move();
	}

    void Update() {
        if (!_renderer.isVisible) {
            if (invisibleTimer <= 0) {
                move(true);
            } else {
                invisibleTimer -= Time.deltaTime;
            }
        }
    }

    private void move(bool set_position = false) {
        invisibleTimer = 3;

        if (set_position) {
            Vector2 position = new Vector2();
            switch (Random.Range(0, 4)) {
                case 0:
                    position.y = 1 - Game.Instance.world_bottom;
                    position.x = Random.Range(Game.Instance.world_left, -Game.Instance.world_left);
                    break;
                case 1:
                    position.x = 1 - Game.Instance.world_left;
                    position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                    break;
                case 2:
                    position.y = Game.Instance.world_bottom - 1;
                    position.x = Random.Range(Game.Instance.world_left, -Game.Instance.world_left);
                    break;
                default:
                    position.x = Game.Instance.world_left - 1;
                    position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                    break;
            }

            _transform.position = position;

            if (resize) {
                float scale = Random.Range(minScale, 1);
                _transform.localScale = new Vector2(scale, scale);
            }
        }

        _rigidBody.AddForce((new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), positionZ) - _transform.position) * speed);
    }

    void OnBecameInvisible() {
        move(true);
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
