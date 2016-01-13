using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {
    [Header("General Settings")]
    public SpriteRenderer healthProgress;
    public SpriteRenderer ammoProgress;

    private float target_health;

    void Awake() {
        Game.STATUS_BAR = this;
    }

	void Start () {
        enabled = false;
	}
	
	void Update () {
        healthProgress.material.SetFloat("_Fill", Mathf.Lerp(
            healthProgress.material.GetFloat("_Fill"),
            target_health,
            0.1f
        ));

        if (healthProgress.material.GetFloat("_Fill") == target_health) {
            enabled = false;
        }
	}

    public void changeAmmo(int ammoAmount) {
        ammoProgress.material.SetFloat("_Fill", (float)ammoAmount / Game.PLAYER_AMMO);
    }

    public void changeHealth(float helath_value) {
        target_health = helath_value / Game.PLAYER_POWER;
        enabled = true;
    }

    void OnDestroy() {
        Game.STATUS_BAR = null;
    }
}
