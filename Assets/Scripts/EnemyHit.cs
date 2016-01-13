using UnityEngine;
using System.Collections;

// Component of "Enemy -> Hit"
public class EnemyHit : MonoBehaviour {

    public int collisionPower = 150;
    private Enemy parent;

	void Start () {
        parent = transform.parent.GetComponent<Enemy>();
        if (!parent.isDuplicator) enabled = false;
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag.Equals(Game.PLAYER_TAG)) {
            if (Game.FEATURES[Game.FEATURE_TYPE_EASY_MODE] == Game.FEATURE_STATE_ACTIVE) {
                Game.player.changePower(-collisionPower / 2);
            } else {
                Game.player.changePower(-collisionPower);
            }
            parent.explode();
            if (LevelController.LEVEL_PAUSED) return;
            Game.Instance.shakeCamera();
        } else if (collider.tag.Equals(Game.ENEMY_HIT_TAG)) {
            parent.explode(true, true);
        } else if (collider.tag.Equals(Game.EXPLOSION_TAG)) {
            // Reduce the power of itself
            parent.changePower(Game.EXPLOSION_POWER);
        }
    }
}
