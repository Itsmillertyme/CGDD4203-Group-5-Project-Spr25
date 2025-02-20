using System;
using UnityEngine;
using UnityEngine.Splines;


public class JourneyLeg : MonoBehaviour
{
    public String friendlyName;

    public SplineContainer splineContainer;
    public Collider startWaypoint;
    public Collider endWaypoint;

    float elapsedTime = 0f;
    bool timerActive = false;

    public float ElapsedTime { get => elapsedTime; private set => elapsedTime = value; }
    public bool TimerActive { get => timerActive; set => timerActive = value; }

    private void Update()
    {
        if (TimerActive)
        {
            ElapsedTime += Time.deltaTime;
        }
    }
}

