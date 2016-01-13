using UnityEngine;
using System.Collections;

public class Level7 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level7()
        : base(false, 17) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
            Game.LEVEL_CONTROLLER.addObject(meteor);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time == 3) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_GUARD);
        }

        if (time > 5 && time % 5 == 0) {
            if (time == 10) {
                Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_MAD_ENEMY);
            } else {
                Game.LEVEL_CONTROLLER.addFeatureBox();
            }
        } else if (time > 0 && time % 7 == 0) {
            if (time % 14 == 0) {
                Game.LEVEL_CONTROLLER.addAmmoBox();
            } else {
                Game.LEVEL_CONTROLLER.addMedikit();
            }
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter == 0) {
            return;
        }

        if (counter % 3 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 1);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01, 2);
        }
    }

}
