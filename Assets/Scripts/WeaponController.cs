using UnityEngine;

/// <summary>
/// Fires the weapon repeatedly based on the weapon's fire rate settings.
/// </summary>
public class WeaponController : MonoBehaviour 
{
	/// <summary>
	/// The game object that represents the laser bolt being fired.
	/// </summary>
	[SerializeField] private GameObject shot;

	/// <summary>
	/// The position and rotation transform where the shot should be spawned from.
	/// </summary>
	[SerializeField] private Transform shotSpawn;

	/// <summary>
	/// The fire rate of the weapon in seconds.
	/// </summary>
	[SerializeField] private float fireRate;

	/// <summary>
	/// The delay in seconds before firing the first shot.
	/// </summary>
	[SerializeField] private float delay;


	private AudioSource audioSrc;
	
	void Start () 
	{
		audioSrc = GetComponent<AudioSource>();
		InvokeRepeating("FireWeapon", delay, fireRate);
	}

	/// <summary>
	/// This method will be called repeatedly by <c>InvokeRepeating</c>.
	/// </summary>
	private void FireWeapon()
	{
		// Create the shot (laser bolt) game object.
		Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

		// Play the weapon's sound effects.
		audioSrc.Play();
	}
}
