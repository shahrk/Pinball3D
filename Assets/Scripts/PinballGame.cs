using UnityEngine;
using UnityEngine.UI;
using System;

// Include the namespace required to use Unity UI
using UnityEngine.UI;

public class PinballGame : MonoBehaviour
{

    public Text scoreText;
    public Text highScoreText;
    public Text winText;
    public Text ballsText;
    public Text timerText;

    public int maxBalls = 3;
    public int score = 0;
    private int highscore = 0;

    public float plungerSpeed = 100;

    public AudioSource audioPlayer;
    public AudioClip plungerClip;
    public AudioClip soundtrackClip;
    public AudioClip gameoverClip;

    public KeyCode newGameKey;
    public KeyCode plungerKey;
    public KeyCode puzzlecameraKey;


    private int ballsLeft = 3;
    private float allowed_time = 30;
    float remaining;
    private bool gameOver = false;
    private GameObject ball;
    private GameObject plunger;
    private GameObject drain;

    private GameObject maincam;
    private GameObject puzzleCamera;
    private float startTime;

    private bool isBlackholeActive = true;

    // At the start of the game..
    void Start()
    {
        plunger = GameObject.Find("Plunger");
        drain = GameObject.Find("Drain");
        ball = GameObject.Find("Ball");
        maincam = GameObject.FindGameObjectWithTag("MainCamera");
        puzzleCamera = GameObject.Find("PuzzleCamera");


        puzzleCamera.SetActive(false);

        ball.SetActive(false);

        audioPlayer = GetComponent<AudioSource>();

        audioPlayer.loop = true;
        audioPlayer.clip = soundtrackClip;
        audioPlayer.volume = 0.3f;
        audioPlayer.Play(); 
    }

    private void Update()
    {

        if (Input.GetKey(newGameKey) == true) NewGame();
        if (Input.GetKey(plungerKey) == true) Plunger();
        if (Input.GetKey(puzzlecameraKey) == true) switchCamera();
        //Turn on the timer
        if (ball.activeSelf == true) {
            remaining =allowed_time - (Time.time - startTime);
            string mins = ((int)remaining/60).ToString();
            string secs = (remaining%60).ToString("f1");
            timerText.text = mins + ":" + secs;            
        }
        else {
            timerText.text = "00" + ":" + "00";            
        }

        // detect ball going past flippers into "drain"
        if (((ball.activeSelf == true) && (ball.transform.position.z < drain.transform.position.z)))
        {
            ball.SetActive(false);
        }
        else if (remaining < 0 && ball.activeSelf == true) {
            ball.SetActive(false);
        }


        if (((remaining <= 0) && (ballsLeft == 0)))
        {
            if (gameOver == false)
            {
                gameOver = true;
                audioPlayer.PlayOneShot(gameoverClip);
            }
        }

        SetText();
        IsSuckedUp();
    }

    // Each physics step..
    void FixedUpdate()
    {

    }

    // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
    void SetText()
    {
        // Update the text field of our 'countText' variable
        scoreText.text = score.ToString();

        ballsText.text = ballsLeft.ToString();

        // Check if our 'count' is equal to or exceeded 12
        if (!isBlackholeActive)
            return;
        if (score >= 900) winText.text = "You won!";
        else if (gameOver) winText.text = "Game Over";
        else if (score == 500) winText.text = "Superstar!";
        else winText.text = "";

        if (score > highscore) highscore = score;
        highScoreText.text = highscore.ToString();
    }

    void IsSuckedUp() {
        if (isBlackholeActive && isWithinEllipse(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z)) {
            winText.text = "BLACK HOLED";
            Vector3 initialPos = new Vector3(ball.transform.position.x, 0.0f, ball.transform.position.z);
            Vector3 finalPos = new Vector3(ball.transform.position.x, plunger.transform.position.y, ball.transform.position.z);
            ball.transform.position = initialPos;
            // ball.SetActive(false);
            isBlackholeActive = false;
            StartCoroutine(waiter());
            // ball.SetActive(true);
            // Rigidbody rb = ball.GetComponent<Rigidbody>();
            // Vector3 movement = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0.0f, UnityEngine.Random.Range(-1.0f, 1.0f));
            // rb.AddForce(movement * plungerSpeed);
            // ball.transform.position = finalPos;
        }
        // ball.SetActive(true);
        // Debug.Log("Setting active");
        // Rigidbody rb = ball.GetComponent<Rigidbody>();
        // Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
        // rb.AddForce(movement * plungerSpeed);

        // // set ball position to location of plunger
        // ball.transform.position = plunger.transform.position;
        // ballsLeft = ballsLeft - 1;

        // audioPlayer.PlayOneShot(plungerClip);
        // startTime = Time.time;
    }

    System.Collections.IEnumerator waiter()
    {
        Vector3 finalPos;
        if (isWithinEllipse1(ball.transform.position.x, plunger.transform.position.y, ball.transform.position.z)) {
            finalPos = new Vector3(6.05F, plunger.transform.position.y, 1.75F);
        } else {
            finalPos = new Vector3(-6.275F, plunger.transform.position.y, -0.4F);
        }
        ball.SetActive(false);
        //Wait for 0.3 seconds
        yield return new WaitForSeconds(0.5F);
        // toggleBlackholeWithDelay();
        ball.SetActive(true);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0.0f, UnityEngine.Random.Range(-1.0f, 0.0f));
        rb.AddForce(movement * plungerSpeed/2);
        ball.transform.position = finalPos;
        isBlackholeActive = true;
    }

    System.Collections.IEnumerator toggleBlackholeWithDelay()
    {
        yield return new WaitForSeconds(1);
        isBlackholeActive = true;
    }

    bool isWithinEllipse(float x, float y, float z) {
        return isWithinEllipse1(x,y,z) || isWithinEllipse2(x,y,z);
    }

    bool isWithinEllipse1(float x, float y, float z) {
        float r_x = 0.75F;
        float r_z = 2.2F;
        float h = 6.05F;
        float k = 4.98F;
        if (Math.Pow(x-h,2)/Math.Pow(r_x,2) + Math.Pow(z-k,2)/Math.Pow(r_z,2) <= 1.0 && y <= 5) {
            return true;
        } else {
            return false;
        }
    }

    bool isWithinEllipse2(float x, float y, float z) {
        float r_x = 0.725F;
        float r_z = 2.45F;
        float h = -6.275F;
        float k = 2.05F;
        if (Math.Pow(x-h,2)/Math.Pow(r_x,2) + Math.Pow(z-k,2)/Math.Pow(r_z,2) <= 1.0 && y <= 5) {
            return true;
        } else {
            return false;
        }
    }

    void NewGame()
    {
        ballsLeft = 3;
        gameOver = false;
        ball.SetActive(false);
        score = 0;

        GameObject[] bumpers;
        bumpers = GameObject.FindGameObjectsWithTag("Bumper");

        foreach (GameObject bumper in bumpers)
        {
            bumper.GetComponent<MeshRenderer>().enabled = true;
            bumper.GetComponent<BoxCollider>().enabled = true;
            bumper.GetComponent<BumperController>().hitCount = 0;
        }
    }

    void Plunger()
    {
        if ((ballsLeft > 0) && (ball.activeSelf == false))
        {
            ball.SetActive(true);
            Debug.Log("Setting active");
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
            rb.AddForce(movement * plungerSpeed);

            // set ball position to location of plunger
            ball.transform.position = plunger.transform.position;
            ballsLeft = ballsLeft - 1;

            audioPlayer.PlayOneShot(plungerClip);
            startTime = Time.time;
        }
    }

    void switchCamera()
    {

        if (maincam.activeSelf == true)
        {
            maincam.SetActive(false);
            puzzleCamera.SetActive(true);
        }
        else
        {
            puzzleCamera.SetActive(false);
            maincam.SetActive(true);

        }
    }
}


