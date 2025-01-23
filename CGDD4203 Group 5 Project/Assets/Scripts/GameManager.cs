using UnityEngine;

public class GameManager : MonoBehaviour {
    //**PROPERTIES**       
    [Header("References")]
    [SerializeField] Transform[] walls;
    [SerializeField] GameObject[] asteroidPrefabs;
    //
    [Header("Game Settings")]
    [SerializeField] int levelWidth;
    [SerializeField] int levelHeight;
    [SerializeField] int asteroidNumber;
    [SerializeField] bool classicCamera;
    [SerializeField] bool updateDebug;
    [SerializeField] bool wallDebug;
    //
    Vector2 levelSize = Vector2.zero;

    //**FIELDS**    
    public Vector2 LevelSize { get => levelSize; }
    public bool ClassicCamera { get => classicCamera; set => classicCamera = value; }
    public bool UpdateDebug { get => updateDebug; set => updateDebug = value; }
    public bool WallDebug { get => wallDebug; set => wallDebug = value; }

    //**UNITY METHODS**
    private void Awake() {
        //Initialize
        levelSize = new Vector2(levelWidth, levelHeight);

        //Move walls into position
        walls[0].transform.position = new Vector3(0, 0, levelHeight / 2f);
        walls[1].transform.position = new Vector3(levelWidth / 2f, 0, 0);
        walls[2].transform.position = new Vector3(0, 0, -levelHeight / 2f);
        walls[3].transform.position = new Vector3(-levelWidth / 2f, 0, 0);

        //Place random initial asteroids
        for (int i = 0; i < asteroidNumber; i++) {

            //Setup random position
            Vector3 spawnPosition = new Vector3(Random.Range(-levelWidth / 2f, levelWidth / 2f), 0, Random.Range(-levelHeight / 2f, levelHeight / 2f));
            spawnPosition.x += spawnPosition.x > 0 ? -5 : 5;
            spawnPosition.z += spawnPosition.z > 0 ? -5 : 5;
            //Setup random rotation
            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 359f), 0);


            //Place Gameobject
            CreateAsteroid(spawnPosition, spawnRotation, 3, 5);
        }
    }

    public void CreateAsteroid(Vector3 positionIn, Quaternion rotationIn, int asteroidSizeIn, float speedIn) {

        for (int i = 0; i < asteroidPrefabs.Length; i++) {
            if (asteroidSizeIn - 1 == i) {
                //Create GameObject
                GameObject asteroid = Instantiate(asteroidPrefabs[i], positionIn, rotationIn);
                //Set attributes
                AstroidController ac = asteroid.GetComponent<AstroidController>();
                ac.Size = asteroidSizeIn;
                ac.Speed = speedIn;

                break;
            }
        }
    }
}


