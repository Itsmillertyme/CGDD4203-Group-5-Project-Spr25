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
}
