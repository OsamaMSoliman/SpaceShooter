using UnityEngine;

/// <summary>
/// Scrolls the background image. Makes it look as if the Player's 
/// spaceship is moving forwards.
/// </summary>
public class BackgroundScroller : MonoBehaviour 
{
	/// <summary>
	/// The speed the background should scroll at.
	/// </summary>
	[SerializeField] private float scrollSpeed = 0.0f;

	/// <summary>
	/// The size along the Z-axis of a single background tile in units.
	/// </summary>
	[SerializeField] private float tileSizeZ;

	/// <summary>
	/// The initial position of the background game object.
	/// </summary>
	private Vector3 startPosition;

	private void Awake() => startPosition = transform.position;
	
	void Update () 
	{
		// Loop the background tiles over and over.
		float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
		transform.position = startPosition + (Vector3.forward * newPosition);
	}
}
