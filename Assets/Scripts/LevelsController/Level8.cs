using UnityEngine;
using System.Collections;

public class Level8 : BaseController {
    private GameObject duplicator = Resources.Load<GameObject>("Enemies/Duplicator");

    public Level8()
        : base(true, 120) {
    }

    public override void onTime() {
        if (time > 0 && time % 13 == 0) {
            Game.LEVEL_CONTROLLER.addMedikit();
        } else if (time > 0 && time % 5 == 0) {
            Game.LEVEL_CONTROLLER.addObject(duplicator);
        }
    }

}
