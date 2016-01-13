using UnityEngine;
using System.Collections;

public class Level21 : BaseController {
    private GameObject enemy_04 = Resources.Load<GameObject>("Enemies/Enemy_04");
    private GameObject enemy_03 = Resources.Load<GameObject>("Enemies/Enemy_03");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");

    public Level21()
        : base(false, 17) {
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

        if (counter % 4 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_04);
        else if (counter % 3 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_03);
        else if (counter % 2 == 0) Game.LEVEL_CONTROLLER.addObject(enemy_02);
        else Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

}