using UnityEngine;
using System.Collections;

public class MainMenu : MenuController {
    [Header("Object Settings")]
    public GameObject playerShip;
    public GameObject playIcon;
    public SceneLoader sceneLoader;

    [Header("Message Settings")]
    public GameObject survivalMessage;
    public Transform messageFiller;

    private bool allow_exit = false;

    void Start() {
        catchAll = false;
        messageFiller.localScale = new Vector3(Game.Instance.world_width * 3 + 1, Game.Instance.world_height * 3 + 1, 1);
    }

    public override void onHit(Collider2D hit, ref bool isTouched) {
        if (survivalMessage.activeSelf) {
            survivalMessage.SetActive(false);
            return;
        }

        switch (hit.name) {
            case "about_btn":
                Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(-0.75f, 0.24f, 0), Quaternion.Euler(Vector3.zero));
                Destroy(hit.gameObject);
                StartCoroutine(onClickAbout());
                isTouched = true;
                break;
            case "survival_btn":
                if (Game.GOLDS < Game.LEVELS_STATUS.Length * 3) {
                    survivalMessage.SetActive(true);
                } else {
                    Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(0.54f, 0.24f, 0), Quaternion.Euler(Vector3.zero));
                    Destroy(hit.gameObject);
                    StartCoroutine(onClickSurvival());
                    isTouched = true;
                }
                break;
            case "extra_btn":
                Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(0.38f, 0.24f, 0), Quaternion.Euler(Vector3.zero));
                Destroy(hit.gameObject);
                StartCoroutine(onClickExtra());
                isTouched = true;
                break;
            case "play_btn":
                Instantiate(Game.Instance.playerExplosion, playerShip.transform.position, Quaternion.Euler(Vector3.zero));
                Game.Instance.shakeCamera();
                Destroy(playIcon);
                Destroy(playerShip);
                StartCoroutine(onClickPlay());
                isTouched = true;
                break;
        }
    }

    public override void touchReleased(Vector3 position) {
        GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, new Vector2(-1.25f, 0.2f), Quaternion.Euler(Vector2.zero));
        bullet.GetComponent<Bullet>().fire(null, position, 5, 500, true);
    }

    public override void touchDetected() {
        enabled = false;
    }

    IEnumerator onClickPlay() {
        yield return new WaitForSeconds(0.7f);
        Game.SURVIVAL_MODE = false;
        if (Game.FEATURES[Game.FEATURE_TYPE_TWO_TIMES_POWER] == Game.FEATURE_STATE_ACTIVE) {
            Game.PLAYER_POWER = 10000;
        } else {
            Game.PLAYER_POWER = 5000;
        }
        sceneLoader.loadScene("Levels");
    }

    IEnumerator onClickExtra() {
        yield return new WaitForSeconds(0.3f);
        sceneLoader.loadScene("Extra");
    }

    IEnumerator onClickSurvival() {
        yield return new WaitForSeconds(0.3f);
        Game.SURVIVAL_MODE = true;
        Game.PLAYER_POWER = 14000;
        sceneLoader.loadScene("NormalLevel");
    }

    IEnumerator onClickAbout() {
        yield return new WaitForSeconds(0.3f);
        sceneLoader.loadScene("About");
    }

    public override void back() {
        if (survivalMessage.activeSelf) {
            survivalMessage.SetActive(false);
            return;
        }
        if (allow_exit) {
            enabled = false;
            Application.Quit();
        } else {
            StartCoroutine(onClickBack());
        }
    }

    IEnumerator onClickBack() {
        allow_exit = true;
        Game.Instance.showMessage("Press back again to exit");
        yield return new WaitForSeconds(3.5f);
        allow_exit = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
