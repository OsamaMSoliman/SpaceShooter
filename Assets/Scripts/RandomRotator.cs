using UnityEngine;

// TODO: server call? (doesn't need to be, but maybe let's make it ServerRpc)

/// <summary>
/// Gives an asteroid a random rotation animation.
/// </summary>
public class RandomRotator : MonoBehaviour 
{
	/// <summary>
	/// The asteroid tumble factor.
	/// </summary>
	[SerializeField] private float tumble;
		
	void Start () 
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
	}	
}
