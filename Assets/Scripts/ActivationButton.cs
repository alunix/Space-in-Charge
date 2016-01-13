using UnityEngine;
using System.Collections;

public class ActivationButton : MonoBehaviour {
    [Header("General Settings")]
    public int id;
    public int pts;
    public SpriteRenderer image;

	void Start () {
        enabled = false;
        resetImage();
	}

    public void resetImage() {
        switch (Game.FEATURES[id]) {
            case Game.FEATURE_STATE_LOCK:
                image.sprite = Game.EXTRA_CONTROLLER.activate_png;
                break;
            case Game.FEATURE_STATE_ACTIVE:
                image.sprite = Game.EXTRA_CONTROLLER.disable_png;
                break;
            case Game.FEATURE_STATE_DISABLE:
                image.sprite = Game.EXTRA_CONTROLLER.enable_png;
                break;
        }
    }

    public void activate(bool using_points) {
        if (using_points) {
            Game.Instance.points -= pts;
            Game.FEATURES[id] = Game.FEATURE_STATE_ACTIVE;
        } else {
            Game.FEATURES[id] = Game.FEATURE_STATE_ACTIVE;
        }

        Game.Instance.saveFeatures(using_points);
    }

    public void enable() {
        Game.FEATURES[id] = Game.FEATURE_STATE_ACTIVE;
        image.sprite = Game.EXTRA_CONTROLLER.disable_png;
        Game.Instance.saveFeatures(false);
    }

    public void disable() {
        Game.FEATURES[id] = Game.FEATURE_STATE_DISABLE;
        image.sprite = Game.EXTRA_CONTROLLER.enable_png;
        Game.Instance.saveFeatures(false);
    }

}
