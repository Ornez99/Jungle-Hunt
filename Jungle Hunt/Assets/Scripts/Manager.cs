using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public float orthographicSize, aspect;
    public Camera cam;
    public float camSpeed;
    private AudioSource AS;

    public int score, time, health;
    public Text txt_score, txt_time, txt_health;
    public Slider sli_air;

    private AsyncOperation asyncLoadLevel;
    public int currentLevel;
    public bool sceneChanged;

    private float timer, timer2;
    public Player player;

    public int lianeCount, crocodileCount, boulderCount;
    public int spawnedCrocodiles, spawnedBoulders;
    public GameObject liane, bush, crocodile, bubbles, boulder;
    public Sprite spr_bush1, spr_bush2;

    public float camMaxPos;

    private void Awake() {
        AS = GetComponent<AudioSource>();
        GameObject.Find("Resolution").transform.localScale = new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1f);
        DontDestroyOnLoad(gameObject);
        time = 2000;
        health = 3;
        score = 0;
    }

    private void Update() {
        if (health < 0 && currentLevel != 4) {
            // Koniec gry
            ChangeLevel(3);
        }

        if (sceneChanged == true) {
            if (asyncLoadLevel.isDone) {
                GameObject.Find("Resolution").transform.localScale = new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1f);
                OnSceneChange();
            }
        }
        if (currentLevel == 4) return;
        if (currentLevel == 0 || sceneChanged == true) return;

        SpawnObject();

        // Podążanie kamery za postacią 
        if (player.transform.position.x < camMaxPos) {
            camMaxPos = player.transform.position.x;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(camMaxPos, cam.transform.position.y, cam.transform.position.z), 0.125f);

        // Zmiana punktów bonusowych
        timer += Time.deltaTime;
        if (timer >= 1f) {
            timer -= 1f;
            time -= 10;
            txt_time.text = "" + time;
        }
    }

    // Tworzenie krokodyli i skał
    private void SpawnObject() {
        if (currentLevel == 2) {
            if (spawnedCrocodiles < crocodileCount) {
                timer2 += Time.deltaTime;

                if (timer2 >= 3f) {
                    Instantiate(crocodile, new Vector3(-10 + player.transform.position.x, Random.Range(-3, 4), 0), Quaternion.Euler(0, 0, 0));

                    if (Random.Range(0, 4) == 2) StartCoroutine("SpawnBubbles"); 

                    timer2 = 0;
                    spawnedCrocodiles++;

                    if (spawnedCrocodiles == crocodileCount) StartCoroutine("ChangeSceneAfterSeconds");
                }
            }
        } else if (currentLevel == 3) {
            if (spawnedBoulders < boulderCount) {
                timer2 += Time.deltaTime;

                if (timer2 >= 3f) {
                    Instantiate(boulder, new Vector3(-12 + player.transform.position.x, -3, 0), Quaternion.Euler(0, 0, 0));
                    timer2 = 0;
                    spawnedBoulders++;

                    if (spawnedBoulders == boulderCount) StartCoroutine("ChangeSceneAfterSeconds");
                }
            }
        }
    }

    // Tworzenie bąbelków
    private IEnumerator SpawnBubbles() {
        WaitForSeconds wfs = new WaitForSeconds(0.5f);
        int numb = Random.Range(2, 6);
        for (int i = 0; i < numb; i++) {
            Instantiate(bubbles, new Vector3(player.transform.position.x + Random.Range(-4, 4), -4.3f, 0), Quaternion.Euler(0, 0, 0));
            yield return wfs;
        }
    }

    // Zmiana sceny po jakimś czasie
    private IEnumerator ChangeSceneAfterSeconds() {
        WaitForSeconds wfs = new WaitForSeconds(2.5f);
        yield return wfs;
        score += 500;
        yield return wfs;
        timer2 = 0;
        spawnedBoulders = 0;
        spawnedCrocodiles = 0;
        ChangeLevel(currentLevel);
    }

    public void StartNewGame() {
        GameObject.Find("Resolution").transform.localScale = new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1f);
        time = 2000;
        health = 3;
        score = 0;
        ChangeLevel(0);
    }

    public void ExitGame() {
        Application.Quit();
    }

    // Ładowanie informacji po zmianie sceny
    private void OnSceneChange() {
        if (currentLevel == 4) {
            sceneChanged = false;
            GameObject.Find("Wynik").GetComponent<Text>().text = "Wynik: " + score;
            GameObject.Find("Start").GetComponent<Button>().onClick.AddListener(() => StartNewGame());
            GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(() => ExitGame());
            return;
        }

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camMaxPos = 0;
        SetCamera();
        sceneChanged = false;
        GameObject.Find("Resolution").transform.localScale = new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1f);
        player = GameObject.Find("Player").GetComponent<Player>();
        txt_score = GameObject.Find("Score").GetComponent<Text>();
        txt_time = GameObject.Find("Time").GetComponent<Text>();
        txt_health = GameObject.Find("Health").GetComponent<Text>();

        txt_score.text = "" + score;
        txt_time.text = "" + time;
        txt_health.text = "" + health;

        if (currentLevel == 1) {
            GenerateLevel1();
        }
        else if (currentLevel == 2) {
            sli_air = GameObject.Find("Air").GetComponent<Slider>();
            sli_air.value = 10;
        }
        else if (currentLevel == 3) {
            GenerateLevel3();
        }

        player.SetPlayer(currentLevel);
    }

    // Ustawienie pola widzenia kamery, aby na każdym urządzeniu było takie samo.
    private void SetCamera() {
        cam.projectionMatrix = Matrix4x4.Ortho(
             -orthographicSize * aspect, orthographicSize * aspect,
             -orthographicSize, orthographicSize,
             cam.nearClipPlane, cam.farClipPlane);
    }

    // Tworzenie ozdobynch rzeczy dla poziomu 3
    private void GenerateLevel3() {
        for (int i = 0; i < boulderCount * 5; i++) {
            int spawnBush = Random.Range(0, 2);

            if (spawnBush == 1) {
                GameObject _ins = Instantiate(bush, new Vector2(-i * 2, 5.5f), Quaternion.Euler(0, 0, 0));
                if (Random.Range(0, 2) == 1) _ins.GetComponent<SpriteRenderer>().sprite = spr_bush2;
                if (Random.Range(0, 2) == 1) _ins.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    // Tworzenie lian i ozdób dla pozomi 1
    private void GenerateLevel1() {
        for (int i = 0; i < lianeCount * 5; i++) {
            int spawnBush = Random.Range(0, 2);
            if (i % 5 == 0) {
                spawnBush = 1;
                GameObject _ins = Instantiate(liane, new Vector2(-i * 2 - 0.063f, 4.25f), Quaternion.Euler(0, 0, 0));
                if (i == 0) _ins.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                player.lianes.Add(_ins);
            } 

            if (spawnBush == 1) {
                GameObject _ins = Instantiate(bush, new Vector2(-i * 2, 5.5f), Quaternion.Euler(0, 0, 0));
                if (Random.Range(0, 2) == 1) _ins.GetComponent<SpriteRenderer>().sprite = spr_bush2;
                if (Random.Range(0, 2) == 1) _ins.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    // Zmiana sceny
    public void ChangeLevel(int _currentLevel) {
        int _nextLevel = _currentLevel + 1;
        if (_nextLevel > 3 && health >= 0) {
            _nextLevel = 1;
            health++;
            score += time;
            time = 2000;
            spawnedCrocodiles = 0;
            spawnedBoulders = 0;
            lianeCount += 5;
            crocodileCount += 5;
            boulderCount += 5;
        }
        asyncLoadLevel = SceneManager.LoadSceneAsync(_nextLevel, LoadSceneMode.Single);
        currentLevel = _nextLevel;
        sceneChanged = true;
    }
}