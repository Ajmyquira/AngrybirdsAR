using System;
using System.Collections;
using System.Collections.Generic; 
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text ScoreText;
    public Text HighscoreText;
    public GameObject FloatingText;
    public GameObject SlingshotBird;
    public GameObject StillBird;
    public GameObject LevelWon;
    public GameObject LevelLost;
    public Slingshot Slingshot;
    public GameObject NewHighscore;
    public int RemainingBirds = 3;
    public float BirdDestructionTime = 5f;
    public bool IsLevelCleared;
    public bool IsLevelCompleted;
    public bool ActiveTurn;
    public int Score;
    public AudioSource WoodDestruction;
    public AudioSource IceDestruction;
    public AudioSource PigDestroy;
    public AudioSource BirdDestroy;
    public AudioSource PigHit;
    public AudioSource LevelCleared;
    public AudioSource LevelFailed;
    public AudioSource LevelCompleted;
    public GameObject Plane;
    public GameObject ImageTarget;

    GameObject bird;
    GameObject stillBird;
    private GameObject[] stillBirds;

    /* ---- Aruco variables ---- */
    WebCamTexture tex;
    // Create default parameres for detection
    DetectorParameters detectorParameters = DetectorParameters.Create();
    // Dictionary holds set of all available markers
    Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);
    // Variables to hold results
    Point2f[][] corners;
    int[] ids;
    Point2f[][] rejectedImgPoints;

    // Voice recognition variables
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, Action> wordToAction;

    void Start()
    {
        // Aruco webcam setting
        WebCamDevice[] devices = WebCamTexture.devices;
        tex = new WebCamTexture(devices[0].name);
        tex.Play();

        if (Instance == null)
        {
            Instance = this;
        }
        int level = SceneManager.GetActiveScene().buildIndex;
        HighscoreText.text = GetHighscore(level).ToString();
        SetNewBird();

        // Voice recognition part
        wordToAction = new Dictionary<string, Action>();
        wordToAction.Add("dispara", Shoot);
        wordToAction.Add("sujeta", Hold);

        keywordRecognizer = new KeywordRecognizer(wordToAction.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognizer;
        keywordRecognizer.Start();
    }

    private void Hold()
    {
        //bird.GetComponent<Bird>()._isPressed = true;
        bird.GetComponent<Bird>().HoldEvent();
    }

    private void Shoot()
    {
        //bird.GetComponent<Bird>()._isPressed = false;
        bird.GetComponent<Bird>().ShootEvent();
    }

    private void WordRecognizer(PhraseRecognizedEventArgs word)
    {
        // Debug.Log(word.text);
        wordToAction[word.text].Invoke(); // Calling the Hold or Shoot function
    }

    void Update()
    {
        // Aruco part
        Plane.GetComponent<Renderer>().material.mainTexture = tex;
        Mat frame = OpenCvSharp.Unity.TextureToMat(tex);
        // Convert image to grasyscale
        Mat grayFrame = new Mat();
        Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);
        findAruco(frame, grayFrame);


        if (!IsLevelCleared && GameObject.FindGameObjectsWithTag("Pig").Length == 0)
        {
            IsLevelCleared = true;
            LevelCleared.Play();
            if (!ActiveTurn)
            {
                FinishLevel();
            }
        }
    }
    
    public void AddScore(int amount, Vector3 position, Color textColor)
    {
        if (IsLevelCompleted)
        {
            return;
        }

        int level = SceneManager.GetActiveScene().buildIndex;
        Score += amount;
        ScoreText.text = Score.ToString();
        GameObject floatingTextObj = Instantiate(FloatingText, position, Quaternion.identity);
        FloatingText floatingText = floatingTextObj.GetComponent<FloatingText>();
        floatingText.UpdateText(amount.ToString(), textColor);
    }

    public void SetNewBird()
    {
        stillBirds = new GameObject[RemainingBirds];
        ActiveTurn = false;
        RemainingBirds--;
        if (RemainingBirds >= 0)
        {
            bird = Instantiate(SlingshotBird, new Vector3(Slingshot.transform.position.x - 0.08f, Slingshot.transform.position.y + 3.82f, Slingshot.transform.position.z - 0.29f), Quaternion.identity);
            bird.transform.parent = ImageTarget.transform;
            // bird.transform.position = ImageTarget.transform.position;
            bird.GetComponent<Bird>().DestructionTime = BirdDestructionTime;
            Slingshot.Bird = bird;
            Camera.main.GetComponent<MainCamera>().Bird = bird;

            foreach (StillBird stillBird in FindObjectsOfType<StillBird>())
            {
                Destroy(stillBird.gameObject);
            }

            if (RemainingBirds > 0)
            {
                for (int i = 0; i < RemainingBirds; i++)
                {
                    stillBird = Instantiate(StillBird, new Vector3(0, 0, 0), Quaternion.identity);
                    stillBirds[i] = stillBird;
                    stillBirds[i].transform.parent = ImageTarget.transform;
                    stillBirds[i].transform.Find("Bird Body").transform.position = new Vector3(Slingshot.transform.position.x - 2.5f * (i + 1), Slingshot.transform.position.y + 3.82f, Slingshot.transform.position.z - 0.29f);
                    if (i % 2 == 0)
                    {
                        stillBirds[i].GetComponent<StillBird>().WaitForSeconds = 0.45f;
                    }
                }
            }
        }

        FinishLevel();
    }

    private void FinishLevel()
    {
        if (IsLevelCleared)
        {
            if (RemainingBirds >= 0)
            {
                StartCoroutine(AddFinalScores());
            }
            else
            {
                EndLevel(true);
            }
        }
        else if (RemainingBirds < 0)
        {
            if (FindObjectsOfType<Pig>().All(p => p.GetComponent<Rigidbody>().velocity.magnitude < 0.1f))
            {
                EndLevel(false);
            }
            else
            {
                StartCoroutine(CheckIfPigsStoppedMoving());
            }
        }
    }

    IEnumerator CheckIfPigsStoppedMoving()
    {
        yield return new WaitForSeconds(0.25f);

        FinishLevel();
    }

    IEnumerator AddFinalScores()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (StillBird stillBird in FindObjectsOfType<StillBird>())
        {
            AddScore(10000, stillBird.transform.Find("Bird Body").transform.position, Color.red);
        }
        foreach (Bird bird in FindObjectsOfType<Bird>())
        {
            AddScore(10000, bird.transform.position, Color.red);
        }

        yield return new WaitForSeconds(1);

        EndLevel(true);
    }

    private void EndLevel(bool wonLevel)
    {
        if (wonLevel)
        {
            int level = SceneManager.GetActiveScene().buildIndex;
            LevelCompleted.Play();
            IsLevelCompleted = true;

            int highscore = GetHighscore(level);
            int score = Score;
            if (score > highscore)
            {
                highscore = score;
                PlayerPrefs.SetInt($"{level}-highscore", highscore);
                PlayerPrefs.Save();
                NewHighscore.SetActive(true);
            }

            LevelWon.transform.Find("Level Text").GetComponent<Text>().text = $"1-{level + 1}";
            LevelWon.transform.Find("Score Amount Text").GetComponent<Text>().text = score.ToString();
            HighscoreText.text = highscore.ToString();
            LevelWon.transform.Find("Highscore Amount Text").GetComponent<Text>().text = highscore.ToString();
            LevelWon.SetActive(true);
        }
        else
        {
            LevelFailed.Play();
            LevelLost.SetActive(true);
        }
    }

    private int GetHighscore(int level)
    {
        return PlayerPrefs.HasKey($"{level}-highscore") ? PlayerPrefs.GetInt($"{level}-highscore") : 0;
    }
    // Aruco functions
    private void findAruco(Mat frame, Mat grayFrame)
    {
        // Detect and draw markers
        CvAruco.DetectMarkers(grayFrame, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

        if (ids.Length != 0)
        { // Markers detected
          // Debug.Log("Id: " + ids[0]);
          // Debug.Log(corners[0][0] + "; " + corners[0][1] + "; " + corners[0][2] + "; " + corners[0][3]);

            // Calculate of the marker center
            Point2f center = corners[0][0] + corners[0][2];
            int x = (int)(center.X / 2.0);
            int y = (int)(center.Y / 2.0);
            // Debug.Log(x + " : " + y);

            float x_des, y_des, z_des;
            // x movement
            if (x > 670) { x_des = -.15f; }
            else if (x <= 670 && x > 550) { x_des = 0; }
            else { x_des = .3f; }
            // y movement
            if (y > 410) { y_des = -.15f; }
            else if (y <= 410 && y > 310) { y_des = 0; }
            else { y_des = .15f; }

            // Calculate of the marker depth
            double perimeter = perimeterCalculate();
            int distance = (int)(25 - 0.009 * perimeter);

            // z movement
            if (distance > 18) { z_des = -.15f; }
            else if (distance <= 18 && distance > 15) { z_des = 0; }
            else { z_des = .15f; }
            //Debug.Log(distance);

            // Move the game object associated
            //initPos = otherObject.transform.position;
            Vector3 destPos = new Vector3(x_des, y_des, z_des);
            //finalPos = initPos + destPos * Time.deltaTime / 2;
            //finalPos = initPos + destPos;
            //otherObject.transform.position = finalPos;
            //print(destPos);

            if (bird != null)
            {
                bird.GetComponent<Bird>().destPos = destPos;
            }
        }
        else {
            if (bird != null)
            {
                bird.GetComponent<Bird>().destPos = new Vector3(0, 0, 0);
            }
        }
        
    }

    // Aruco functions
    private double perimeterCalculate()
    {
        // Calculate of distances
        double distance1 = Math.Sqrt(Math.Pow(corners[0][0].X - corners[0][1].X, 2) +
            Math.Pow(corners[0][0].Y - corners[0][1].Y, 2));
        double distance2 = Math.Sqrt(Math.Pow(corners[0][1].X - corners[0][2].X, 2) +
            Math.Pow(corners[0][1].Y - corners[0][2].Y, 2));
        double distance3 = Math.Sqrt(Math.Pow(corners[0][2].X - corners[0][3].X, 2) +
            Math.Pow(corners[0][2].Y - corners[0][3].Y, 2));
        double distance4 = Math.Sqrt(Math.Pow(corners[0][3].X - corners[0][0].X, 2) +
            Math.Pow(corners[0][3].Y - corners[0][0].Y, 2));
        double perimeter = distance1 + distance2 + distance3 + distance4;
        return perimeter;
    }
}