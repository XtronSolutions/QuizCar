using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustController : MonoBehaviour {
	private bool value = true;
	int count = 0;
	public ParticleSystem [] flames;
	// Use this for initialization
	void Start () {

		StartCoroutine (PlayParticles());
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	IEnumerator PlayParticles(){
	
		yield return new WaitForSeconds (2f);
		while (value) {
			foreach (var flm in flames) {
		
				flm.Play ();
			}
			count++;
			if (count == 2) {
			
				value = false;
				StopCoroutine (PlayParticles ());
			}
			yield return new WaitForSeconds (2f);
		}
	}
}
