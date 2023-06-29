using UnityEngine;
using System.Collections;

//  TODO: Despawn locally

/// <summary>
/// Destroys a game object after a specified time.
/// Example: Destroying an explosion particle effect after a short delay.
/// </summary>
public class DestroyByTime : MonoBehaviour 
{
	/// <summary>
	/// The number of seconds to wait before destroying this game object.
	/// </summary>
	[SerializeField] private float lifetime;
	
	void Start () 
	{
		Destroy(gameObject, lifetime);
	}		
}
