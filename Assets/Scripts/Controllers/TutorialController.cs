using UnityEngine;
using System.Collections;

public class TutorialController : MenuController {
    [Header("General Settings")]
    public Transform player;
    public Animator player_animator;
    public Animator fire_animator;
    public SceneLoader sceneLoader;

    private bool pauseable = false;
    private bool paused = false;

    private Vector3 target_position = new Vector3(-2.06f, -0.26f, 0);

	void Start () {
        StartCoroutine(fire());
        StartCoroutine(setPauseable());
	}

    IEnumerator fire() {
        yield return new WaitForSeconds(2.1f);
        while (true) {
            if (!paused) {
                GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, player.position, Quaternion.Euler(Vector2.zero));
                bullet.GetComponent<Bullet>().fire(gameObject, target_position, 5, 100, true);
            }
            yield return new WaitForSeconds(1.4f);
        }
    }

    public override void onHit(Collider2D hit, ref bool isTouched) {
        if (!paused && hit.name.Equals("done_btn")) {
            isTouched = true;
            enabled = false;
            pauseable = false;
            Transform hitTransform = hit.transform;
            Instantiate(Game.Instance.boxCollected, hitTransform.position + new Vector3(-0.39f, 0.28f, 0), Quaternion.Euler(Vector3.zero));
            Destroy(hitTransform.parent.gameObject);
            StartCoroutine(displayScore());
        } else if (paused && hit.name.Equals("continue_btn")) {
            isTouched = true;
            resume();
        } else if (paused && hit.name.Equals("restart_btn")) {
            isTouched = true;
            resume();
        } else if (paused && hit.name.Equals("ceasefire_btn")) {
            isTouched = true;
            Time.timeScale = 1;
            loadScene("Levels");
        }
    }

    public override void touchReleased(Vector3 position) {
        if (paused) return;
        GameObject bullet = (GameObject)Instantiate(Game.Instance.bullet, player.position, Quaternion.Euler(Vector2.zero));
        bullet.GetComponent<Bullet>().fire(gameObject, position, 5, 100, true);
    }

    public override void touchDetected() { }

    public override void back() {
        if (sceneLoader.pausing || !pauseable) return;
        if (paused) {
            resume();
        } else {
            pause();
        }
    }

    private void pause() {
        paused = true;
        sceneLoader.pauseGame();
    }

    private void resume() {
        paused = false;
        Time.timeScale = 1;
        sceneLoader.displayScene();
    }

    IEnumerator displayScore() {
        yield return new WaitForSeconds(0.3f);
        Game.Instance.setCurrentLevel(1);
        Game.Instance.saveLevel(0, 4, false);
        loadScene("NormalLevel");
    }

    IEnumerator setPauseable() {
        yield return new WaitForSeconds(0.5f);
        pauseable = true;
    }

    private void loadScene(string scene_name) {
        pauseable = false;
        StopAllCoroutines();
        player_animator.Stop();
        fire_animator.Stop();
        sceneLoader.loadScene(scene_name, true);
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
