using UnityEngine;

public class EnemyBoltCollisions : MonoBehaviour
{
    public int damage;
    private ShieldController shieldController;
    private PlayerController playerController;
    public GameObject explosion;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary" || other.tag == "Enemy" || other.tag == "Collectible" || other.tag == "EnemyBolt")
        {
            return;
        }
        else if (other.tag == "Player")
        {
            playerController = other.GetComponent<PlayerController>();
            playerController.PlayerDamaged(damage);
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if (other.tag == "Shield")
        {
            shieldController = other.GetComponent<ShieldController>();
            shieldController.ShieldDamaged(damage);
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}