using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {
    private const int TYPE_LOADING = 0;
    private const int TYPE_PAUSED = 1;
    private const int TYPE_CLEARED = 2;
    private const int TYPE_FAILED = 3;

    [Header("General Settings")]
    public Transform fillerRight;
    public Transform fillerLeft;
    public Transform rightSide;
    public Transform leftSide;

    [Header("Holder Settings")]
    public bool removeHolder = true;
    public GameObject holder;
    public GameObject rightSideObject;
    public GameObject leftSideObject;

    [Header("Menu Settings")]
    public GameObject pauseHolder;
    public GameObject clearedHolder;
    public GameObject failedHolder;
    public bool setLevelPauseable = false;

    [Header("Star Settings")]
    public GameObject gold1Right;
    public GameObject gold1Left;
    public GameObject gold2;
    public GameObject gold3;

    [Header("Type Settings")]
    public SpriteRenderer image_left;
    public SpriteRenderer image_right;
    public Sprite loading_left;
    public Sprite loading_right;
    public Sprite paused_left;
    public Sprite paused_survival_left;
    public Sprite paused_right;
    public Sprite cleared_left;
    public Sprite cleared_right;
    public Sprite failed_left;
    public Sprite failed_right;
    public Sprite survival_left;
    public Sprite survival_right;

    private float animationSpeed = 50;
    private string scene_name;
    private bool showLoading = false;
    private bool hideLoading = false;
    private float position;
    private float position_def;
    private int type = 0;
    [HideInInspector]
    public bool pausing = false;
    [HideInInspector]
    public bool isClosed = false;
    private int postponeAction = -1;
    private int gold_number_postponed;
    private float fillerZ;
    private AudioSource audio;

    void Awake() {
        audio = GetComponent<AudioSource>();
    }

	void Start () {
        enabled = false;

        Vector3 scale = new Vector3(Game.Instance.world_width / 2 * 3.5f, Game.Instance.world_height * 3, 1);
        fillerLeft.localScale = scale;
        fillerRight.localScale = scale;

        position_def = Game.Instance.world_left - 1f;

        fillerZ = leftSide.position.z;

        if (Game.ON_BOOTSTRAP) {
            position = position_def;
            leftSide.position = new Vector3(position, 0, fillerZ);
            rightSide.position = new Vector3(-position, 0, fillerZ);
            Game.ON_BOOTSTRAP = false;
        } else {
            position = 0;
            StartCoroutine(openDoor());
        }
	}
	
	void Update () {
        if (showLoading) {
            if (position < 0) {
                position += Time.deltaTime * animationSpeed;

                if (position >= 0) {
                    position = 0;
                    if (type != TYPE_PAUSED && !setLevelPauseable) {
                        Instantiate(Game.Instance.menuGlow);
                    }
                }

                leftSide.position = new Vector3(position, 0, fillerZ);
                rightSide.position = new Vector3(-position, 0, fillerZ);
            } else {
                showLoading = false;
                enabled = false;

                switch (type) {
                    case TYPE_LOADING:
                        if (removeHolder) Destroy(holder);
                        startLoadingLevel();
                        break;
                    case TYPE_PAUSED:
                        pausing = false;
                        pauseHolder.SetActive(true);
                        if (setLevelPauseable) {
                            Game.Instance.stopShakingCamera();
                            Time.timeScale = 0;
                        }
                        break;
                    case TYPE_CLEARED:
                        clearedHolder.SetActive(true);
                        try {
                            Game.LEVEL_CONTROLLER.removeObjectsHolder();
                        } catch { }
                        break;
                    case TYPE_FAILED:
                        failedHolder.SetActive(true);
                        try {
                            Game.LEVEL_CONTROLLER.removeObjectsHolder();
                        } catch { }
                        break;
                }
            }
        } else if (hideLoading) {
            if (position > position_def) {
                position -= Time.deltaTime * animationSpeed;

                if (position <= position_def) {
                    position = position_def;
                }

                leftSide.position = new Vector3(position, 0, fillerZ);
                rightSide.position = new Vector3(-position, 0, fillerZ);
            } else {
                hideLoading = false;
                enabled = false;
                isClosed = false;

                if (postponeAction > -1) {
                    switch (postponeAction) {
                        case TYPE_LOADING:
                            postponeAction = -1;
                            loadScene(scene_name, true);
                            break;
                        case TYPE_PAUSED:
                            postponeAction = -1;
                            pauseGame();
                            break;
                        case TYPE_CLEARED:
                            postponeAction = -1;
                            clearLevel(gold_number_postponed);
                            break;
                        case TYPE_FAILED:
                            postponeAction = -1;
                            failLevel();
                            break;
                    }
                } else {
                    if (setLevelPauseable) {
                        LevelController.LEVEL_PAUSED = false;
                    }
                    rightSideObject.SetActive(false);
                    leftSideObject.SetActive(false);
                }
            }
        }
	}

    private void displayGolds(int numbers) {
        switch (numbers) {
            case 0:
                gold1Left.SetActive(false);
                gold1Right.SetActive(false);
                gold2.SetActive(false);
                gold3.SetActive(false);
                break;
            case 1:
                gold1Left.SetActive(true);
                gold1Right.SetActive(true);
                gold2.SetActive(false);
                gold3.SetActive(false);
                break;
            case 2:
                gold1Left.SetActive(true);
                gold1Right.SetActive(true);
                gold2.SetActive(true);
                gold3.SetActive(false);
                break;
            case 3:
                gold1Left.SetActive(true);
                gold1Right.SetActive(true);
                gold2.SetActive(true);
                gold3.SetActive(true);
                break;
        }
    }

    public void failLevel() {
        if (isClosed) {
            displayScene(TYPE_FAILED);
            return;
        }

        isClosed = true;
        if (type != TYPE_FAILED) {
            type = TYPE_FAILED;
            displayGolds(0);
            if (Game.SURVIVAL_MODE) {
                image_left.sprite = survival_left;
                image_right.sprite = survival_right;
            } else {
                image_left.sprite = failed_left;
                image_right.sprite = failed_right;
            }
        }
        closeDoor();
    }

    public void clearLevel(int gold_numbers) {
        if (isClosed) {
            gold_number_postponed = gold_numbers;
            displayScene(TYPE_CLEARED);
            return;
        }

        isClosed = true;

        if (type != TYPE_CLEARED) {
            type = TYPE_CLEARED;
            displayGolds(gold_numbers);
            image_left.sprite = cleared_left;
            image_right.sprite = cleared_right;
        }

        closeDoor();
    }

    public void pauseGame() {
        if (isClosed) {
            displayScene(TYPE_PAUSED);
            return;
        }

        isClosed = true;
        pausing = true;
        if (type != TYPE_PAUSED) {
            type = TYPE_PAUSED;
            displayGolds(0);
            if (Game.SURVIVAL_MODE) {
                image_left.sprite = paused_survival_left;
            } else {
                image_left.sprite = paused_left;
            }
            image_right.sprite = paused_right;
        }
        closeDoor();
    }

    public void loadScene(string scene, bool changeType = false) {
        scene_name = scene;

        if (isClosed) {
            displayScene(TYPE_LOADING);
            return;
        }

        isClosed = true;

        if (changeType && type != TYPE_LOADING) {
            type = TYPE_LOADING;
            displayGolds(0);
            image_left.sprite = loading_left;
            image_right.sprite = loading_right;
        }

        closeDoor();
    }

    private void closeDoor() {
        rightSideObject.SetActive(true);
        leftSideObject.SetActive(true);
        animationSpeed = 50;
        showLoading = true;
        audio.Play();
        enabled = true;
    }

    public void displayScene(int postpone = -1) {
        postponeAction = postpone;

        if (type == TYPE_PAUSED) pauseHolder.SetActive(false);
        if (type == TYPE_CLEARED) clearedHolder.SetActive(false);
        if (type == TYPE_FAILED) failedHolder.SetActive(false);

        animationSpeed = (postpone == -1 ? 14 : 50);
        hideLoading = true;
        enabled = true;
    }

    private void startLoadingLevel() {
        StartCoroutine(loadLevelAsync());
    }

    IEnumerator loadLevelAsync() {
        yield return new WaitForSeconds(0.7f);
        Application.LoadLevelAsync(scene_name);
    }

    IEnumerator openDoor() {
        yield return new WaitForSeconds(0.1f);
        displayScene();
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
