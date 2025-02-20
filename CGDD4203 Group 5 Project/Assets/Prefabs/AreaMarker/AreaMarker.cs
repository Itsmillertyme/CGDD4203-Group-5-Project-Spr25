using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public class AreaMarker : MonoBehaviour
{
    static PriorityQueue<AreaMarker, int> waitingAreaMarkers = new PriorityQueue<AreaMarker, int>();
    [Header("Configuration")]
    public int priority = 0;
    public UnityEvent OnAreaEntered;
    public UnityEvent OnPopupHidden;
    public float secondsToShowFor = 5f;

    [Header("Internal Prefab References")]
    [SerializeField] GameObject ui;

    void Start()
    {
        ui.SetActive(false);
    }

    void OnTriggerEnter(Collider _) // Collider is unused, use collision layers to limit what can collide with this (significantly more performant)
    {
        OnAreaEntered.Invoke();
        waitingAreaMarkers.Enqueue(this, priority);
        // if (waitingAreaMarkers.Peek() != this)
        // {
        //     waitingAreaMarkers.Dequeue().OnPopupHidden.AddListener(ShowPopup);
        // }
        // else
        // {
        //     ShowPopup();
        // }

        // If this areaMarker is first in line, just show it
        if (waitingAreaMarkers.Count == 1 && waitingAreaMarkers.Peek() == this)
        {
            waitingAreaMarkers.Dequeue().ShowPopup(); // This probably has too many checks, I'm being too careful.
        }
    }

    public void ShowPopup()
    {
        StartCoroutine(ShowPopupAnimation());
    }

    IEnumerator ShowPopupAnimation()
    {
        // Not adding this yet, just incase it breaks things
        // if (visibleAreaMarkers.Peek() != this)
        // { yield break; }

        // TODO: Add an actual animation
        ui.SetActive(true);

        yield return new WaitForSeconds(secondsToShowFor);

        ui.SetActive(false);
        OnPopupHidden.Invoke();
        if (waitingAreaMarkers.Count != 0)
        {
            waitingAreaMarkers.Dequeue().ShowPopup();
        }
    }
}
