using UnityEngine;

public class GameManager : MonoBehaviour {
    //**PROPERTIES**       
    [Header("References")]
    [SerializeField] Transform[] walls;
    [SerializeField] GameObject[] asteroidPrefabs;
    //
    Vector2 levelSize = Vector2.zero;

    //**FIELDS**
    [Header("Game Settings")]
    public bool classicCamera;
    public int levelWidth = 100;
    public int levelHeight = 56;
    public bool debugMode;
    public bool updateDebug;
    public bool triggerDebug;
    //
    public Vector2 LevelSize { get => levelSize; }

    //**UNITY METHODS**
    private void Awake() {
        //Initialize
        levelSize = new Vector2(levelWidth, levelHeight);

        //Move walls into position
        walls[0].transform.position = new Vector3(0, 0, levelHeight / 2f);
        walls[1].transform.position = new Vector3(levelWidth / 2f, 0, 0);
        walls[2].transform.position = new Vector3(0, 0, -levelHeight / 2f);
        walls[3].transform.position = new Vector3(-levelWidth / 2f, 0, 0);

        //Place random asteroids
        for (int i = 0; i < 2; i++) {

            //Setup random position
            Vector3 spawnPosition = new Vector3(Random.Range(-levelWidth / 2f, levelWidth / 2f), 0, Random.Range(-levelHeight / 2f, levelHeight / 2f));
            spawnPosition.x += spawnPosition.x > 0 ? -5 : 5;
            spawnPosition.z += spawnPosition.z > 0 ? -5 : 5;
            //Setup random rotation
            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 359f), 0);

            //Place Gameobject
            GameObject asteroid = Instantiate(asteroidPrefabs[asteroidPrefabs.Length - 1], spawnPosition, spawnRotation);
        }
    }
}


