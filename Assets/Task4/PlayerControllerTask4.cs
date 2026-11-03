using UnityEngine;  // needed to access MonoBehaviour, Mathf, Random, GameObject, Transform, etc
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.VisualScripting;        // needed to access TextMeshPro text box types

public class PlayerControllerTask4 : MonoBehaviour
{
    
    [Header("Text box references")]
    public TextMeshProUGUI ScoreTextBox;
    public TextMeshProUGUI TimerTextBox;
    public TextMeshProUGUI CompletionTextBox;

    [Header("Gameplay variables")]
    [SerializeField] int Score = 0;
    [SerializeField] float Timer = 60;
    [SerializeField] bool CompletionStatus = false;
    AudioSource playerSound;
    [SerializeField] AudioClip Coin_pickup;
    [SerializeField] AudioClip gameOverClip;
    Rigidbody physicsBody;
    float speed;
    float movespeed = 10;
    float Zspeed;
    float Xspeed;
    float Yspeed;

    void Start()
    {
      

        physicsBody = GetComponent<Rigidbody>();
        playerSound = GetComponent<AudioSource>();

        speed = Random.Range(0.1f, 0.7f);
        print(name);
        ChangeName();
        name = ToString();
        print(name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(playerSound != null)
        {
            playerSound.PlayOneShot(Coin_pickup);
        }
        Score = Score + 1;
        Destroy(other.gameObject);
    }
    void Update()
    {
        if (CompletionStatus == false)
        {
            controls();
            Vector3 direction = new Vector3(Xspeed, Yspeed, Zspeed); //move based on "controls"

            //physicsBody.linearVelocity = direction * movespeed;
            physicsBody.AddForce(new Vector3(Xspeed, Yspeed, Zspeed) * movespeed);
        }
        else
        {
            if(Keyboard.current.escapeKey.isPressed)
            {
                UnityEditor.EditorApplication.ExitPlaymode();
            }
        }
        Timer = Timer - Time.deltaTime;
        Timer = Mathf.Clamp(Timer, 0, 60);
        UpdateTextBoxes();
        Kill_player();
        


    }
    void UpdateTextBoxes() //make screen have screen
    {
        string Objective;
        if (CompletionStatus == false)
        {
            Objective = "Get coins";
        }
        else
            Objective = "complete";
        ScoreTextBox.text = "Your score: " + Score.ToString();
        TimerTextBox.text = "Your time: " + ((int)Timer).ToString();
        CompletionTextBox.text = "objective: " + Objective;
    }
    void ChangeName() //renames the stupid red ball
    {
        name = "Player ball";
    }

    private void OnMouseDown()
    {
        
    }
    private void Restart(float startingtimer, int startingtScore = 0) //restart switch
    {
        Timer = startingtimer;
        Score = startingtScore;
    }
    void AddPoints(int amount) // make the points
    {
        Score += amount;
    }
    void Kill_player()
    {
        if (Score >= 3)
        {
            CompletionStatus = true;
        }
        if (Timer <= 0 && !CompletionStatus)
        {
            
            playerSound.PlayOneShot(gameOverClip);
            transform.position = new Vector3(0, -10, 0);
        }
    }

    
    void controls()
    {
        movespeed = 10;
        Zspeed = 0;
        Yspeed = -1;
        Xspeed = 0;
        if (Keyboard.current.shiftKey.isPressed)
        {
            movespeed = 30;
        }
        //Z axis
        Zspeed = (Keyboard.current.wKey.value - Keyboard.current.sKey.value);
        //X axis
        Xspeed = (Keyboard.current.dKey.value - Keyboard.current.aKey.value);


        //bounce off walls 
        float bounceSpeed = movespeed / 2;


        //right wall
        float rightBoundary = 10;

        if (transform.position.x > rightBoundary)

        {

            float newXspeed = -bounceSpeed;

            float maintainZspeed = physicsBody.linearVelocity.z;

            physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);

        }


        //Left wall
        float leftBoundary = -10f;

        if (transform.position.x < leftBoundary)

        {

            float newXspeed = +bounceSpeed;

            float maintainZspeed = physicsBody.linearVelocity.z;

            physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);

        }


        //front wall
        float frontBoundary = -10f;

        if (transform.position.z < frontBoundary)

        {

            float newZspeed = +bounceSpeed;

            float maintainXspeed = physicsBody.linearVelocity.x;

            physicsBody.linearVelocity = new Vector3(maintainXspeed, 0, newZspeed);

        }

        //backwall
        float backBoundary = 10;

        if (transform.position.z > backBoundary)

        {

            float newZspeed = -bounceSpeed;

            float maintainXspeed = physicsBody.linearVelocity.x;

            physicsBody.linearVelocity = new Vector3(maintainXspeed, 0, newZspeed);

        }

        //unstuck

        if (Keyboard.current.uKey.wasReleasedThisFrame)
        {
            transform.position = new Vector3(3, 0, 0);
        }


        //saving
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            PlayerPrefs.SetInt("playerScore", Score);
        }
        
        
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Score = PlayerPrefs.GetInt("playerScore");
        }

        


    }

}


