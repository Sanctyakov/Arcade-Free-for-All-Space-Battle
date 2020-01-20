using UnityEngine;

//A static class that communicates with the Game Controller script, so that each instantiated object doesn't need to find the script by tag.

public static class GameControl
{
    private static GameController gC;

    private static AudioSource aS;

    public static void SetGameController(GameController gameController)
    {
        gC = gameController; //The Game Controller script reference is set.
    }

    public static void SetAudioSource(AudioSource audioSource)
    {
        aS = audioSource; //The Audio Source reference is set.
    }

    #region Read only properties

    //The following properties can only be read, never written. In this way, this class acts as an accessibility manager.

    public static float MaxHealth
    {
        get { return gC.maxHealth; }
    }

    public static float ShieldCapacity
    {
        get { return gC.shieldCapacity; }
    }

    public static int Rockets
    {
        get { return gC.rockets; }
    }

    public static float ShieldRate
    {
        get { return gC.shieldRate; }
    }

    #endregion

    #region Events

    //These static events will tell game objects what to do on specific occassions, as dictated by the Game Controller script.

    public delegate void GameFreeze(); //Things will stop doing stuff. Called upon during the Splash Screen, Pause and Game Over.
    public static event GameFreeze OnGameFreeze;

    public delegate void GameFree(); //Things will resume doing stuff.
    public static event GameFree OnGameFree;

    public delegate void HealthRestored(); //Players subscribed to this event will replenish their health.
    public static event HealthRestored OnHealthRestored;

    public delegate void RocketsReplenished(); //Rocket launchers subscribed to this event will replenish their rockets.
    public static event RocketsReplenished OnRocketsReplenished;

    public delegate void ShieldUpgraded(); //Shields subscribed to this event will reduce their downtime.
    public static event ShieldUpgraded OnShieldUpgraded;

    public static void CallGameFreeze()
    {
        OnGameFreeze?.Invoke(); //Events will only be invoked if they are not null.
    }

    public static void CallGameFree()
    {
        OnGameFree?.Invoke();
    }

    public static void CallHealthRestored()
    {
        OnHealthRestored?.Invoke();
    }

    public static void CallRocketsReplenished()
    {
        OnRocketsReplenished?.Invoke();
    }

    public static void CallShieldUpgraded()
    {
        OnShieldUpgraded?.Invoke();
    }

    #endregion

    #region GameController public methods

    //Just redirect calls to Game Controller script.

    public static void AddCoin() //Called when a coin is obtained.
    {
        gC.AddCoin();
    }

    public static void EnemyDown(float score) //Called when an enemy is killed.
    {
        gC.EnemyDown(score);
    }

    public static void PlayerDown()
    {
        gC.PlayerDown();
    }

    public static void UpdateHealth(float health)
    {
        gC.UpdateHealth(health);
    }

    public static void UpdateShield(float shield)
    {
        gC.UpdateShield(shield);
    }

    public static void RocketLaunched()
    {
        gC.RocketLaunched();
    }

    #endregion

    #region Audio

    //Game objects pass on audio clips for the Game Control to send to the audio source and play.

    public static void PlayClip (AudioClip clip)
    {
        aS.PlayOneShot(clip); //Game objects send their audio clip to this script so that the same audio source plays all audio, which prevents having too many audio sources.
    }

    #endregion
}
