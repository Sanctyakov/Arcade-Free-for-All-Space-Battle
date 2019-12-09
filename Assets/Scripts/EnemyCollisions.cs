using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    public GameObject explosion;
    public GameObject coin;
    public int score;
    public int damage;
    private GameController gameController;
    private ShieldController shieldController;
    private PlayerController playerController;

    void Start()
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
    }

    void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy" || other.tag == "Collectible" || other.tag == "EnemyBolt")
		{
			return;
		}
        else if (other.tag == "Rocket")
        {
            EnemyDown();
        }
        else if (other.tag == "Shield")
        {
            shieldController = other.GetComponent<ShieldController>();
            shieldController.ShieldDamaged(damage);
            EnemyDown();
        }
        else if (other.tag == "PlayerBolt")
        {
            EnemyDown();
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            playerController = other.GetComponent<PlayerController>();
            playerController.PlayerDamaged(damage);
            EnemyDown();
        }	
	}

    void EnemyDown ()
    {
        gameController.UpdateScore(score);
        Instantiate(explosion, transform.position, transform.rotation);
        Instantiate(coin, transform.position, transform.rotation);
        gameController.EnemyDown();
        Destroy(gameObject);
    }
}