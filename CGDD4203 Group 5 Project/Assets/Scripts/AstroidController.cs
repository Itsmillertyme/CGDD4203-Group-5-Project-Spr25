using UnityEngine;

public class AstroidController : MonoBehaviour {

    //**PROPERTIES**
    [SerializeField] float speed;

    //**FIELDS**
    public float Speed { get => speed; set => speed = value; }

    //**UNITY METHODS**
    void Update() {
        //Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    //
    private void OnTriggerEnter(Collider other) {
        //Test if colliding with wall trigger
        if (other.gameObject.tag == "Wall") {
            //inTrigger = true;

            //Debug.Log("Asteroid wall trigger");

            //Debug.Log($"Asteroid entering location: {transform.position}");
            //Test wall orientation
            if (other.transform.localScale.x > other.transform.localScale.z) {
                //Flip z coord
                transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z + (transform.position.z < 0 ? -0.1f : 0.1f));
            }
            else {
                //Flip x coord
                transform.position = new Vector3(-transform.position.x + (transform.position.x < 0 ? -0.1f : 0.1f), transform.position.y, transform.position.z);
            }

        }
        //Debug.Log($"Asteroid flipped location: {transform.position}");
    }

    //**UTILITY METHODS**
    public void Split() {
        Destroy(gameObject);
    }
}

