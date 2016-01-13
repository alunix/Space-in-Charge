using UnityEngine;
using System.Collections;

abstract public class MenuController : MonoBehaviour {

    public bool catchAll = true;

	void Update() {
        #if UNITY_ANDROID
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    onTouch(Game.Instance.mainCamera.ScreenToWorldPoint(touch.position));
                }
            }
        }
        #else
        if (Input.GetMouseButtonDown(0)) {
            onTouch(Game.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        #endif

        if (Input.GetKeyDown(KeyCode.Escape)) {
            back();
        }
    }

    private void onTouch(Vector3 position) {
        if (catchAll) {
            Collider2D[] hits = Physics2D.OverlapPointAll(position);
            if (hits != null) {
                bool isTouched = false;
                foreach (Collider2D hit in hits) {
                    onHit(hit, ref isTouched);
                }
                if (isTouched) touchDetected();
                else touchReleased(position);
            }
        } else {
            Collider2D hit = Physics2D.OverlapPoint(position);
            if (hit != null) {
                bool isTouched = false;
                onHit(hit, ref isTouched);
                if (isTouched) touchDetected();
                else touchReleased(position);
            } else {
                touchReleased(position);
            }
        }
    }

    abstract public void onHit(Collider2D hit, ref bool isTouched);
    abstract public void touchDetected();
    abstract public void touchReleased(Vector3 position);
    abstract public void back();
}
