using UnityEngine;
using System.Collections;

public class Backgroung : MonoBehaviour {

    private MeshRenderer _renderer;

	void Start () {
        int ratio = Mathf.CeilToInt(Game.Instance.mainCamera.aspect);
        transform.localScale = new Vector3(ratio * Game.Instance.world_height, Game.Instance.world_height, 1);
        
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTextureScale = new Vector2(ratio, 1);
        _renderer.sortingLayerName = "Background Image";
        _renderer.sortingOrder = 0;
        
        enabled = false;
	}

}
