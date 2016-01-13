using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsController : MonoBehaviour {
    [Header("General Settings")]
    public Transform levels;
    public Indicator indicator;
    public SceneLoader sceneLoader;
    public TextMesh score_txt;
    public TextMesh gold_txt;

    [Header("Images Settings")]
    public Sprite locked;
    public Sprite done;

    private float start_position;
    private float levels_start_position;
    private Vector3 levels_position;

    #if UNITY_ANDROID
    private int draggerFingerId = -1;
    private Dictionary<int, float> fingerId_startPosition;
    private Dictionary<int, float> fingerId_startTime;
    private const float MAX_SWIPE_TIME = 0.5f;
    private const float MIN_SWIPE_DISTANCE = 0.1f;
    #else
    private bool isDragging = false;
    #endif

    void Awake() {
        Game.LEVELS_CONTROLLER = this;
        #if UNITY_ANDROID
        fingerId_startPosition = new Dictionary<int,float>();
        fingerId_startTime = new Dictionary<int, float>();
        #endif
    }

	void Start () {
        levels_position = levels.position;
        score_txt.text = Game.Instance.points.ToString("n0");
        gold_txt.text = Game.Instance.countGolds().ToString() + "/" + (Game.LEVELS_STATUS.Length * 3).ToString();
	}
	
	void Update () {
        #if UNITY_ANDROID
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    int fingerId = touch.fingerId;
                    float fingerStartPosition = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position).x;

                    fingerId_startPosition.Add(fingerId, fingerStartPosition);
                    fingerId_startTime.Add(fingerId, Time.time);

                    if (draggerFingerId == -1) {
                        draggerFingerId = fingerId;
                        start_position = fingerStartPosition;
                        levels_start_position = levels.position.x;
                    }
                } else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
                    int fingerId = touch.fingerId;
                    float fingerStartTime = fingerId_startTime[fingerId];
                    Vector3 touch_position = Game.Instance.mainCamera.ScreenToWorldPoint(touch.position);
                    float fingerStartPosition = fingerId_startPosition[fingerId];
                    float delta_x = touch_position.x - fingerStartPosition;

                    if (Mathf.Abs(delta_x) < MIN_SWIPE_DISTANCE) {
                        if (fingerId == draggerFingerId) {
                            draggerFingerId = -1;
                        }
                        onTouch(touch_position);
                    } else if (Time.time - fingerStartTime < MAX_SWIPE_TIME && delta_x != 0) {
                        if (fingerId == draggerFingerId) {
                            draggerFingerId = -1;
                            if (delta_x > 0) {
                                indicator.animateAfterDrag(-1);
                            } else {
                                indicator.animateAfterDrag(1);
                            }
                        }
                    } else if (fingerId == draggerFingerId) {
                        draggerFingerId = -1;
                        indicator.animateAfterDrag();
                    } 

                    fingerId_startPosition.Remove(fingerId);
                    fingerId_startTime.Remove(fingerId);
                } else if (touch.phase == TouchPhase.Moved) {
                    if (touch.fingerId == draggerFingerId) {
                        dragLevels(Game.Instance.mainCamera.ScreenToWorldPoint(touch.position).x);
                    }
                }
            }
        }
        #else
        if (Input.GetMouseButtonDown(0)) {
            start_position = Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
            levels_start_position = levels.position.x;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            Vector3 clickPosition = Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (clickPosition.x == start_position) {
                onTouch(clickPosition);
            } else {
                indicator.animateAfterDrag();
            }
        }

        if (isDragging) {
            dragLevels(Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition).x);
        }
        #endif

        if (Input.GetKeyDown(KeyCode.Escape)) {
            back();
        }
	}

    private void onTouch(Vector3 position) {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);
        bool touch_detected = false;
        if (hits != null) {
            foreach (Collider2D hit in hits) {
                if (hit.tag.Equals(Game.LEVEL_TAG)) {
                    enabled = false;
                    touch_detected = true;
                    int level = int.Parse(hit.name);
                    Game.Instance.setCurrentLevel(level);

                    Instantiate(Game.Instance.boxCollected, hit.transform.position, Quaternion.Euler(Vector3.zero));
                    Destroy(hit.gameObject);

                    if (level == 0) {
                        StartCoroutine(loadLevel("Tutorial"));
                    } else {
                        StartCoroutine(loadLevel("NormalLevel"));
                    }
                } else if (hit.name.Equals("back_btn")) {
                    enabled = false;
                    touch_detected = true;

                    Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(0.38f, 0.23f, 0), Quaternion.Euler(Vector3.zero));
                    Destroy(hit.transform.parent.gameObject);

                    StartCoroutine(onClickBack());
                }
            }
        }
        if (!touch_detected) indicator.hideOtherLevels();
    }

    private void dragLevels(float current_position) {
        float positionX = current_position - start_position + levels_start_position;
        levels_position.x = positionX;
        indicator.activateNeighbours(positionX);
        levels.position = levels_position;
    }

    IEnumerator onClickBack() {
        yield return new WaitForSeconds(0.3f);
        back();
    }

    IEnumerator loadLevel(string scene_name) {
        yield return new WaitForSeconds(0.3f);
        Game.SURVIVAL_MODE = false;
        sceneLoader.loadScene(scene_name);
    }

    private void back() {
        enabled = false;
        sceneLoader.loadScene("MainMenu");
    }

    void OnDestroy() {
        StopAllCoroutines();
        Game.LEVELS_CONTROLLER = null;
    }
}
