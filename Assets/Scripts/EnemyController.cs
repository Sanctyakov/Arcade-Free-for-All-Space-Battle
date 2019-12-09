using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public Boundary boundary;
	public float tilt, dodge, smoothing;
	public Vector2 startWait, maneuverTime, maneuverWait;
	private float currentSpeed, targetManeuver;
    private Vector3 linearBackup, angularBackup;
    private bool enemyFree;

    private GameController gameController;
    private GameObject player;

    void OnEnable()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }

        gameController.OnGameFreeze += OnGameFreeze;
        gameController.OnGameFree += OnGameFree;
    }


    void OnDisable()
    {
        gameController.OnGameFreeze -= OnGameFreeze;
        gameController.OnGameFree -= OnGameFree;
    }

    void Start()
    {
        enemyFree = true;

        currentSpeed = GetComponent<Rigidbody>().velocity.z;

        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	void FixedUpdate ()
	{
        if (!enemyFree)
        {
            return;
        }

        if (player != null)
        {
            if (player.transform.position.z < transform.position.z)
            {
                targetManeuver = player.transform.position.x;
            }
            else
            {
                targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
            }
        }
        else
        {
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
        }

        float newManeuver = Mathf.MoveTowards(GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);

        GetComponent<Rigidbody>().velocity = new Vector3 (newManeuver, 0.0f, currentSpeed);

		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);

        GetComponent<Rigidbody>().rotation = Quaternion.Euler (0, 0, GetComponent<Rigidbody>().velocity.x * -tilt);
	}

    void OnGameFreeze()
    {
        enemyFree = false;
        linearBackup = GetComponent<Rigidbody>().velocity;
        angularBackup = GetComponent<Rigidbody>().angularVelocity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        
    }

    void OnGameFree()
    {
        enemyFree = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().velocity = linearBackup;
        GetComponent<Rigidbody>().angularVelocity = angularBackup;
    }
}