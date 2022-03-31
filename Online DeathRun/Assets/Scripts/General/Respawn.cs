using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Respawn : MonoBehaviour
{
	public AudioClip deathClip;
	[SerializeField] private Transform Player;
	[SerializeField] private Transform respawnPoint;
	// Start is called before the first frame update

	IEnumerator respawnPlayer(float spawnDelay)
	{
		yield return new WaitForSeconds(spawnDelay);
	}

	private void OnTriggerEnter(Collider other)
	{
		StartCoroutine("respawnPlayer", 1f);
		Player.transform.position = respawnPoint.transform.position;
		Physics.SyncTransforms();
		AudioSource.PlayClipAtPoint (deathClip, transform.position);
	}
	
}

