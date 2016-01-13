using UnityEngine;
using System.Collections;

public class Survival : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");

    public Survival()
        : base(false, 1) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
    }

    public override void onTime() {
        if (time == 20 || time == 100) Game.LEVEL_CONTROLLER.addObject(meteor);

        if (time > 0 && time % 25 == 0) Game.LEVEL_CONTROLLER.addObject(duplicator);

        if (time % 4 == 0) Game.LEVEL_CONTROLLER.addFeatureBox();
        else if (time % 5 == 0) Game.LEVEL_CONTROLLER.addMedikit();
        else if (time % 7 == 0) Game.LEVEL_CONTROLLER.addAmmoBox();
        else if (time % 18 == 0) Game.LEVEL_CONTROLLER.addMine();
    }

    public override void enemyDefeated() {
        counter++;

        if (counter % 4 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_04, 3);
        else if (counter % 3 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_03, 2);
        else if (counter % 2 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_02, 1);
        else Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

}
