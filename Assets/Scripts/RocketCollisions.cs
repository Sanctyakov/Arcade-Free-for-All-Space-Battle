using UnityEngine;

public class RocketCollisions : MonoBehaviour
{
    public GameObject explosion;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Instantiate(explosion, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}