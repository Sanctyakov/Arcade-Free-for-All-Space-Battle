using UnityEngine;

public class Player : MonoBehaviour
{
    public Ship ship; //Players have ships.
    public Blaster blaster; //Players have different types of guns.
    public RocketLauncher rocketLauncher;

    void Start() //Players get their health from Game Control.
    {
        GameControl.OnHealthRestored += OnHealthRestored;
    }

    void OnDestroy()
    {
        GameControl.OnHealthRestored -= OnHealthRestored;
    }

    void Update()
    {
        if (!ship.free) //No firing guns if the game is paused or the round is over.
        {
            return;
        }

        if (Input.GetButton("Fire1"))
        {
            blaster.Fire();
        }

        if (Input.GetButton("Fire2") && rocketLauncher.ammo > 0)
        {
            rocketLauncher.Fire();
        }
    }

    void FixedUpdate()
    {
        if (!ship.free) //No moving if the game is paused or the round is over.
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal"); //Arrows or WASD.
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        ship.Move(movement);
    }

    void OnHealthRestored()
    {
        ship.health = GameControl.MaxHealth;
    }
}
