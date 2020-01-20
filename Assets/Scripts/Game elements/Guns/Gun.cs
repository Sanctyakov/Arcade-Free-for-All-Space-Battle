using UnityEngine;

public class Gun : MonoBehaviour //Ships have guns. They will open fire according to this script.
{
	public GameObject shot;
	public Transform shotSpawn; //The position in front of the ship from whence the shot will be instantiated.
	public float fireRate;

    [HideInInspector]
    public float nextFire = 0; //An auxiliary counter to help create fire rate.

    public AudioClip shotSFX;

	public void Fire() //Opening fire shoots projectiles if the fire rate permits it.
	{
        if (Time.time > nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
	}

    public virtual void Shoot() //Projectiles are instantiated with a sound.
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        GameControl.PlayClip(shotSFX);
    }
}