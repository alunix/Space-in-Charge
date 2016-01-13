using UnityEngine;
using System.Collections;

public class Level13 : BaseController {
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level13()
        : base(false, 4) {
            Game.LEVEL_CONTROLLER.addObject(enemy_03);
    }

    public override void onTime() {
        if (time == 5) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time % 11 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        } else if (time % 6 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }

        if (time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addObject(duplicator);
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        Game.LEVEL_CONTROLLER.addObject(enemy_03, 3);
    }

}
