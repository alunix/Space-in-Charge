using UnityEngine;
using System.Collections;

public class Level9 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");

    public Level9()
        : base(false, 13) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_03);
    }

    public override void onTime() {
        if (time == 3) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_AMMO_LASER);
        }

        if (time > 5 && time % 5 == 0) {
            if (time == 10) {
                Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_GUARD);
            } else {
                Game.LEVEL_CONTROLLER.addFeatureBox();
            }
        } else if (time > 0 && time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        } else if (time > 0 && time % 4 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter == 0) {
            return;
        }

        if (counter % 6 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03, 3);
        } else if (counter % 3 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 2);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        }
    }

}
