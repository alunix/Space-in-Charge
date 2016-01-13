using UnityEngine;
using System.Collections;

public class Level12 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level12()
        : base(false, 13) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time == 10) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time % 11 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        } else if (time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        if (counter % 4 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03, 2);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        }
    }

}
