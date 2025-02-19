using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    //**PROPERTIES**       
    [Header("References")]
    [SerializeField] Transform[] walls;
    [SerializeField] GameObject[] asteroidPrefabs;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemyProjectileParent;
    public AsteroidSpawner asteroidSpawner;
    //
    [Header("Game Settings")]
    [SerializeField] int levelWidth;
    [SerializeField] int levelHeight;
    [SerializeField] bool shipDebug;
    [SerializeField] bool wallDebug;
    [SerializeField] bool enemyDebug;
    //
    Vector2 levelSize = Vector2.zero;

    //**PROPERTIES**    
    public Vector2 LevelSize { get => levelSize; }
    public bool ShipDebug { get => shipDebug; set => shipDebug = value; }
    public bool WallDebug { get => wallDebug; set => wallDebug = value; }
    public bool EnemyDebug { get => enemyDebug; set => enemyDebug = value; }

    //**UNITY METHODS**
    private void Awake()
    {
        //Initialize
        levelSize = new Vector2(levelWidth, levelHeight);

        //Move walls into position
        walls[0].transform.position = new Vector3(0, 0, levelHeight / 2f);
        walls[1].transform.position = new Vector3(levelWidth / 2f, 0, 0);
        walls[2].transform.position = new Vector3(0, 0, -levelHeight / 2f);
        walls[3].transform.position = new Vector3(-levelWidth / 2f, 0, 0);

        current = this;
    }

    //**UTILITY METHODS**

    //
    public List<Vector3> GenerateEnemyWaypoints()
    {
        List<Vector3> waypoints = new List<Vector3>();

        //Get number of waypoints to add
        int numWaypoints = Random.Range(3, 6); //3-5 waypoints

        //Generate each random one
        for (int i = 0; i < numWaypoints; i++)
        {
            float xCoord = Random.Range(levelWidth / -2f, levelWidth / 2f + 1);
            float zCoord = Random.Range(levelHeight / -2f, levelHeight / 2f + 1);
            waypoints.Add(new Vector3(xCoord, 0, zCoord));
        }

        //Make last waypoint way offscreen
        waypoints.Add(new Vector3(Random.Range(levelWidth / 2f + 5, 75), 0, Random.Range(-75, levelHeight / -2f - 5)));

        return waypoints;
    }
    //
    public void SpawnEnemy()
    {
        //Spawn
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-74, levelWidth / -2f - 3), 0, Random.Range(levelHeight / 2f + 3, 74)), Quaternion.Euler(0, 0, 0));
    }
}





