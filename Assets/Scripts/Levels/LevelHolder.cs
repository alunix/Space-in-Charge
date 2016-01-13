using UnityEngine;
using System.Collections;

public class LevelHolder : MonoBehaviour {
    [Header("General Settings")]
    public SpriteRenderer image;
    public GameObject lockObject;
    public GameObject playObject;
    public GameObject gold1;
    public GameObject gold2;
    public GameObject gold3;

    private const float GOLD_Y = 0;

	void Start () {
        enabled = false;
        setStatus();
	}

    public void setStatus() {
        int status = Game.LEVELS_STATUS[int.Parse(name)];

        switch (status) {
            case 0:
                image.sprite = Game.LEVELS_CONTROLLER.locked;
                playObject.SetActive(false);
                lockObject.SetActive(true);
                break;
            case 1:
                image.sprite = Game.LEVELS_CONTROLLER.locked;
                playObject.SetActive(true);
                lockObject.SetActive(false);
                tag = Game.LEVEL_TAG;
                break;
            case 2:
                image.sprite = Game.LEVELS_CONTROLLER.done;
                playObject.SetActive(false);
                lockObject.SetActive(false);
                gold1.SetActive(true);
                gold1.transform.localPosition = new Vector2(0, GOLD_Y);
                gold2.SetActive(false);
                gold3.SetActive(false);
                tag = Game.LEVEL_TAG;
                break;
            case 3:
                image.sprite = Game.LEVELS_CONTROLLER.done;
                playObject.SetActive(false);
                lockObject.SetActive(false);
                gold1.SetActive(true);
                gold1.transform.localPosition = new Vector2(0.15f, GOLD_Y);
                gold2.SetActive(true);
                gold2.transform.localPosition = new Vector2(-0.15f, GOLD_Y);
                gold3.SetActive(false);
                tag = Game.LEVEL_TAG;
                break;
            case 4:
                image.sprite = Game.LEVELS_CONTROLLER.done;
                playObject.SetActive(false);
                lockObject.SetActive(false);
                gold1.SetActive(true);
                gold1.transform.localPosition = new Vector2(0, GOLD_Y);
                gold2.SetActive(true);
                gold2.transform.localPosition = new Vector2(-0.3f, GOLD_Y);
                gold3.SetActive(true);
                gold3.transform.localPosition = new Vector2(0.3f, GOLD_Y);
                tag = Game.LEVEL_TAG;
                break;
        }
    }
}
