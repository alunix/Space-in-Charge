﻿using UnityEngine;
using System.Collections;

public class Level3 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");

    public Level3()
        : base(false, 13) {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time > 0 && time % 5 == 0) {
            if (time % 10 == 0) {
                Game.LEVEL_CONTROLLER.addAmmoBox();
            } else {
                Game.LEVEL_CONTROLLER.addMedikit();
            }
        }
    }

    public override void enemyDefeated() {
        counter--;
        if (counter == 0) {
            return;
        }

        if (counter % 3 == 0) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
        }
    }

}
