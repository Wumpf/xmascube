using UnityEngine;
using System.Collections;

public class LightFlickerScript : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

        light.intensity = Mathf.Max(4.2f, Mathf.Min(5.75f, light.intensity + Random.Range(-0.2f, 0.2f)));
	}
}
