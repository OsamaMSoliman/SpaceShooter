using UnityEngine;
using System.Collections;

/// <summary>
/// An enemy spaceship will randomly zig-zag across the screen to make it more
/// challenging for the Player to shoot it down.
/// </summary>
public class EvasiveManeuver : MonoBehaviour 
{
	/// <summary>
	/// The boundary of the level.
	/// </summary>
	[SerializeField] private Boundary boundary;

	/// <summary>
	/// The tilt factor to tilt the enemy spaceship when strafing left or right.
	/// </summary>
	[SerializeField] private float tilt;

	/// <summary>
	/// How far should the enemy ship strafe to the left or right.
	/// </summary>
	[SerializeField] private float dodge;

	/// <summary>
	/// The speed of the evasive maneuver.
	/// </summary>
	[SerializeField] private float speed;

	/// <summary>
	/// The random range in seconds to wait before starting the evasive maneuver.
	/// </summary>
	public Vector2 startWait;

	/// <summary>
	/// The random range in seconds to maneuver in a particular direction.
	/// </summary>
	public Vector2 maneuverTime;

	/// <summary>
	/// The random range in seconds to wait before maneuvering to a new direction.
	/// </summary>
	[SerializeField] private Vector2 maneuverWait;

	/// <summary>
	/// The velocity vector's X-axis value.
	/// </summary>
	private float moveHorizontal;


	private Rigidbody rb;
	
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		StartCoroutine( Evade() );
	}

	IEnumerator Evade()
	{
		// Enemy ships will start the evasive maneuver randomly.
		yield return new WaitForSeconds( Random.Range(startWait.x, startWait.y) );

		// Evasive maneuver will be performed until the enemy ship is destroyed or exits
		// the level.
		while (true) 
		{
			// Enemy ship will randomly strafe left or right.
			moveHorizontal = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
			yield return new WaitForSeconds( Random.Range(maneuverTime.x, maneuverTime.y) );

			// Enemy ship will now travel in a straight line for random number of seconds 
			// before strafing left or right again.
			moveHorizontal = 0;
			yield return new WaitForSeconds( Random.Range(maneuverWait.x, maneuverWait.y) );
		}
	}
	
	void FixedUpdate()
	{
		// Change the velocity vector's X-axis value only. The Y and Z values are 
		// constant. Changing the X values will move the enemy ship diagonally 
		// left or right.
		Vector3 newVelocity = rb.velocity;
		newVelocity.x = Mathf.MoveTowards(rb.velocity.x, 
		                                  moveHorizontal, 
		                                  speed * Time.deltaTime);
		rb.velocity = newVelocity;

		// Make sure the enemy spaceship does not fly out of the level's boundaries.
		rb.position = new Vector3(
			Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
		);

		// Tilt the enemy spaceship when it is banking left or right 
		// during the evasive maneuver.
		rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
