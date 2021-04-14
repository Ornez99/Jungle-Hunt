using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    private void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * 4f);
        if (transform.position.y >= 4f) Destroy(gameObject);
    }

}
