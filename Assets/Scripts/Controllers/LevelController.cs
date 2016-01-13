using UnityEngine;
using System.Collections;

public class LevelController : MenuController {
    [Header("General Settings")]
    public SceneLoader sceneLoader;
    public GameObject PLAYER;
    public GameObject pause_btn;
    public GameObject medikit;
    public GameObject featureBox;
    public GameObject ammoBox;
    public GameObject mine;
    public TextMesh time_txt;

    public static bool LEVEL_PAUSEABLE = false;
    public static bool LEVEL_PAUSED = false;

    private GameObject objectsHolder = null;
    private BaseController controller;
    private float grid_width;
    private int last_outside_place = 0;

    void Awake() {
        catchAll = true;
        Game.LEVEL_CONTROLLER = this;
    }

    void Start() {
        Game.Instance.level_pause_btn = pause_btn;
        pause_btn = null;
        grid_width = Game.Instance.world_width / 4;
        startLevel();
    }

    public override void onHit(Collider2D hit, ref bool isTouched) {
        switch (hit.name) {
            case "continue_btn":
                resume();
                break;
            case "restart_btn":
                Time.timeScale = 1;
                startLevel();
                break;
            case "pause_btn":
                back();
                break;
            case "ceasefire_btn":
                backToLevelsMenu(!Game.SURVIVAL_MODE);
                break;
            case "back_btn":
                backToLevelsMenu(false);
                break;
            case "next_btn":
                Game.SURVIVAL_MODE = false;
                if (Game.CURRENT_LEVEL < Game.LEVELS_STATUS.Length - 2) {
                    Time.timeScale = 1;
                    Game.Instance.setCurrentLevel(Game.CURRENT_LEVEL + 1);
                    startLevel();
                } else {
                    backToLevelsMenu(false);
                }
                break;
        }
    }

    private void backToLevelsMenu(bool isCeasefire) {
        Time.timeScale = 1;
        LEVEL_PAUSEABLE = false;
        LEVEL_PAUSED = true;
        StopAllCoroutines();
        if (isCeasefire) Game.Instance.ceasefire();
        if (Game.SURVIVAL_MODE) {
            sceneLoader.loadScene("MainMenu", true);
        } else {
            sceneLoader.loadScene("Levels", true);
        }
    }

    public override void touchDetected() { }

    public override void touchReleased(Vector3 position) { }

    public override void back() {
        if (sceneLoader.pausing || !LEVEL_PAUSEABLE) return;
        if (LEVEL_PAUSED) {
            resume();
        } else {
            pause();
        }
    }

    private void pause() {
        LEVEL_PAUSED = true;
        sceneLoader.pauseGame();
    }

    private void resume() {
        Time.timeScale = 1;
        sceneLoader.displayScene();
    }

    private void startLevel() {
        StopAllCoroutines();
        LEVEL_PAUSED = false;
        setPauseable(false);

        removeObjectsHolder();
        Game.player = null;
        objectsHolder = new GameObject("Objects_Holder");
        Instantiate(PLAYER).transform.parent = objectsHolder.transform;

        if (sceneLoader.isClosed) sceneLoader.displayScene();

        if (Game.SURVIVAL_MODE) {
            time_txt.text = "SURVIVAL MODE";
            StartCoroutine(survivalMode());
        } else {
            time_txt.text = "LEVEL " + Game.CURRENT_LEVEL;
            StartCoroutine(levelNotification());
        }

        setPauseable(true);
    }

    public void removeObjectsHolder() {
        if (objectsHolder != null) Destroy(objectsHolder);
    }

    IEnumerator survivalMode() {
        yield return new WaitForSeconds(2);
        controller = new Survival();
    }


    IEnumerator levelNotification() {
        yield return new WaitForSeconds(2);
        switch (Game.CURRENT_LEVEL) {
            case 1:
                controller = new Level1();
                break;
            case 2:
                controller = new Level2();
                break;
            case 3:
                controller = new Level3();
                break;
            case 4:
                controller = new Level4();
                break;
            case 5:
                controller = new Level5();
                break;
            case 6:
                controller = new Level6();
                break;
            case 7:
                controller = new Level7();
                break;
            case 8:
                controller = new Level8();
                break;
            case 9:
                controller = new Level9();
                break;
            case 10:
                controller = new Level10();
                break;
            case 11:
                controller = new Level11();
                break;
            case 12:
                controller = new Level12();
                break;
            case 13:
                controller = new Level13();
                break;
            case 14:
                controller = new Level14();
                break;
            case 15:
                controller = new Level15();
                break;
            case 16:
                controller = new Level16();
                break;
            case 17:
                controller = new Level17();
                break;
            case 18:
                controller = new Level18();
                break;
            case 19:
                controller = new Level19();
                break;
            case 20:
                controller = new Level20();
                break;
            case 21:
                controller = new Level21();
                break;
            case 22:
                controller = new Level22();
                break;
            case 23:
                controller = new Level23();
                break;
        }
    }

    public static void setPauseable(bool pauseable) {
        LEVEL_PAUSEABLE = pauseable;
        Game.Instance.level_pause_btn.SetActive(pauseable);
    }

    private void setInsidePoints(out Vector2 start_point, out Vector2 end_point, int grid) {
        start_point = new Vector2();
        end_point = new Vector2();

        switch (grid) {
            case 2:
                start_point.x = grid_width * -1;
                start_point.y = -Game.Instance.world_bottom;
                end_point.x = 0;
                end_point.y = 0;
                break;
            case 3:
                start_point.x = 0;
                start_point.y = -Game.Instance.world_bottom;
                end_point.x = grid_width;
                end_point.y = 0;
                break;
            case 4:
                start_point.x = grid_width;
                start_point.y = -Game.Instance.world_bottom;
                end_point.x = grid_width * 2;
                end_point.y = 0;
                break;
            case 5:
                start_point.x = grid_width * -2;
                start_point.y = 0;
                end_point.x = grid_width * -1;
                end_point.y = Game.Instance.world_bottom;
                break;
            case 6:
                start_point.x = grid_width * -1;
                start_point.y = 0;
                end_point.x = 0;
                end_point.y = Game.Instance.world_bottom;
                break;
            case 7:
                start_point.x = 0;
                start_point.y = 0;
                end_point.x = grid_width;
                end_point.y = Game.Instance.world_bottom;
                break;
            case 8:
                start_point.x = grid_width;
                start_point.y = 0;
                end_point.x = grid_width * 2;
                end_point.y = Game.Instance.world_bottom;
                break;
        }
    }

    private Vector2 getInsidePosition() {
        int grid = Random.Range(2, 9);
        int first_grid = grid;

        Vector2 start_point;
        Vector2 end_point;

        setInsidePoints(out start_point, out end_point, grid);

        while (Physics2D.OverlapArea(start_point, end_point, Game.Instance.enemyMask) != null) {
            grid++;
            if (grid > 8) grid = 2;
            if (first_grid == grid) {
                return Vector2.zero;
            }
            setInsidePoints(out start_point, out end_point, grid);
        }

        return (end_point + start_point) / 2;
    }

    public void addMedikit() {
        Vector2 inside_position = getInsidePosition();
        if (inside_position == Vector2.zero) return;

        ((GameObject)Instantiate(medikit, inside_position, Quaternion.Euler(Vector3.zero))).transform.parent = objectsHolder.transform;
    }

    public void addFeatureBox(int type = 0) {
        Vector2 inside_position = getInsidePosition();
        if (inside_position == Vector2.zero) return;

        if (type == 0) {
            ((GameObject)Instantiate(featureBox, inside_position, Quaternion.Euler(Vector3.zero))).transform.parent = objectsHolder.transform;
        } else {
            GameObject box = (GameObject)Instantiate(featureBox, inside_position, Quaternion.Euler(Vector3.zero));
            box.transform.parent = objectsHolder.transform;
            box.GetComponentInChildren<FeatureBox>().type = type;
        }
    }

    public void addAmmoBox() {
        Vector2 inside_position = getInsidePosition();
        if (inside_position == Vector2.zero) return;

        ((GameObject)Instantiate(ammoBox, inside_position, Quaternion.Euler(Vector3.zero))).transform.parent = objectsHolder.transform;
    }

    public void addMine() {
        Vector2 inside_position = getInsidePosition();
        if (inside_position == Vector2.zero) return;

        ((GameObject)Instantiate(mine, inside_position, Quaternion.Euler(Vector3.zero))).transform.parent = objectsHolder.transform;
    }

    public void addObject(GameObject obj, float delay) {
        StartCoroutine(addObjectDelayed(obj, delay));
    }

    IEnumerator addObjectDelayed(GameObject obj, float delay) {
        yield return new WaitForSeconds(delay);
        addObject(obj);
    }

    public void addObject(GameObject obj) {
        int place = Random.Range(1, 7);
        while (last_outside_place == place) {
            place = Random.Range(1, 7);
        }
        last_outside_place = place;

        Vector2 outside_position = new Vector2();

        switch (place) {
            case 1:
                // LEFT
                outside_position.x = Game.Instance.world_left - 1;
                outside_position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                break;
            case 2:
                // TOP_LEFT
                outside_position.y = 1 - Game.Instance.world_bottom;
                outside_position.x = Random.Range(Game.Instance.world_left, 0);
                break;
            case 3:
                // TOP_RIGHT
                outside_position.y = 1 - Game.Instance.world_bottom;
                outside_position.x = Random.Range(0, -Game.Instance.world_left);
                break;
            case 4:
                // RIGHT
                outside_position.x = 1 - Game.Instance.world_left;
                outside_position.y = Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom);
                break;
            case 5:
                // BOTTOM_RIGHT
                outside_position.y = Game.Instance.world_bottom - 1;
                outside_position.x = Random.Range(0, -Game.Instance.world_left);
                break;
            case 6:
                // BOTTOM_LEFT
                outside_position.y = Game.Instance.world_bottom - 1;
                outside_position.x = Random.Range(Game.Instance.world_left, 0);
                break;
        }

        ((GameObject)Instantiate(obj, outside_position, Quaternion.Euler(Vector3.zero))).transform.parent = objectsHolder.transform;
    }

    public void clearLevel() {
        StopAllCoroutines();
        LEVEL_PAUSEABLE = false;
        LEVEL_PAUSED = true;
        int value = 2;
        if (Game.player.power >= 3000) {
            value = 4;
        } else if (Game.player.power >= 1500) {
            value = 3;
        }
        Game.Instance.saveLevel(Game.CURRENT_LEVEL, value, true);
        sceneLoader.clearLevel(value - 1);
    }

    public void failLevel() {
        StopAllCoroutines();
        LEVEL_PAUSEABLE = false;
        LEVEL_PAUSED = true;
        Game.Instance.savePoints();
        StartCoroutine(waitForFail());
    }

    public void enemyDefeated() {
        if (controller != null) controller.enemyDefeated();
    }

    IEnumerator waitForFail() {
        yield return new WaitForSeconds(1.2f);
        sceneLoader.failLevel();
    }

    void OnDestroy() {
        StopAllCoroutines();
        Game.LEVEL_CONTROLLER = null;
    }
}
