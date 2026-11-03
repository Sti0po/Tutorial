//using UnityEngine;  // needed to access MonoBehaviour, Mathf, Random, GameObject, Transform, etc
//using TMPro;        // needed to access TextMeshPro text box types
//using UnityEngine.InputSystem;

//public class PlayerControllerTask6 : MonoBehaviour
//{
//    Vector3 velocity;

//    [Header("Text box references")]
//    public TextMeshProUGUI ScoreTextBox;
//    public TextMeshProUGUI TimerTextBox;
//    public TextMeshProUGUI CompletionTextBox;

//    [Header("Gameplay variables")]
//    [SerializeField] int Score = 0;
//    [SerializeField] float Timer = 60;
//    [SerializeField] bool CompletionStatus = false;

//    float moveSpeed = 10;

//    void AddPoints(int amount)
//    {
//        Score += amount;
//    }

//    void Start()
//    {
//    }

//    void Update()
//    {
//        float xSpeed = 0;
//        float zSpeed = 0;                           // start with zero for speed if nothing is pressed
//        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)        // forward key pressed
//        {
//            zSpeed += moveSpeed;
//        }
//        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)      // backward key pressed
//        {
//            zSpeed -= moveSpeed;
//        }
//        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)      // backward key pressed
//        {
//            xSpeed -= moveSpeed;
//        }
//        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)      // backward key pressed
//        {
//            xSpeed += moveSpeed;
//        }
//        // speeds now take into account BOTH directions, AND will be zero when neither are used
//        velocity = new Vector3(xSpeed, 0, zSpeed) * Time.deltaTime;

//        if (transform.position.z < -5)                      // too far forward?
//            transform.position += new Vector3(0, 0, 10);    // then wrap around to the back
//        if (transform.position.z > 5)                       // too far back?
//            transform.position -= new Vector3(0, 0, 10);    // then wrap around to the front
//        if (transform.position.x < -5)                      // too far left?
//            transform.position += new Vector3(10, 0, 0);    
//        if (transform.position.x > 5)                       // too far right?
//            transform.position -= new Vector3(10, 0, 0);    

//        transform.position += velocity;
//        Timer = Timer - Time.deltaTime;
//        Timer = Mathf.Clamp(Timer, 0, 60f);
//        UpdateTextBoxes();

//        if (Score >= 3) // win condition met
//        {
//            CompletionStatus = true;
//        }
//        if (Timer <= 0 && !CompletionStatus) // out of time before completing level
//        {
//            Destroy(gameObject);
//        }

//        if (Timer <= 0 && Score != 3)
//        {
//            Destroy(gameObject);
//        }
//    }


//    private void UpdateTextBoxes()
//    {
//        ScoreTextBox.text = "Your score: " + Score.ToString();
//        TimerTextBox.text = "Your time: " + Timer.ToString();
//        CompletionTextBox.text = "Level complete? " + CompletionStatus.ToString();
//    }
//}

using UnityEngine;  // needed to access MonoBehaviour, Mathf, Random, GameObject, Transform, etc
using TMPro;        // needed to access TextMeshPro text box types
using UnityEngine.InputSystem;

public class PlayerControllerTask6 : MonoBehaviour
{
    [Header("Text box references")]
    public TextMeshProUGUI ScoreTextBox;
    public TextMeshProUGUI TimerTextBox;
    public TextMeshProUGUI CompletionTextBox;

    [Header("Gameplay variables")]
    [SerializeField] int Score = 0;
    [SerializeField] float Timer = 60;
    [SerializeField] bool CompletionStatus = false;
    [SerializeField] Rigidbody physicsBody;

    [SerializeField] AudioSource playerSound;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip gameOverClip;

    float moveSpeed = 10;

    void AddPoints(int amount)
    {
        Score += amount;
    }

    void Start()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin") && playerSound != null)
        {
            playerSound.PlayOneShot(coinClip);
        }
        Score = Score + 1;
        Destroy(other.gameObject);
    }

    void Update()
    {
        if (CompletionStatus == false)
        {
            // game in play: player can move
            float zInput = (Keyboard.current.wKey.value - Keyboard.current.sKey.value);
            float xInput = (Keyboard.current.dKey.value - Keyboard.current.aKey.value);
            Vector3 direction = new Vector3(xInput, 0, zInput);
            physicsBody.AddForce(direction * moveSpeed);

            // make the player bounce off some pre-defined invisible walls
            float bounceSpeed = moveSpeed / 4;  // adjust as you like for game feel
            float rightBoundary = 3.75f;
            if (transform.position.x > rightBoundary)   // simple position check against a boundary on the axis
            {
                float newXspeed = -bounceSpeed;
                float maintainZspeed = physicsBody.linearVelocity.z;
                physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);
            }
            float leftBoundary = 1.5f;
            if (transform.position.x < leftBoundary)
            {
                float newXspeed = +bounceSpeed;
                float maintainZspeed = physicsBody.linearVelocity.z;
                physicsBody.linearVelocity = new Vector3(newXspeed, 0, maintainZspeed);
            }

            // "unstuck" button
            if (Keyboard.current.uKey.wasReleasedThisFrame)
            {
                transform.position = new Vector3(3, 0, 0);
            }
        }
        else
        {
            // game completed: let player quit when they press escape
            if(Keyboard.current.escapeKey.isPressed)
            {
                UnityEditor.EditorApplication.ExitPlaymode();
            }
        }

        // loading and saving score
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            PlayerPrefs.SetInt("PlayerScore", Score);
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Score = PlayerPrefs.GetInt("PlayerScore");
        }

        Timer = Timer - Time.deltaTime;
        Timer = Mathf.Clamp(Timer, 0, 60f);
        UpdateTextBoxes();

        if (Score >= 3) // win condition met
        {
            CompletionStatus = true;
        }

        if (Timer <= 0 && Score != 3)
        {
            Destroy(gameObject);
            playerSound.PlayOneShot(gameOverClip);
        }
    }


    private void UpdateTextBoxes()
    {
        ScoreTextBox.text = "Your score: " + Score.ToString();
        TimerTextBox.text = "Your time: " + Timer.ToString();
        CompletionTextBox.text = "Level complete? " + CompletionStatus.ToString();
    }
}
