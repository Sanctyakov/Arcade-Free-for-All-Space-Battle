using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

    private GameController gameController;

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

    void Start ()
	{
		InvokeRepeating ("Fire", delay, fireRate);
	}

	void Fire ()
	{
		Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		GetComponent<AudioSource>().Play();
	}

    void OnGameFreeze()
    {
        CancelInvoke("Fire");
    }

    void OnGameFree()
    {
        InvokeRepeating("Fire", delay, fireRate);
    }
}
