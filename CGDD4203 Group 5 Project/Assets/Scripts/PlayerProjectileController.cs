using UnityEngine;

public class PlayerProjectileController : MonoBehaviour {
    [SerializeField] float speed;
    float initialSpeed;

    private void Awake() {

        Debug.Log(transform.position);

    }

    void Update() {
        transform.Translate(Vector3.forward * (initialSpeed + speed) * Time.deltaTime);
    }

    public void SetInitialSpeed(float initSpeedIn) {
        initialSpeed = initSpeedIn;
    }
}
