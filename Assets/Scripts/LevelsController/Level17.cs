using UnityEngine;
using System.Collections;

public class Level17 : BaseController {
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level17()
        : base(false, 6) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04);
    }

    public override void onTime() {
        if (time == 10) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time % 8 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        } else if (time % 4 == 0) {
            Game.LEVEL_CONTROLLER.addFeatureBox();
        } else if (time % 21 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
        }

        if (time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addMine();
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        Game.LEVEL_CONTROLLER.addObject(enemy_04, 3);
    }

}