using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

    private Transform _transform;

	void Start () {
        _transform = transform;
        StartCoroutine(flash());
        enabled = false;
	}

    IEnumerator flash() {
        while (true) {
            _transform.position = new Vector3(Random.Range(Game.Instance.world_left, -Game.Instance.world_left), Random.Range(Game.Instance.world_bottom, -Game.Instance.world_bottom), 0);
            float scale = Random.Range(0.5f, 1f);
            _transform.localScale = new Vector3(scale, scale, 1);
            yield return new WaitForSeconds(1.1f);
        }
    }

    void OnDestroy() {
        StopAllCoroutines();
    }

}
