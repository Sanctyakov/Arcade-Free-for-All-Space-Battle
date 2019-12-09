using UnityEngine;

[System.Serializable]

public class Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
	public float speed, tilt, fireRate, rocketRate;
    public Boundary boundary;
    public GameObject explosion;
    public GameObject shot;
    public Transform shotSpawn;
    public AudioClip shotSFX;
    public GameObject rocket;
    public Transform rocketSpawn;
    public AudioClip rocketSFX;
    private Rigidbody rb;
    private int health;

    private bool playerFree;

    private float nextFire;

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
        gameController.OnHealthRestored += OnHealthRestored;

        rb = GetComponent<Rigidbody>();

        health = gameController.MaxHealth;

        gameController.UpdateHealth(health);

        playerFree = true;
    }


    void OnDisable()
    {
        gameController.OnGameFreeze -= OnGameFreeze;
        gameController.OnGameFree -= OnGameFree;
        gameController.OnHealthRestored -= OnHealthRestored;
    }

    void Update ()
	{
        if (!playerFree)
        {
            return;
        }

		if (Input.GetButton("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().PlayOneShot (shotSFX);
		}

        if (Input.GetButton("Fire2") && Time.time > nextFire && gameController.Rockets > 0)
        {
            nextFire = Time.time + rocketRate;
            Instantiate(rocket, shotSpawn.position, shotSpawn.rotation);
            GetComponent<AudioSource>().PlayOneShot(rocketSFX);
            gameController.RocketLaunched();
        }
    }

	void FixedUpdate ()
	{
        if (!playerFree)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.velocity = movement * speed;

        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }

    public void PlayerDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            PlayerDown();
        }

        gameController.UpdateHealth(health);
    }

    void OnHealthRestored()
    {
        health = gameController.MaxHealth;

        gameController.UpdateHealth(health);
    }

    void OnGameFreeze()
    {
        playerFree = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnGameFree()
    {
        playerFree = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    void PlayerDown()
    {
        gameController.PlayerDown();
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
