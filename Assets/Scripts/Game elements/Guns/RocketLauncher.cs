using UnityEngine;
public class RocketLauncher : Gun //Rocket Launchers are guns that have limited ammo.
{
    [HideInInspector]
    public int ammo;

    void Start()
    {
        GameControl.OnRocketsReplenished += OnRocketsReplenished;

        ammo = GameControl.Rockets; //Ammo will be dictated by the Game Controller, not set within the editor.
    }

    void OnDestroy()
    {
        GameControl.OnRocketsReplenished -= OnRocketsReplenished;
    }

    void OnRocketsReplenished() //Upon hearing this event, the gun will update its ammo.
    {
        ammo = GameControl.Rockets;
    }

    public override void Shoot()
    {
        if (ammo > 0)
        {
            base.Shoot(); //The rocket launcher's fire rate is considered in the base method.

            ammo--;

            GameControl.RocketLaunched(); //Informs the Game Control that rocket launcher ammo is reduced by one.
        }
    }
}
