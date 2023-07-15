using UnityEngine;

/// <summary>
/// <c>Boundary</c> represents a rectangle bounds.
/// </summary>
[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

/// <summary>
/// Controller for the Player's spaceship.
/// </summary>
public class PlayerController : MonoBehaviour 
{
	/// <summary>
	/// The speed of the Player's ship. This value will be set from Unity's 
	/// Inspector panel.
	/// </summary>
	[SerializeField] private float speed;

	/// <summary>
	/// The boundary of the game world.
	/// </summary>
	[SerializeField] private Boundary boundary;
	
	/// <summary>
	/// The tilt of the spaceship when banking to the left or right.
	/// </summary>
	[SerializeField] private float tilt;

	/// <summary>
	/// The laser bolt game object that will be created when the 
	/// Player presses 'Fire'.
	/// </summary>
	[SerializeField] private GameObject laserBolt;

	/// <summary>
	/// The transform containing the position and rotation where the laser bolt
	/// game object will be created at.
	/// </summary>
	public Transform shotSpawn;

	/// <summary>
	/// Player's firing rate.
	/// </summary>
	[SerializeField] private float fireRate = 0.5f;

	/// <summary>
	/// The time in seconds when the Player can fire the next shot.
	/// </summary>
	private float nextFire = 0.0f;


	private Rigidbody rb;
	private AudioSource audioSrc;

	private void Awake() {
		rb = GetComponent<Rigidbody>();
		audioSrc = GetComponent<AudioSource>();
	}


	void Update()
	{
		// Create the laser bolt when Player presses the 'Fire' key.
		// We also limit the number of laser bolts the Player can fire at 
		// any one time.
		if (Input.GetButton("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate(laserBolt, shotSpawn.position, shotSpawn.rotation);

			// Play the sound effect when Player fires the spaceship's weapons.
			audioSrc.Play();
		}
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// Move the spaceship based on player's input.
		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		rb.velocity = movement * speed;

		// Clamp the spaceship to be within the game world's boundaries.
		rb.position = new Vector3(
			Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
		);

		// Tilt the spaceship when banking to the left or right.
		rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}