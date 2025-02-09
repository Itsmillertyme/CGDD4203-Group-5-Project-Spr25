using UnityEngine;

public class EnemyProjectileController : MonoBehaviour {
    //**PROPERTIES**
    [SerializeField] int damage;
    [SerializeField] float speed;

    //**UNITY METHODS**
    void Update() {
        //Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    //
    private void OnTriggerEnter(Collider other) {

        //Impact player ship
        if (other.gameObject.tag == "Player") {
            ShipController ship = other.gameObject.GetComponent<ShipController>();
            if (!ship.IsInvulnerable) {
                ship.UpdateHealth(-damage);
            }
        }
    }
}
