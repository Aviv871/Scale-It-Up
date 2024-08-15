using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour 
{
	public float time = 3f;
	private float timer;

	private void Update() 
	{
		timer += Time.deltaTime;

		if(timer > time)
		{
			GetComponent<Fader>().FadeTo("Menu");
		}
	}
}
