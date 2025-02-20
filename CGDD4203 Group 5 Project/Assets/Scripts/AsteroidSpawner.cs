
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] asteroidPrefabs;
    [SerializeField] int maxAsteroidCount;

    public bool spawnOnAwake = false;

    void Awake()
    {
        if (spawnOnAwake) SpawnAsteriods();
    }

    public void SpawnAsteriods()
    {
        //Place random initial asteroids
        for (int i = 0; i < maxAsteroidCount; i++)
        {
            //TODO: Random position in box again
            Vector3 spawnPosition = (new Vector3(Random.value, 0.5f, Random.value) * 2f) - Vector3.one;

            //Setup random rotation
            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 359f), 0);

            //Place Gameobject
            CreateAsteroid(spawnPosition * maxAsteroidCount, spawnRotation, asteroidPrefabs.Length - 1, 5);
        }
    }


    public void CreateAsteroid(Vector3 positionIn, Quaternion rotationIn, int asteroidSizeIn, float speedIn)
    {
        if (asteroidSizeIn < 0 || asteroidSizeIn >= asteroidPrefabs.Length)
        {
            return;
        }
        //Create GameObject
        GameObject asteroid = Instantiate(asteroidPrefabs[asteroidSizeIn], positionIn, rotationIn);
        //Set attributes
        AstroidController ac = asteroid.GetComponent<AstroidController>();
        ac.Size = asteroidSizeIn;
        ac.Speed = speedIn;
        ac.asteroidSpawner = this;
    }

}






