using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liane : MonoBehaviour {
    public Sprite[] spr;
    public int currentState;

    public float speed, timer;
    public bool isRising;

    public Vector3[] colliderPos;
    public float[] colliderRot;

    private void Awake() {
        speed = Random.Range(12, 24);
    }

    private void Update() {
        timer += Time.deltaTime * speed;

        if (timer >= 2f) {
            timer = 0;

            if (isRising) {
                if (currentState < 15) currentState++;
                else {
                    currentState--;
                    isRising = false;
                }
            } else {
                if (currentState > 5) currentState--;
                else {
                    currentState++;
                    isRising = true;
                }
            }

            // Animacja
            GetComponent<SpriteRenderer>().sprite = spr[currentState];
            // Zmiana rotacji i pozycji strefy kolizji.
            transform.GetChild(0).transform.position = colliderPos[currentState] + transform.position;
            transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, colliderRot[currentState]);
        }
    }
}