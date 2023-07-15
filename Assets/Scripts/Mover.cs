using UnityEngine;

// TODO: not sure if this should be local or the server should signal it after spawning????

/// <summary>
/// Moves a game object that this script is attached to.
/// </summary>
public class Mover : MonoBehaviour 
{
	/// <summary>
	/// The speed of the game object. Use negative values to invert the direction.
	/// </summary>
	[SerializeField] private float speed;

	void Start () 
	{
		// When the laser bolt is added to the scene, we give it 
		// a velocity to move it forward.
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}

}
