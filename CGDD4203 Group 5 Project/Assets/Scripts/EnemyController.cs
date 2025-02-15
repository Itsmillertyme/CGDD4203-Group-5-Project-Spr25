using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    //**PROPERTIES**
    [Header("Component References")]
    [SerializeField] GameObject player;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] Transform projectileParent;

    NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;
    [SerializeField] float SearchRadius;
    [SerializeField] float weaponCooldownRate;

    bool isReadyToFire = true;
    List<Vector3> waypoints;
    int currentWaypoint;

    //DEV ONLY - DEBUG RAY 
    float rotationSpeed = 90f; // degrees/s
    float currentAngle = 0f;

    //**UNITY METHODS****
    private void Awake() {
        //Cache references
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        projectileParent = GameObject.Find("EnemyProjectiles").transform;

        //Get waypoints
        waypoints = gameManager.GenerateEnemyWaypoints();

        //Setup Navmesh agent
        currentWaypoint = 0;
        agent.destination = waypoints[currentWaypoint];
    }
    //
    private void Update() {


        //Test if arrived at waypoint
        if (agent.remainingDistance <= agent.stoppingDistance) {
            if (currentWaypoint < waypoints.Count - 1) {
                // More waypoints left                
                currentWaypoint++;
                agent.destination = waypoints[currentWaypoint];
                Debug.Log($"Target waypoint {waypoints[currentWaypoint]}");
            }
            else {
                // Reached final waypoint (OFF MAP)
                Debug.Log("Destroying ship off map");
                Destroy(gameObject);
                return;
            }
        }


        //Detect player
        if (Vector3.Distance(transform.position, player.transform.position) < SearchRadius && isReadyToFire) {
            //fire projectile 
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.LookRotation(player.transform.position - transform.position), projectileParent);


            //Start cooldown
            isReadyToFire = false;
            StartCoroutine(WeaponCooldown());
        }


        //debug search radius
        if (gameManager.EnemyDebug) {
            //Draw search distance
            currentAngle += rotationSpeed * Time.deltaTime;
            Vector3 dir = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currentAngle * Mathf.Deg2Rad));
            Vector3 endPoint = transform.position + dir * SearchRadius;
            Debug.DrawRay(transform.position, dir * SearchRadius, Color.red);
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPoint);

            //Debug output
            string output = "ENEMY\n==============================\n";
            output += $"Stopping distance: {agent.stoppingDistance}\n";
            output += $"Remaining distance to target waypoint: {agent.remainingDistance}\n";
            output += "Waypoints:\n";
            for (int i = 0; i < waypoints.Count; i++) {
                output += i == currentWaypoint ? "\t>" : "\t ";

                output += waypoints[i] + "\n";
            }
            output += "==============================";


            Debug.Log(output);

        }
    }

    //**COROUTINES**
    IEnumerator WeaponCooldown() {
        yield return new WaitForSeconds(weaponCooldownRate);
        isReadyToFire = true;

    }
}