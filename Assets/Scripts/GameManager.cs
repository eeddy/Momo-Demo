using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Image p1h1, p1h2, p1h3; //Hearts
    public Text p1Score;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public FloorManager p1FloorManager;

    public GameObject momo1, p1Spikes;
    public GameObject gameOverScreen, pauseScreen; 
    public Text time; 

    private float previousTime = 0;
    private int player1Lives; 

    private bool p1Touching;

    private SoundManager soundManager;
    private float gameStart = 0;
    private float gameEnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        Time.timeScale = 1;

        //Set initial player lives 
        player1Lives = 3;

        p1Touching = false;
        
        soundManager = FindObjectOfType<SoundManager>();
        gameStart = Time.time;

        //Set control strategy 
        MovementControllerEMG emgController = momo1.GetComponent<MovementControllerEMG>();
        MovementController keyboardController = momo1.GetComponent<MovementController>();
        if (PlayerPrefs.GetInt("control") == 1) {
            keyboardController.enabled = false;
            emgController.enabled = true;
        } else {
            keyboardController.enabled = true;
            emgController.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for escape pressed 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }

        if (player1Lives == 0) {
            if(gameEnd == 0) {
                gameEnd = Time.time;
            }
            gameOverScreen.SetActive(true);
            Time.timeScale = 0;
        } else {
            CheckForPowerUpCollision();
            CheckForSpikeTouch();
            if(p1FloorManager.CheckSwordTouch()) {
                p1FloorManager.Reset();
                soundManager.PlayLoseLifeSound();
                player1Lives--;
            }
        } 
        UpdateLives();
    }

    public void PlayAgain() {
        p1FloorManager.Reset();
        player1Lives = 3;
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
        gameEnd = 0;
        gameStart = Time.time;
    }
    
    public void Quit() {
        SceneManager.LoadScene("MainMenu");
    }

    public void UnPause() {
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    void CheckForSpikeTouch() 
    {
        if (p1Spikes.GetComponent<BoxCollider2D>().IsTouching(momo1.GetComponent<BoxCollider2D>()) && !p1Touching) {
            p1Touching = true;
            p1FloorManager.Reset();
            soundManager.PlayLoseLifeSound();
            player1Lives--;
        } else if(!p1Spikes.GetComponent<BoxCollider2D>().IsTouching(momo1.GetComponent<BoxCollider2D>())) {
            p1Touching = false;
        }
    }

    void CheckForPowerUpCollision() 
    {
        //Player 1:
        for(int i=0; i<p1FloorManager.powerups.Count; i++) {
            if(momo1.GetComponent<BoxCollider2D>().bounds.Contains(p1FloorManager.powerups[i].transform.position)) {
                soundManager.PlayCoinCollectedSound();
                p1FloorManager.AcquiredPowerup(i);
            }
        }
    }

    void UpdateLives() 
    {
        UpdateplayerLives(player1Lives, p1h1, p1h2, p1h3);
    }

    void UpdateplayerLives(int numLives, Image heart1, Image heart2, Image heart3) {
        heart1.sprite = fullHeart;
        heart2.sprite = fullHeart;
        heart3.sprite = fullHeart;
        if (numLives < 3) {
            heart3.sprite = emptyHeart;
        } 
        if (numLives < 2) {
            heart2.sprite = emptyHeart;
        } 
        if (numLives < 1) {
            heart1.sprite = emptyHeart;
        }
    }
}
