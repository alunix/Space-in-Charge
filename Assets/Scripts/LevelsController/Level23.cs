using UnityEngine;
using System.Collections;

public class Level23 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");

    public Level23()
        : base(false, 18) {
            Game.LEVEL_CONTROLLER.addObject(enemy_04);
    }

    public override void onTime() {
        if (time % 4 == 0) Game.LEVEL_CONTROLLER.addFeatureBox();
        else if (time % 5 == 0) Game.LEVEL_CONTROLLER.addMedikit();
        else if (time % 10 == 0) Game.LEVEL_CONTROLLER.addAmmoBox();
        else if (time % 13 == 0) Game.LEVEL_CONTROLLER.addMine();
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }

        if (counter % 4 == 0) {
            Game.LEVEL_CONTROLLER.addObject(meteor, 3);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        } else if (counter % 3 == 0) {
            Game.LEVEL_CONTROLLER.addObject(duplicator, 2);
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
        } else if (counter % 2 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_04);
        else Game.LEVEL_CONTROLLER.addObject(enemy_03);
    }

}