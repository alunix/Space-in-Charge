using UnityEngine;
using System.Collections;

public class Level16 : BaseController {
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level16()
        : base(true, 70) {
    }

    public override void onTime() {
        if (time == 65) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time % 4 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        } else if (time % 21 == 0) {
            Game.LEVEL_CONTROLLER.addAmmoBox();
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
    }

}