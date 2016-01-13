using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtraController : MonoBehaviour {
    [Header("General Settings")]
    public SceneLoader sceneLoader;
    public Transform cards;
    public ActivationButton[] features;
    public TextMesh score_txt;

    [Header("Image Settings")]
    public Sprite activate_png;
    public Sprite enable_png;
    public Sprite disable_png;

    private float start_position;
    private float cards_start_position;
    private Vector3 cards_position;

    [Header("Message Settings")]
    public GameObject failedMessage;
    public Transform failedFiller;

    private const float MIN_SWIPE_DISTANCE = 0.1f;

    #if UNITY_ANDROID
    private int draggerFingerId = -1;
    private Dictionary<int, float> fingerId_startPosition = new Dictionary<int,float>();
    #else
    private bool isDragging = false;
    #endif

    void Awake() {
        Game.EXTRA_CONTROLLER = this;
    }

    void Start() {
        cards_position = cards.position;
        setScore();
        failedFiller.localScale = new Vector3(Game.Instance.world_width * 3 + 1, Game.Instance.world_height * 3 + 1, 1);
    }

    void Update () {
        #if UNITY_ANDROID
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    int fingerId = touch.fingerId;
                    float fingerStartPosition = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position).x;
                    
                    fingerId_startPosition.Add(fingerId, fingerStartPosition);

                    if (draggerFingerId == -1) {
                        draggerFingerId = fingerId;
                        start_position = fingerStartPosition;
                        cards_start_position = cards.position.x;
                    }
                } else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
                    int fingerId = touch.fingerId;
                    Vector3 touch_position = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position);
                    float fingerStartPosition = fingerId_startPosition[fingerId];
                    float delta_x = touch_position.x - fingerStartPosition;

                    if (Mathf.Abs(delta_x) < MIN_SWIPE_DISTANCE) {
                        if (fingerId == draggerFingerId) {
                            draggerFingerId = -1;
                        }
                        onTouch(touch_position);
                    } else if (fingerId == draggerFingerId) {
                        draggerFingerId = -1;
                    }

                    fingerId_startPosition.Remove(fingerId);
                } else if (touch.phase == TouchPhase.Moved) {
                    if (touch.fingerId == draggerFingerId) {
                        dragCards(Game.Instance.mainCamera.ScreenToWorldPoint(touch.position).x);
                    }
                }
            }
        }
        #else
        if (Input.GetMouseButtonDown(0)) {
            start_position = Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
            cards_start_position = cards.position.x;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            Vector3 clickPosition = Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (clickPosition.x == start_position) {
                onTouch(clickPosition);
            }
        }

        if (isDragging) {
            dragCards(Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition).x);
        }
        #endif

        if (Input.GetKeyDown(KeyCode.Escape)) {
            back();
        }
	}

    private void setScore() {
        score_txt.text = Game.Instance.points.ToString("n0");
    }

    private void dragCards(float current_position) {
        float positionX = current_position - start_position + cards_start_position;
        cards_position.x = Mathf.Clamp(positionX, -6.7f, -2.4f);
        cards.position = cards_position;
    }

    private void onTouch(Vector3 position) {
        if (failedMessage.activeSelf) {
            failedMessage.SetActive(false);
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(position);
        switch (hit.name) {
            case "back_btn":
                enabled = false;

                Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(0.38f, 0.23f, 0), Quaternion.Euler(Vector3.zero));
                Destroy(hit.transform.parent.gameObject);

                StartCoroutine(onClickBack());
                break;
            case "easy_btn":
                onClickActivation(Game.FEATURE_TYPE_EASY_MODE);
                break;
            case "ammo_btn":
                onClickActivation(Game.FEATURE_TYPE_INFINITE_AMMO);
                break;
            case "reveal_btn":
                onClickActivation(Game.FEATURE_TYPE_DISCOVER_BOXES);
                break;
            case "enemy_btn":
                onClickActivation(Game.FEATURE_TYPE_STUPID_ENEMIES);
                break;
            case "power_btn":
                onClickActivation(Game.FEATURE_TYPE_TWO_TIMES_POWER);
                break;
        }
    }

    private void onClickActivation(int id) {
        switch (Game.FEATURES[id]) {
            case Game.FEATURE_STATE_LOCK:
                if (features[id].pts < Game.Instance.points) {
                    activate(id, true);
                } else {
                    activationFailed();
                }
                break;
            case Game.FEATURE_STATE_ACTIVE:
                features[id].disable();
                break;
            case Game.FEATURE_STATE_DISABLE:
                features[id].enable();
                break;
        }
    }

    private void activationFailed() {
        failedMessage.SetActive(true);
    }

    private void activate(int id, bool using_points) {
        features[id].activate(using_points);

        int length = features.Length;
        for (int i = 0; i < length; i++) {
            features[i].resetImage();
        }

        setScore();
    }

    IEnumerator onClickBack() {
        yield return new WaitForSeconds(0.3f);
        back();
    }

    private void back() {
        if (failedMessage.activeSelf) {
            failedMessage.SetActive(false);
            return;
        }

        enabled = false;
        sceneLoader.loadScene("MainMenu");
    }

    void OnDestroy() {
        StopAllCoroutines();
        Game.EXTRA_CONTROLLER = null;
    }
}