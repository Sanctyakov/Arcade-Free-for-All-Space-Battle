using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage; //The damage the projectile deals. Set within the editor.

    public GameObject explosion; //Projectiles instantiate explosions upon impact, varying according to each type.

    //Projectile and explosion sounds are played on awake, set within each prefab.

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary" || other.tag == "Collectible")
        {
            return; //Bolts will ignore the boundary (which will destroy them all the same) and collectibles (they will pass through them).
        }

        if (tag == "PlayerProjectile") //If this bolt is fired by the player.
        {
            if (other.tag == "Enemy") //We check only if an enemy is hit. Anything else will be passed through.
            {
                other.GetComponent<Ship>().Damaged(damage); //Transmit damage to the enemy.

                Instantiate(explosion, transform.position, transform.rotation); //Explosions are only instantiated when damage is done.

                Destroy(gameObject); //The projectile is destroyed.
            }
        }
        else
        {
            if (other.tag == "Enemy")
            {
                return; //Enemy fire will pass through each other.
            }
            else if (other.tag == "Player") //Damage is passed to either the player's ship or shield.
            {
                other.GetComponent<Ship>().Damaged(damage);
                Instantiate(explosion, transform.position, transform.rotation);
            }
            else if (other.tag == "Shield")
            {
                other.GetComponent<Shield>().Damaged(damage);
                Instantiate(explosion, transform.position, transform.rotation);
            }

            Destroy(gameObject); //Finally, the projectile is destroyed regardless of what it hits.
        }
    }
}