using UnityEngine;

public class Ship : MonoBehaviour
{
    public float score; //The score each ship is worth.
    public float health, tilt, speed, collisionDamage; //References set in inspector.
    public Boundary boundary; //Ships cannot exceed screen borders.
    public GameObject explosion; //The explosion that the ship will instantiate upon death.
    public GameObject coin; //Enemy ships drop coins upon death.
    public Rigidbody rb;
    public AudioClip explosionSFX;

    public bool free; //Ships are free to move according to Game State.

    private bool player; //In the inspector, player ships hold no reference to coins and have a score value of 0.

    private Vector3 linearBackup, angularBackup; //Backup vectors for pausing and resuming.

    void Start()
    {
        GameControl.OnGameFreeze += OnGameFreeze;
        GameControl.OnGameFree += OnGameFree;

        if (GetComponent<Player>() == null)
        {
            player = false;
        }
        else //If the ship belongs to a player:
        {
            player = true;

            health = GameControl.MaxHealth; //Players' health will be set by the Game Controller.

            GameControl.UpdateHealth(health); //Display the player's health.
        }
    }

    public void OnDestroy()
    {
        GameControl.OnGameFreeze -= OnGameFreeze;
        GameControl.OnGameFree -= OnGameFree;
    }

    public void Move(Vector3 movement) //All ships will move within the game's boundaries, and tilt sideways.
    {
        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );

        rb.velocity = movement * speed;
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }

    void OnGameFreeze()
    {
        free = false;
        linearBackup = rb.velocity; //Store the ship's current position and rotation.
        angularBackup = rb.angularVelocity;
        rb.velocity = Vector3.zero; //Freeze the ship.
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnGameFree()
    {
        free = true;
        rb.constraints = RigidbodyConstraints.None; //Unfreeze the ship.
        rb.velocity = linearBackup; //Restore velocity as was backed up.
        rb.angularVelocity = angularBackup;
    }

    public void Damaged(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Down(); //The ship is destroyed.
        }

        if (player)
        {
            GameControl.UpdateHealth(health); //Inform Game Control of the damage.
        }
    }

    void Down()
    {
        Instantiate(explosion, transform.position, transform.rotation);

        GameControl.PlayClip(explosionSFX);

        if (player)
        {
            GameControl.PlayerDown();
        }
        else
        {
            GameControl.EnemyDown(score); //The game records the score the destroyed ship was worth.
            Instantiate(coin, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (player) //If a player ship collides with another.
        {
            if (other.tag == "Boundary" || other.tag == "Shield")
            {
                return; //Player ships cannot touch the boundary, but just in case.
            }
            else if (other.tag == "Collectible") //Player ships collect coins.
            {
                GameControl.AddCoin();
                Destroy(other.gameObject);
            }
            else if (other.tag == "Enemy") //Enemy ships receive damage from players, and viceversa.
            {
                other.GetComponent<Ship>().Damaged(collisionDamage);
            }
        }
        else //If an enemy ship collides with another:
        {
            if (other.tag == "Boundary" || other.tag == "Enemy" || other.tag == "Collectible" || other.tag == "EnemyBolt")
            {
                return; //Ships will only destroy themselves and deal damage to players and shields upon collision with them.
            }
            else if (other.tag == "Shield") //Enemy ships damage shields they collide with.
            {
                other.GetComponent<Shield>().Damaged(collisionDamage);
            }
            else if (other.tag == "Player") //Enemy ships damage players they collide with.
            {
                other.GetComponent<Ship>().Damaged(collisionDamage);
            }
        }
    }
}