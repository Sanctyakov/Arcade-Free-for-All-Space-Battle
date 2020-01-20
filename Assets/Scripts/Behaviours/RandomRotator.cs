using UnityEngine;

public class RandomRotator : MonoBehaviour 
{
	public float tumble; //Coins and other drifting objects will tumble with a random intensity.
	
	void Start ()
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
	}
}