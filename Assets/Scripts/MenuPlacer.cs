using UnityEngine;
using System.Collections;

public class MenuPlacer : MonoBehaviour {

    public bool left = false;
    public bool right = false;
    public bool bottom = false;
    public bool top = false;
    public float margin = 0;
    public float z = -10;

    void Awake() {
    }

	// Use this for initialization
	void Start () {
        Vector3 position = new Vector3(0, 0, z);

        if (right) {
            position.x = -Game.Instance.world_left - margin;
        } else if (left) {
            position.x = Game.Instance.world_left + margin;
        }

        if (top) {
            position.y = -Game.Instance.world_bottom - margin;
        } else if (bottom) {
            position.y = Game.Instance.world_bottom + margin;
        }

        transform.position = position;

        enabled = false;
	}
}
