using UnityEngine;
using System.Collections;

public class BaseController {
    public int time = 0; // Seconds
    public int counter;

    private bool isCountDown = false;
    private System.TimeSpan timespan;

    public BaseController(bool isCountDown, int time_counter) {
        this.isCountDown = isCountDown;
        if (isCountDown) {
            this.time = time_counter;
            this.counter = 0;
        } else {
            this.counter = time_counter;
            this.time = 0;
        }

        Game.LEVEL_CONTROLLER.StartCoroutine(start());
    }

    public virtual void onTime() {
    }

    public virtual void enemyDefeated() {
    }

    IEnumerator start() {
        while (true) {
            timespan = System.TimeSpan.FromSeconds(time);
            Game.LEVEL_CONTROLLER.time_txt.text = string.Format(
                "{0:D2}:{1:D2}",
                (int) timespan.TotalMinutes,
                timespan.Seconds
            );

            if (!Game.SURVIVAL_MODE) {
                if (isCountDown && time == 0) {
                    Game.LEVEL_CONTROLLER.clearLevel();
                    break;
                } else if (!isCountDown && counter <= 0) {
                    Game.LEVEL_CONTROLLER.clearLevel();
                    break;
                }
            }

            onTime();
            yield return new WaitForSeconds(1);
            if (isCountDown) time--;
            else time++;
        }
    }
}
