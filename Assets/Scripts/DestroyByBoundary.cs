using UnityEngine;

// TODO: use Network Objectpooling
// use OnTriggerExit to despawn! 

/// <summary>
/// Destroys a game object when it exits the level's boundaries.
/// </summary>
public class DestroyByBoundary : MonoBehaviour 
{
	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject);
	}
}