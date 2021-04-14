using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

    public float shakeDuration;
    public Vector3 originalPos;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    private void Awake() {
        originalPos = transform.position;
    }

    private void Update() {
        originalPos = new Vector3(transform.position.x, -5.85f, 0);

        // Trzęsienie ziemi 
        if (shakeDuration > 0) {
            shakeDuration -= Time.deltaTime * decreaseFactor;
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
        } else {
			shakeDuration = 0f;
			transform.localPosition = originalPos;
		}

    }
}
