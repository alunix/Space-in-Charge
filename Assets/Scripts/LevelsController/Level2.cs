using UnityEngine;
using System.Collections;

public class Level2 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");

    public Level2()
        : base(false, 11) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time > 0 && time % 16 == 0) {
            if (time % 32 == 0) {
                Game.LEVEL_CONTROLLER.addMedikit();
            } else {
                Game.LEVEL_CONTROLLER.addAmmoBox();
            }
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter == 0) {
            return;
        }
        Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

}
