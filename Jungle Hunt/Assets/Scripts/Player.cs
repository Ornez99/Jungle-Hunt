using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Manager manager;
    private BoxCollider2D BC;
    public SpriteRenderer SR;
    private Color srColor;
    public Sprite[] spr1, spr2, spr3;
    public AudioSource AS;
    public AudioClip audio_startJump, audio_endJump, audio_roundStart, audio_crocoKilled, audio_playerDamaged;

    public float xSpeed, ySpeed;
    public int currentLevel;
    public bool readyToPlay;

    private bool isJumping, isSitting, catchedByBubbles;
    public List<GameObject> lianes;
    public Liane liane;
    public int currentLiane;

    private float halfWidth;
    private int moveTouchId;
    private Vector3 startMoveTouch, endMoveTouch;
    private Transform movementCircleBack, movementCircleFront;
    private bool isMoving;
    public Transform camT;
    private float remainingAir;

    private float timer1, timer2;
    private int currentSpriteNumber;


    // Załadowanie informacji dla danej sceny
    public void SetPlayer(int _currentLevel) {
        GameObject.Find("Resolution").transform.localScale = new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1f);
        BC = GetComponent<BoxCollider2D>();
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        camT = GameObject.Find("Main Camera").transform;
        xSpeed = 0;
        ySpeed = 0;
        currentLiane = 0;
        currentLevel = _currentLevel;
        if (currentLevel == 1) liane = lianes[0].GetComponent<Liane>();
        halfWidth = (float)Screen.width / 2f;

        if (currentLevel >= 2) {
            movementCircleBack = GameObject.Find("movementCircleBack").transform;
            movementCircleFront = GameObject.Find("movementCircleFront").transform;
            if (currentLevel == 2) manager.sli_air = GameObject.Find("Air").GetComponent<Slider>();
            remainingAir = 10f;
        }

        AS.PlayOneShot(audio_roundStart);
        manager.txt_health.text = manager.health + "";
        readyToPlay = true;
    }

    private void Update() {
        if (readyToPlay == false) return;

        if (currentLevel == 1) { // Dla poziomu 1
            if (isJumping == false) {
                transform.position = new Vector3(liane.transform.position.x + 0.5f + liane.colliderPos[liane.currentState].x, (liane.colliderPos[liane.currentState].y + 3.5f), 0);
                ChangeSpritesForLevel1();
                if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0) JumpLvl1();
            } else {
                ySpeed -= Time.deltaTime * 32f;
                if (transform.position.y <= -4f) {
                    DeadOnLevel1();
                }
            }
        } else if (currentLevel == 2) { // Dla poziomu 2
            ChangeAirLevel();
            ChangeSpritesForLevel2();
            AndroidControls();
            xSpeed *= 5f;
            xSpeed -= 2f;
            ySpeed *= 5f;
            
            // Ograniczenie ruchów postaci, aby nie wypływała poza ekran
            if (transform.position.x > camT.transform.position.x + 8f && xSpeed > 0) xSpeed = -2f;
            if (transform.position.x < camT.transform.position.x - 8f && xSpeed < 0) xSpeed = 2f;
            if (transform.position.y < -4f && ySpeed < 0) ySpeed = 0f;
            if (transform.position.y > 4f && ySpeed > 0) ySpeed = 0f;

            if (xSpeed > 5f) xSpeed = 5f;
            if (ySpeed > 5f) ySpeed = 5f;
            if (ySpeed < -5f) ySpeed = -5f;
            if (xSpeed < -5f) xSpeed = -5f;
        } else if (currentLevel == 3) { // Dla poziomu 3
            if(isJumping == false)
                AndroidControls();

            BC.size = new Vector2(1.4f, 1.36f);

            if (ySpeed > 0.7f && isJumping == false) {
                JumpLvl3();
            }
            if (isJumping == true) {
                ySpeed -= Time.deltaTime * 16f;
                if (transform.position.y <= -3f) {
                    transform.position = new Vector3(transform.position.x, -3f, 0);
                    isJumping = false;
                }
            }

            if (isJumping == false) {
                if (ySpeed < -0.7f) {
                    SitLvl3();
                } else {
                    isSitting = false;
                }
            }

            if (isJumping == false && isSitting == false) {
                ChangeSpritesForLevel3();
                xSpeed *= 5f;
                ySpeed = 0;
            }
            if (transform.position.x > camT.transform.position.x + 10f && xSpeed > 0) xSpeed = -2f;
        }

        if (catchedByBubbles == true) {
            xSpeed = 0;
            ySpeed = 4f;
            if (transform.position.y >= 3.9f) {
                ySpeed = 0;
                catchedByBubbles = false;
            }
        }

        transform.Translate(new Vector3(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0));
    }

    // Siadanie
    private void SitLvl3() {
        isSitting = true;
        xSpeed = 0;
        ySpeed = 0;
        transform.position = new Vector3(transform.position.x, -3.1f, 0f);
        SR.sprite = spr3[5];
        BC.size = new Vector2(1.4f, 0.7f);
    }

    // Skakanie dla poziomu 3
    private void JumpLvl3() {
        transform.position =
        transform.position = new Vector3(transform.position.x, -2.95f, 0f);
        ySpeed = 7f;
        xSpeed = -5f;
        isJumping = true;
        SR.sprite = spr3[4];
        AS.PlayOneShot(audio_startJump);
    }

    // Animowanie postaci dla poziomu 3
    private void ChangeSpritesForLevel3() {
        timer2 += Time.deltaTime;
        if (timer2 >= 0.2f) {
            currentSpriteNumber++;
            if (currentSpriteNumber >= 4) currentSpriteNumber = 0;
            SR.sprite = spr3[currentSpriteNumber];
            timer2 = 0;
        }
    }

    // Zmiana poziomu powietrza
    private void ChangeAirLevel() {
        remainingAir -= Time.deltaTime;
        manager.sli_air.value = remainingAir;
        if (transform.position.y >= 3.9f) remainingAir = 10;
        if (remainingAir <= 0) {
            DeadOnLevel2();
            StartCoroutine("playerDamaged");
            remainingAir = 10;
        }
    }

    // Sterowanie dla poziomu 2 i 3
    private void AndroidControls() {
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).position.x < halfWidth && Input.GetTouch(i).phase == TouchPhase.Began && isMoving== false) {
                    isMoving = true;
                    moveTouchId = Input.GetTouch(i).fingerId;
                    startMoveTouch = Input.GetTouch(i).position;
                    movementCircleBack.transform.position = new Vector2(startMoveTouch.x , startMoveTouch.y);
                }

                if (Input.GetTouch(i).fingerId == moveTouchId && Input.GetTouch(i).phase == TouchPhase.Ended) {
                    isMoving = false;
                }

                xSpeed = 0;
                ySpeed = 0;

                if (isMoving == true) {
                    if (Input.GetTouch(i).fingerId == moveTouchId) {
                        endMoveTouch = Input.GetTouch(i).position;
                        Vector2 offset = endMoveTouch - startMoveTouch;
                        Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
                        movementCircleFront.transform.position = new Vector2((startMoveTouch.x) + (direction.x * 70f ), (startMoveTouch.y) + (direction.y * 70f));

                        xSpeed = direction.x;
                        ySpeed = direction.y;
                    }
                }
            }
        } else {
            isMoving = false;
            xSpeed = 0;
            ySpeed = 0;
        }
    }

    // Gdy postać spadnie z liany
    private void DeadOnLevel1() {
        transform.position = new Vector3(lianes[currentLiane].transform.position.x, 0, 0);
        manager.health--;
        manager.txt_health.text = manager.health + "";
        isJumping = false;
        xSpeed = 0;
        ySpeed = 0;
        AS.PlayOneShot(audio_playerDamaged);
        // Zmiana pozycji kamery po śmierci.
        GameObject.Find("Main Camera").transform.position = new Vector3(transform.position.x - 8, 0, -10);
        manager.camMaxPos = transform.position.x;
    }

    // Gdy postać zostanie zraniona
    private void DeadOnLevel2() {
        manager.health--;
        manager.txt_health.text = manager.health + "";
        xSpeed = 0;
        ySpeed = 0;
    }

    // Animowanie postaci dla poziomu 1
    private void ChangeSpritesForLevel1() {
        if (liane.isRising == true) {
            SR.sprite = spr1[0];
        } else {
            SR.sprite = spr1[2];
        }
    }

    // Animowanie postaci dla poziomu 2
    private void ChangeSpritesForLevel2() {
        timer2 += Time.deltaTime;
        if (timer2 >= 0.3f) {
            currentSpriteNumber++;
            if (currentSpriteNumber >= 4) currentSpriteNumber = 0;
            SR.sprite = spr2[currentSpriteNumber];
            timer2 = 0;
        }
    }

    // Skakanie z liany
    private void JumpLvl1() {
        isJumping = true;
        xSpeed = -17;
        ySpeed = 9f;
        SR.sprite = spr1[1];
        AS.PlayOneShot(audio_startJump);
    }

    // Gdy postać złapie kolejną liane
    private void LianeCatched() {
        AS.PlayOneShot(audio_endJump);
        manager.score += 100;
        manager.txt_score.text = manager.score + "";
        xSpeed = 0;
        ySpeed = 0;

        if (currentLiane + 2 == lianes.Count) {
            manager.score += 500;
            manager.txt_score.text = manager.score + "";
            manager.ChangeLevel(currentLevel);
        } else {
            currentLiane++;
            liane = lianes[currentLiane].GetComponent<Liane>();
        }
    }

    // Wizualizacja zranienia postaci
    private IEnumerator playerDamaged() {
        srColor = Color.red;
        AS.PlayOneShot(audio_playerDamaged);
        while (srColor.r > 0.1f) {
            srColor = Color.Lerp(srColor, Color.white, 0.125f);
            SR.color = srColor;
            yield return new WaitForEndOfFrame();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "liane") {
            isJumping = false;
            other.GetComponent<BoxCollider2D>().enabled = false;
            LianeCatched();
        } else if (other.tag == "crocodile") {
            if (other.GetComponent<Crocodile>().mouthIsOpen == true) {
                other.GetComponent<BoxCollider2D>().enabled = false;
                other.GetComponent<Crocodile>().XSpeed *= 2f;
                StartCoroutine("playerDamaged");
                DeadOnLevel2();
            }
            else if (other.transform.position.y - 0.3f > transform.position.y) {
                manager.score += 100;
                manager.txt_score.text = manager.score + "";
                AS.PlayOneShot(audio_crocoKilled);
                Destroy(other.gameObject);
            }
            else if (other.transform.position.y - 0.3f <= transform.position.y) {
                other.GetComponent<BoxCollider2D>().enabled = false;
                other.GetComponent<Crocodile>().XSpeed *= 2f;
                StartCoroutine("playerDamaged");
                DeadOnLevel2();
            }
        } else if(other.tag == "boulder") {
            other.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("playerDamaged");
            manager.health--;
            manager.txt_health.text = manager.health + "";
        } else if (other.tag == "bubble") {
            catchedByBubbles = true;
            other.transform.position = transform.position;
            other.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

}
