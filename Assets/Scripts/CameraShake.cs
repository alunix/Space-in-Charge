using UnityEngine;
using System.Collections;

// Component of "Main Camera"
public class CameraShake : MonoBehaviour {
    private Vector3 originPosition;
    private Quaternion originRotation;
    private Transform _transform;

    private float shake_intensity = 0;

	// Use this for initialization
	void Start () {
        _transform = transform;
        originPosition = _transform.position;
        originRotation = _transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        if (shake_intensity > 0) {
            _transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            _transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                originRotation.z + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                originRotation.w + Random.Range(-shake_intensity, shake_intensity) * 0.2f
            );
            shake_intensity -= Time.deltaTime / 7;
            if (shake_intensity <= 0) {
                _transform.position = originPosition;
                _transform.rotation = originRotation;
            }
        } else {
            enabled = false;
        }
	}

    public void shake() {
        enabled = true;
        shake_intensity = 0.1f;
    }

    public void stopShake() {
        enabled = false;
        shake_intensity = 0;
        _transform.position = originPosition;
        _transform.rotation = originRotation;
    }
}
