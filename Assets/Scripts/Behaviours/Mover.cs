using UnityEngine;

public class Mover : MonoBehaviour
{
	public float speed; //This script moves things in the Z direction according to the speed entered in the inspector (negative will move downwards, positive upwards).

	void Start () //We will not concern ourselves with mass in this game, so we'll modify velocity directly instead of using AddForce.
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}
