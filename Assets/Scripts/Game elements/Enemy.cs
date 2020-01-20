using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player; //A reference to the player game object.

    private float targetX = 0; //A variable for movement on the X axis.

    public Ship ship; //Enemies have ships.

    public Gun gun; //Enemies have guns. We reference the parent class each enemy can reference to different guns in the inspector.

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //The first thing enemies do is locate the player character.

        ship.speed = Random.Range(ship.speed / 2, ship.speed); //Enemies will move at slightly different speeds.
    }

    void Update()
    {
        if (!ship.free) //No firing when the game is paused or the round is over.
        {
            return;
        }

        if (player != null)
        {
            gun.Fire(); //Enemies will continuously open fire if there is a player alive. The fire rate is set in the gun's inspector.
        }
    }

    void FixedUpdate ()
	{
        if (!ship.free)
        {
            return; //If the game is paused or over, enemies will not move.
        }

        if (player != null)
        {
            if (player.transform.position.z < transform.position.z) //If enemies are able to shoot the player.
            {
                if (player.transform.position.x < transform.position.x - 0.1f) //A small margin so that the enemy won't continually move sideways.
                {
                    targetX = Mathf.Lerp(targetX, -1, 0.1f); //Move left or right depending on where the player is (but do not match player's movement exactly).
                }
                else if (player.transform.position.x > transform.position.x + 0.1f)
                {
                    targetX = Mathf.Lerp(targetX, 1, 0.1f); //The Lerp helps the enemy move towards its target X slowly. The player can now avoid enemies.
                }
                else
                {
                    targetX = Mathf.Lerp(targetX, 0, 0.1f); //Coordinates for the movement vector will never exceed 1 or -1, because speed is already provided by the ship.
                }
            }
            else
            {
                targetX = 0.0f;
            }
        }

        Vector3 movement = new Vector3(targetX, 0.0f, -transform.forward.z); //We finally have our movement vector.

        ship.Move(movement); //A ship's Move method is the same for all ships, be they friend or foe.
    }
}