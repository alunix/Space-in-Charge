using UnityEngine;
using System.Collections;

public class AboutController : MenuController {
    [Header("General Settings")]
    public SceneLoader sceneLoader;

    void Start() {
        catchAll = false;
    }

    public override void onHit(Collider2D hit, ref bool isTouched) {
        switch (hit.name) {
            case "back_btn":
                Instantiate(Game.Instance.boxCollected, hit.transform.position + new Vector3(0.38f, 0.23f, 0), Quaternion.Euler(Vector3.zero));
                Destroy(hit.transform.parent.gameObject);

                StartCoroutine(onClickBack());
                break;
        }
    }

    public override void back() {
        enabled = false;
        sceneLoader.loadScene("MainMenu");
    }

    public override void touchReleased(Vector3 position) { }

    public override void touchDetected() { }

    IEnumerator onClickBack() {
        yield return new WaitForSeconds(0.3f);
        back();
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
