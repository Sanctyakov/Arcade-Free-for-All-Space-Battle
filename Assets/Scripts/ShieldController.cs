using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private GameController gameController;
    private Collider col;
    private MeshRenderer mr;

    private int shield;
    private float shieldCounter;
    private bool shieldDamaged, stop;

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

        col = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();

        gameController.OnGamePause += OnGamePause;
        gameController.OnGameFree += OnGameFree;
        gameController.OnShieldUpgraded += OnShieldUpgraded;

        shield = gameController.ShieldCapacity;
        gameController.UpdateShield(shield);
    }


    void OnDisable()
    {
        gameController.OnGamePause -= OnGamePause;
        gameController.OnGameFree -= OnGameFree;
        gameController.OnShieldUpgraded -= OnShieldUpgraded;
    }


    void Update()
    {
        if (stop || !shieldDamaged)
        {
            return;
        }

        shieldCounter += Time.deltaTime;

        if (gameController.ShieldRate - shieldCounter <= 0)
        {
            ShieldUp();
        }
    }

    public void ShieldDamaged(int damage)
    {
        shieldCounter = 0;
        shield -= damage;

        if (shield <= 0)
        {
            shield = 0;
            ShieldDown();
        }

        shieldDamaged = true;

        gameController.UpdateShield(shield);
    }

    void ShieldDown()
    {
        shieldCounter = 0;
        col.enabled = false;
        mr.enabled = false;
    }

    void ShieldUp()
    {
        col.enabled = true;
        mr.enabled = true;
        shield += 1;

        if (shield >= gameController.ShieldCapacity)
        {
            shield = gameController.ShieldCapacity;
            shieldCounter = 0;
            shieldDamaged = false;
        }

        gameController.UpdateShield(shield);
    }

    void OnGamePause()
    {
        stop = true;
    }

    void OnGameFree()
    {
        stop = false;
    }

    void OnShieldUpgraded()
    {
        shieldDamaged = true;
        gameController.UpdateShield(shield);
    }
}
