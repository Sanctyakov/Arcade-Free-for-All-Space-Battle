using UnityEngine;

public class Shield : MonoBehaviour
{
    public Collider col; //Set within the prefab.
    public MeshRenderer mr; //We want to be able to make the shield disappear.

    private float shield, shieldRate, shieldCapacity;
    private float shieldCounter; //The time it takes the shield to start regenerating.
    private bool stop; //Signals the shield to stop regerating.

    void Start()
    {
        GameControl.OnGameFreeze += OnGameFreeze;
        GameControl.OnGameFree += OnGameFree;
        GameControl.OnShieldUpgraded += OnShieldUpgraded;

        shield = GameControl.ShieldCapacity; //Shields start out maxed.
        shieldCapacity = GameControl.ShieldCapacity;
        shieldRate = GameControl.ShieldRate;
        GameControl.UpdateShield(shield);
    }


    void OnDestroy()
    {
        GameControl.OnGameFreeze -= OnGameFreeze;
        GameControl.OnGameFree -= OnGameFree;
        GameControl.OnShieldUpgraded -= OnShieldUpgraded;
    }


    void Update()
    {
        if (stop || shield == shieldCapacity)
        {
            return; //Do not regenerate if not enough time has passed, or if the shield is at full.
        }

        shieldCounter += Time.deltaTime;

        if (shieldRate - shieldCounter <= 0)
        {
            ShieldUp(); //If enough time has passed, regenerate.
        }
    }

    public void Damaged(float damage)
    {
        shieldCounter = 0; //Reset the counter.
        shield -= damage;

        if (shield <= 0)
        {
            shield = 0;
            shieldCounter = 0;
            col.enabled = false; //Stop detecting collisions.
            mr.enabled = false; //The shield disappears.
        }

        GameControl.UpdateShield(shield); //Communicate damage received to Game Control.
    }

    void ShieldUp() //Regenerates the shield.
    {
        col.enabled = true; //This shouldn't be called in Update(), but it just takes a few seconds.
        mr.enabled = true;

        if (shield >= shieldCapacity)
        {
            shield = shieldCapacity; //If enough time has passed, bring up downed shields.
        }
        else
        {
            shield += 1;
        }

        GameControl.UpdateShield(shield);
    }

    void OnGameFreeze()
    {
        stop = true; //Stop updating if the game is paused or the round is over.
    }

    void OnGameFree()
    {
        stop = false; //Resume updating.
    }

    void OnShieldUpgraded()
    {
        shield = GameControl.ShieldCapacity; //Gets new values from Game Control upon purchase.
        shieldCapacity = GameControl.ShieldCapacity;

        ShieldUp(); //As shield capacity is already at max, this function will just display it.
    }
}