using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) //Our boundary will destroy any game object that passes through it.
	{
		Destroy(other.gameObject); //We've made sure that the player will never reach it.
	}
}