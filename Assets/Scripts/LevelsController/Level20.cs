using UnityEngine;
using System.Collections;

public class Level20 : BaseController {
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level20()
        : base(true, 60) {
    }

    public override void onTime() {
        if (time % 10 == 0) Game.LEVEL_CONTROLLER.addObject(meteor);
        if (time % 5 == 0) Game.LEVEL_CONTROLLER.addMedikit();
        if (time % 21 == 0) Game.LEVEL_CONTROLLER.addAmmoBox();
    }

    public override void enemyDefeated() {
        counter--;
        if (counter <= 0) {
            return;
        }
    }

}