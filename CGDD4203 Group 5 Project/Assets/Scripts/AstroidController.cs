using UnityEngine;

public class AstroidController : MonoBehaviour {

    //**PROPERTIES**
    [SerializeField] float speed;
    int size; //1-3 atm    
    //
    GameManager gameManager;

    //**FIELDS**
    public float Speed { get => speed; set => speed = value; }
    public int Size { get => size; set => size = value; }

    //**UNITY METHODS**

    void Awake() {
        //Cache references
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    //
    void Update() {
        //Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    //
    void OnTriggerEnter(Collider other) {
        //Test if colliding with wall trigger
        if (other.gameObject.tag == "Wall") {
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
            //Debug.Log($"Asteroid flipped location: {transform.position}");
        }
        //Test if colliding with player projectile
        else if (other.gameObject.tag == "PlayerProjectile") {
            //Destroy projectile
            Destroy(other.gameObject);

            //Split into smaller asteroids
            if (size != 1) {
                Split(2);
            }
            else {
                Destroy(gameObject);
            }

            //Update player score
            GameObject.FindWithTag("Player").GetComponent<ShipController>().updateScore(Size);

        }
    }

    //**UTILITY METHODS**
    public void Split(int numberOfDebris) {

        //Helpers
        Vector3[] newAsteroidPositions = new Vector3[numberOfDebris];
        Quaternion[] newAsteroidRotations = new Quaternion[numberOfDebris];
        int newSize = size - 1;
        float distance = (newSize * 2 + 1) * (2 / 3); //rough and dirty
        float spread = 45;//in degrees
        float offsetGap = 10; //in degrees
        float angleStep = 360 / numberOfDebris;

        //Debug.Log($"Current rotation: {transform.rotation.y}");

        //Setup and spawn new debris asteroids
        for (int i = 0; i < numberOfDebris; i++) {
            //get spawns, equally spread out radially from current asteroid position a new radius length
            float currentAngleRadians = 90 + (angleStep * i) * Mathf.Deg2Rad;
            newAsteroidPositions[i] = new Vector3(transform.position.x + distance * Mathf.Cos(currentAngleRadians), 0, transform.position.z + distance * Mathf.Sin(currentAngleRadians));

            //get rotations
            Quaternion rot;
            if (Random.value > 0.5f) {
                rot = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Random.Range(offsetGap / 2, spread / 2 + offsetGap / 2), 0);
            }
            else {
                rot = Quaternion.Euler(0, transform.rotation.eulerAngles.y - Random.Range(offsetGap / 2, spread / 2 + offsetGap / 2), 0);
            }

            //Debug.Log($"New rotation: {rot.eulerAngles}");
            newAsteroidRotations[i] = rot;

            //Spawn in GameObject
            gameManager.CreateAsteroid(newAsteroidPositions[i], newAsteroidRotations[i], newSize, speed * 1.2f);
        }

        //Debug.Break();

        //destroy this one
        Destroy(gameObject);
    }

}

