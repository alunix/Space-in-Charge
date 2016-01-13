﻿using UnityEngine;
using System.Collections;

public class Level6 : BaseController {
    private GameObject enemy_01 = Resources.Load<GameObject>("Enemies/Enemy_01");
    private GameObject enemy_02 = Resources.Load<GameObject>("Enemies/Enemy_02");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level6()
        : base(false, 13) {
            Game.LEVEL_CONTROLLER.addObject(enemy_02);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
            Game.LEVEL_CONTROLLER.addObject(enemy_01);
    }

    public override void onTime() {
        if (time == 3) {
            Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_EXPLODE_ENEMIES);
        } else if (time == 6) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time > 5 && time % 5 == 0) {
            if (time == 10) {
                Game.LEVEL_CONTROLLER.addFeatureBox(FeatureBox.TYPE_AMMO_AUTO);
            } else {
                Game.LEVEL_CONTROLLER.addFeatureBox();
            }
        } else if (time > 0 && time % 7 == 0) {
            if (time % 14 == 0) {
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
            Game.LEVEL_CONTROLLER.addObject(enemy_02, 3);
        } else {
            Game.LEVEL_CONTROLLER.addObject(enemy_01, 2);
        }
    }

}
