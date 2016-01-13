using UnityEngine;
using System.Collections;

public class Level1 : BaseController {
    private GameObject meteor = Resources.Load<GameObject>("Enemies/Meteor");

    public Level1()
        : base(true, 50) {
    }

    public override void onTime() {
        if (time == 49) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        } else if (time == 7) {
            Game.LEVEL_CONTROLLER.addObject(meteor);
        }

        if (time > 0 && time % 7 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        }
    }

}
