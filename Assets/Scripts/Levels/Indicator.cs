using UnityEngine;
using System.Collections;

public class Indicator : MonoBehaviour {
    private int TOTAL; // starts from 0
    private const float TRANSITION_TIME = 0.5f; // Seconds
    public const float HOLDER_DISTANCE = 7;
    private const float INDICATOR_DISTANCE = 0.2f;

    [Header("General Settings")]
    public Transform indicator;
    public Animator animator;
    public Transform levels;
    public GameObject[] levels_array;

    private int current = 0;
    private int last_current = 0;
    private bool animate = false;
    private bool drag_mode = false;
    private float indicatorTargetPosition;
    private float targetPosition;
    private float deltaPosition;
    private float transitionTime;

	void Start () {
        enabled = false;
        TOTAL = levels_array.Length - 1;
        setCurrent(Game.CURRENT_LEVEL / 8);
	}

    void Update() {
        if (animate) {
            if (transitionTime > 0) {
                transitionTime -= Time.deltaTime;
                if (transitionTime <= 0) {
                    transitionTime = 0;
                    animateHolders();
                    if (drag_mode) {
                        hideOtherLevels();
                    } else {
                        levels_array[last_current].SetActive(false);
                    }
                    enabled = false;
                } else {
                    animateHolders();
                }
            }
        } else {
            transitionTime = 0;

            indicator.localPosition = new Vector2(INDICATOR_DISTANCE * current, 0);

            if (last_current != current) {
                levels_array[last_current].SetActive(false);
            }

            levels.position = new Vector2(-current * HOLDER_DISTANCE, 0);

            enabled = false;
        }
    }

    private void animateHolders() {
        if (transitionTime == 0) {
            levels.position = new Vector2(targetPosition, 0);
            indicator.localPosition = new Vector2(indicatorTargetPosition, 0);
        } else {
            levels.position = Vector2.MoveTowards(
                levels.position,
                new Vector2(targetPosition, 0),
                HOLDER_DISTANCE * Time.deltaTime / TRANSITION_TIME
            );

            indicator.localPosition = Vector2.MoveTowards(
                indicator.localPosition,
                new Vector2(indicatorTargetPosition, 0),
                INDICATOR_DISTANCE * Time.deltaTime / TRANSITION_TIME
            );
        }
    }

    public void hideOtherLevels() {
        for (int i = 0; i <= TOTAL; i++) {
            if (i != current) levels_array[i].SetActive(false);
        }
    }

    public void setCurrent(int current, bool animate = false) {
        last_current = this.current;
        
        this.current = current;
        this.animate = animate;
        drag_mode = false;

        targetPosition = -current * HOLDER_DISTANCE;
        deltaPosition = Mathf.Abs(targetPosition - levels.position.x);
        transitionTime = deltaPosition * TRANSITION_TIME / HOLDER_DISTANCE;

        indicatorTargetPosition = current * INDICATOR_DISTANCE;

        levels_array[current].SetActive(true);

        enabled = true;

        if (animate) {
            animator.Play("scale", -1, 0);
        }
    }

    public void next() {
        if (current < TOTAL) setCurrent(current + 1, true);
    }

    public void previous() {
        if (current > 0) setCurrent(current - 1, true);
    }

    public void activateNeighbours(float position) {
        if (position < 0) {
            int current_in_drag = (int) Mathf.Clamp((Mathf.Abs(position) / HOLDER_DISTANCE), 0, TOTAL);
            if (!levels_array[current_in_drag].activeSelf) levels_array[current_in_drag].SetActive(true);
            if (current_in_drag > 0 && !levels_array[current_in_drag - 1].activeSelf) levels_array[current_in_drag - 1].SetActive(true);
            if (current_in_drag < TOTAL && !levels_array[current_in_drag + 1].activeSelf) levels_array[current_in_drag + 1].SetActive(true);
        }
    }

    public void animateAfterDrag(int swipe = 0) {
        float positionX = levels.position.x;

        if (swipe == 0) {
            if (positionX > 0) current = 0;
            else current = (int)Mathf.Clamp(Mathf.Abs(Mathf.Round(positionX / HOLDER_DISTANCE)), 0, TOTAL);
        } else {
            current = Mathf.Clamp(current + swipe, 0, TOTAL);
            levels_array[current].SetActive(true);
        }

        animate = true;
        drag_mode = true;

        targetPosition = -current * HOLDER_DISTANCE;
        deltaPosition = Mathf.Abs(targetPosition - positionX);

        transitionTime = deltaPosition * TRANSITION_TIME / HOLDER_DISTANCE;

        indicatorTargetPosition = current * INDICATOR_DISTANCE;

        enabled = true;
        animator.Play("scale", -1, 0);
    }
}
