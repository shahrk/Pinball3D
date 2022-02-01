using UnityEngine;
using UnityEngine.UI;

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
    private float allowed_time = 300;
    float remaining;
    private bool gameOver = false;
    private GameObject ball;
    private GameObject plunger;
    private GameObject drain;

    private GameObject maincam;
    private GameObject puzzleCamera;
    private float startTime;

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


        if (((ball.activeSelf == false) && (ballsLeft == 0)))
        {
            if (gameOver == false)
            {
                gameOver = true;
                audioPlayer.PlayOneShot(gameoverClip);
            }
        }

        SetText();
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
        if (gameOver) winText.text = "Game Over";
        else if (score == 1500) winText.text = "Superstar!";
        else if (score >= 2100) winText.text = "You won!";
        else winText.text = "";

        if (score > highscore) highscore = score;
        highScoreText.text = highscore.ToString();
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


