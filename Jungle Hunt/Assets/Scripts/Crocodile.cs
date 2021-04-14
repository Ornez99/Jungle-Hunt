using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : MonoBehaviour {
private SpriteRenderer SR;
    public Sprite[] croc_sprites, croc_openMouth_sprites;
    private int spriteState;
    private bool incrementState;

    public float XSpeed, YSpeed;

    public float timer, spriteTimer, openMouthPeriod;
    public bool mouthIsOpen;

    private float timer1;
    private int YState;

    private void Awake() {
        XSpeed = 2f;
        openMouthPeriod = Random.Range(10, 30) / 10f;
        SR = GetComponent<SpriteRenderer>();
    }


    public void Update() {
        timer1 += Time.deltaTime;
        if (timer1 >= 1f) {
            YState = Random.Range(0, 3);
            if (YState == 0) YSpeed = 0;
            else if (YState == 1) YSpeed = 1;
            else if (YState == 2) YSpeed = -1;
            timer1 = 0;
        }
        if (transform.position.y <= -3.5f && YSpeed < 0) YSpeed = 0;
        if (transform.position.y >= 3.5f && YSpeed > 0) YSpeed = 0;

        if (transform.position.x > GameObject.Find("Player").transform.position.x + 14) Destroy(gameObject);
        transform.Translate(new Vector3(XSpeed * Time.deltaTime, YSpeed * Time.deltaTime, 0));
        spriteTimer += Time.deltaTime;
        if (spriteTimer >= 0.5f) {
            ChangeSprite();
            spriteTimer = 0;
        }

        if (mouthIsOpen == false) {
            timer += Time.deltaTime;
            if (timer >= openMouthPeriod) {
                mouthIsOpen = true;
                timer = 0;
            }
        } else {
            timer += Time.deltaTime;
            if (timer >= 1f) {
                mouthIsOpen = false;
                timer = 0;
            }
        }
    }

    // Animowanie krokodyla
    private void ChangeSprite() {
        if (spriteState == 2) {
            incrementState = false;
        } else if (spriteState == 0) {
            incrementState = true;
        }

        if (incrementState == true) {
            spriteState++;
        } else {
            spriteState--;
        }

        if (mouthIsOpen == true) {
            SR.sprite = croc_openMouth_sprites[spriteState];
        } else {
            SR.sprite = croc_sprites[spriteState];
        }
    }
}
