using UnityEngine;
using System.Collections;

public class Level22 : BaseController {
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level22()
        : base(true, 90) {
    }

    public override void onTime() {
        if (time % 4 == 0) Game.LEVEL_CONTROLLER.addFeatureBox();
        else if (time % 5 == 0) Game.LEVEL_CONTROLLER.addMedikit();
        else if (time % 10 == 0) Game.LEVEL_CONTROLLER.addAmmoBox();
        else if (time % 13 == 0) Game.LEVEL_CONTROLLER.addMine();

        if (time % 6 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_01);
        else if (time % 15 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_02);
        else if (time % 22 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_03);
        else if (time % 32 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_04);

        if (time == 80) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }
    }

}