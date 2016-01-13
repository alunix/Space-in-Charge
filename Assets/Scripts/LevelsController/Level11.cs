using UnityEngine;
using System.Collections;

public class Level11 : BaseController {
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");

    public Level11()
        : base(true, 60) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
    }

    public override void onTime() {
        if (time == 59) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_AMMO_CLUSTER);
        }

        if (time % 11 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        }

        if (time % 3 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }

        if (time % 13 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03);
        } else if (time % 7 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
        } else if (time % 4 == 0) {
            Game.LEVEL_CONTROLLER.addMine();
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        if (counter % 6 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03, 3);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 2);
            Game.LEVEL_CONTROLLER.addMine();
        }
    }

}
