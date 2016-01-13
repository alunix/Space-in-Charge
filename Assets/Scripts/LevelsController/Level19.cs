using UnityEngine;
using System.Collections;

public class Level19 : BaseController {
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level19()
        : base(true, 75) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addObject(duplicator);
            Game.LEVEL_CONTROLLER.addMedikit();
        } else if (time % 21 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04);
        } else if (time % 13 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        }

        if (time == 30) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time % 7 == 0) {
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
    }

}