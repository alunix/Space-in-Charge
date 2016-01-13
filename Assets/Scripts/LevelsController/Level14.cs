using UnityEngine;
using System.Collections;

public class Level14 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level14()
        : base(false, 10) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time > 0 && time % 10 == 0) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time == 3) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_EXPLODE_ENEMIES);
        } else if (time == 7) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_GUARD);
        }

        if (time % 11 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        } else if (time % 6 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }

        if (time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addMine();
        }
    }

    public override void enemyDefeated() {
        counter--;
        Debug.Log(counter);
        if (counter <= 0) {
            return;
        }

        if (counter % 2 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03, 3);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 2);
        }
    }

}
