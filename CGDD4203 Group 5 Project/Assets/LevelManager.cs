using System;
using System.Linq;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{

    public JourneyLeg[] journeyLegs;

    public float totalElapsedLevelTime
    {
        get => journeyLegs.Sum(l => l.ElapsedTime);
    }

    public void StartNextLeg()
    {
        // TODO: Move the level back so the player is close to origin
        // TODO: Add a pretty transition cutscene to hide the last thing
        // TODO: Enable Objects in Next Leg, disable or destroy ones in current
        throw new NotImplementedException();
    }
}
