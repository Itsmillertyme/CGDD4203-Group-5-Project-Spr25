using UnityEngine;

public class CameraController : MonoBehaviour {
    Transform player;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        transform.position = player.position + new Vector3(0, 15, -3);
    }
}
