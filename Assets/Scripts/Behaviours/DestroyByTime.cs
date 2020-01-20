using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	public float lifetime; //Coins and such will disappear over time if the player does not obtain them.

	void Start ()
	{
		Destroy (gameObject, lifetime);
	}
}
