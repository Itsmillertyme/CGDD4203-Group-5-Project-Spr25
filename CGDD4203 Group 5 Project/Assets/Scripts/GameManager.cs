using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] bool devMode;

    public bool DevMode { get => devMode; }

    private void Awake() {

    }
}
