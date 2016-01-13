using UnityEngine;
using System.Collections;

public class Level18 : BaseController {
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level18()
        : base(false, 13) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04);
            Game.LEVEL_CONTROLLER.addObject(enemy_03);
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time == 7) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_GUARD);
        } else if (time == 4) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time > 0 && time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        } else if (time % 4 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 10 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        if (counter % 4 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04, 3);
        } else if (counter % 3 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03, 2);
        } else if (counter % 2 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 1);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        }
    }

}