using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour {

    private float xSpeed, ySpeed;
    private Transform ground;
    private float groundPosY;
    private float jumpMultiplier, fallMultiplier;
    private AudioSource AS;
    public AudioClip audio_boom;
    public bool canGivePoints;

    private void Awake() {
        canGivePoints = true;
        AS = GetComponent<AudioSource>();
        xSpeed = Random.Range(3, 6);
        ground = GameObject.Find("ground").transform;
        int big = Random.Range(0, 6);
        groundPosY = -3.5f;
        jumpMultiplier = 1;
        fallMultiplier = 1;
        if (big == 0) {
            xSpeed = 6;
            jumpMultiplier = 4;
            fallMultiplier = 2;
            groundPosY = -3.25f;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Update() {
        if (transform.position.x > GameObject.Find("Player").transform.position.x + 14) Destroy(gameObject);

        if (canGivePoints == true) {
            if (transform.position.x - 2 >= GameObject.Find("Player").transform.position.x) {
                canGivePoints = false;
                if (GetComponent<CircleCollider2D>().enabled == true) {
                    GameObject.Find("Manager").GetComponent<Manager>().score += 100;
                    GameObject.Find("Manager").GetComponent<Manager>().txt_score.text = GameObject.Find("Manager").GetComponent<Manager>().score + "";
                }
            }
        }

        if (transform.position.y <= groundPosY) {
            AS.PlayOneShot(audio_boom);
            Jump();
            ground.GetComponent<Ground>().shakeDuration += 1f;
        }

        ySpeed -= Time.deltaTime * 7f * fallMultiplier;
        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);
    }

    private void Jump() {
        transform.position = new Vector3(transform.position.x, groundPosY + 0.05f, 0);
        ySpeed = 2f * jumpMultiplier;
    }
}
