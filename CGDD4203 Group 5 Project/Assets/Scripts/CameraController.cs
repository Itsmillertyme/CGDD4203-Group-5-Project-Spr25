using UnityEngine;

public class CameraController : MonoBehaviour {
    //**PROPERTIES**
    [SerializeField] GameManager gameManager;
    //
    Transform player;

    //**UNITY METHODS**
    void Awake() {
        //Cache references
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    //
    void Update() {
        //Change camera view
        if (gameManager.ClassicCamera) {
            transform.position = new Vector3(0, 50, 0);
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else {
            transform.position = player.position + new Vector3(0, 70, 0);
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
