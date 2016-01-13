using UnityEngine;
using System.Collections;

public class FeatureTitle : MonoBehaviour {
    [Header("General Settings")]
    public TextMesh title;
    public Animator animator;

	void Start () {
        enabled = false;
	}

    public void displayTitle(string text) {
        title.text = text;
        animator.Play("Feature_Title_Appear");
        StartCoroutine(destroy());
    }

    IEnumerator destroy() {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
