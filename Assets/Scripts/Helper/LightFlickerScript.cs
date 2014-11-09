using UnityEngine;
using System.Collections;

public class LightFlickerScript : MonoBehaviour {

    private float _flickerGoal;
    private float _flickerSpeed = 0.2f;
    private float _startIntensity;

    void Start()
    {
        _startIntensity = light.intensity;
        StartCoroutine(SelectFlicker());
    }

	// Update is called once per frame
	void Update ()
    {
        light.intensity = Mathf.Lerp(_flickerGoal, light.intensity, Mathf.Exp(-Time.deltaTime / _flickerSpeed));
	}

    IEnumerator SelectFlicker()
    {
        while(true)
        {
            _flickerGoal = _startIntensity + Random.Range(-0.6f, 1.0f);
            yield return new WaitForSeconds(_flickerSpeed);
        }
    }
}
