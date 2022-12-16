using System.Collections; 
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject ImageTarget;

    GameObject bird;
    GameObject stillBird;
    private GameObject[] stillBirds;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        int level = SceneManager.GetActiveScene().buildIndex;
        HighscoreText.text = GetHighscore(level).ToString();
        SetNewBird();
    }
    void Update()
    {
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
}