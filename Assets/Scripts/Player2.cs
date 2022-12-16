using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using Photon.Pun;
using Photon.Realtime;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class Player2 : MonoBehaviourPunCallbacks
{
    // Game objects
    GameObject imageTarget;

    // Objects lists
    private Pig[] arrPigs;
    private Wood[] arrWoods;
    private Ice[] arrIces;
    public int RemainingPigs;
    public int RemainingWoods;
    public int RemainingIces;
    int numPerObject = 4;
    public int numObject = 0; //0: cerdo, 1: vidrio, 2: madera

    int numPigs = 0;
    int numWoods = 0;
    int numIces = 0;

    bool completePigs = false;
    bool completeIces = false;
    bool completeWoods = false;

    public int numWordRecognized = -1;

    // Network identifier
    PhotonView view;

    // Aruco variables
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

    // Start is called before the first frame update
    void Start()
    {
        // Image Target instance
        imageTarget = GameObject.Find("ImageTarget");

        // Aruco webcam setting
        WebCamDevice[] devices = WebCamTexture.devices;
        tex = new WebCamTexture(devices[0].name);
        tex.Play();

        view = GetComponent<PhotonView>();

        // Initialization of the number of objects
        RemainingPigs = numPerObject;
        RemainingIces = numPerObject;
        RemainingWoods = numPerObject;
        // Creation of array of objects
        // Pigs
        arrPigs = new Pig[RemainingPigs];
        arrPigs[0] = GameObject.FindObjectsOfType<Pig>()[0];
        numPigs++;
        // Woods
        arrWoods = new Wood[RemainingWoods];
        arrWoods[0] = GameObject.FindObjectsOfType<Wood>()[0];
        numWoods++;
        // Ice
        arrIces = new Ice[RemainingIces];
        arrIces[0] = GameObject.FindObjectsOfType<Ice>()[0];
        numIces++;

        // Voice recognition part
        wordToAction = new Dictionary<string, Action>();
        wordToAction.Add("cerdo", NewPig);
        wordToAction.Add("cristal", NewGlass);
        wordToAction.Add("madera", NewWood);

        keywordRecognizer = new KeywordRecognizer(wordToAction.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognizer;
        keywordRecognizer.Start();
    }
    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            GetComponent<Renderer>().material.mainTexture = tex;
            Mat frame = OpenCvSharp.Unity.TextureToMat(tex);
            // Convert image to grasyscale
            Mat grayFrame = new Mat();
            Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);

            // If the object was intantiated
            if (numWordRecognized == 0 || numWordRecognized == 1 || numWordRecognized == 2) // NUEVO CERDITO / VIDRIO / MADERA
            {
                FindAruco(frame, grayFrame);
            }
            // If the object was droppod
            else if (numWordRecognized == 3) // SOLTAR ARUCO
            {
                Debug.Log("Object dropped");
            }
        }
        // Activate the gravity
        if (completePigs && completeIces && completeWoods)
        {
            Debug.Log("Gravity activated");
            for (int i = 0; i < RemainingPigs; i++)
            {
                arrPigs[i].GetComponent<Pig>().kinematic = true;
            }
            for (int i = 0; i < RemainingIces; i++)
            {
                arrIces[i].GetComponent<Ice>().kinematic = true;
            }
            for (int i = 0; i < RemainingWoods; i++)
            {
                arrWoods[i].GetComponent<Wood>().kinematic = true;
            }
        }
    }
    private void NewPig()
    {
        numObject = 0;
        // Mover el last created object
        if (numPigs < RemainingPigs)
        {
            arrPigs[numPigs] = Instantiate(GameObject.FindObjectsOfType<Pig>()[0], new Vector3(arrPigs[0].GetComponent<Rigidbody>().position.x + 8.0f, arrPigs[0].GetComponent<Rigidbody>().position.y - 7.4f, arrPigs[0].GetComponent<Rigidbody>().position.z - 12.0f), Quaternion.identity);
            arrPigs[numPigs].transform.parent = imageTarget.transform;
        }
        numPigs++;
        if (numPigs == RemainingPigs)
        {
            completePigs = true;
        }
    }
    private void NewGlass()
    {
        numObject = 1;
        // Mover el last created object
        if (numIces < RemainingIces)
        {
            arrIces[numIces] = Instantiate(GameObject.FindObjectsOfType<Ice>()[0], new Vector3(arrIces[0].GetComponent<Rigidbody>().position.x + 8.0f, arrIces[0].GetComponent<Rigidbody>().position.y - 1.1f, arrIces[0].GetComponent<Rigidbody>().position.z - 12.0f), Quaternion.identity);
            arrIces[numIces].transform.parent = imageTarget.transform;
        }
        numIces++;
        if (numIces == RemainingIces)
        {
            completeIces = true;
        }
    }
    private void NewWood()
    {
        numObject = 2;

        // Mover el last created object
        if (numWoods < RemainingWoods)
        {
            arrWoods[numWoods] = Instantiate(GameObject.FindObjectsOfType<Wood>()[0], new Vector3(arrWoods[0].GetComponent<Rigidbody>().position.x + 8.0f, arrWoods[0].GetComponent<Rigidbody>().position.y - 10.5f, arrWoods[0].GetComponent<Rigidbody>().position.z - 12.0f), Quaternion.identity);
            arrWoods[numWoods].transform.parent = imageTarget.transform;
        }
        numWoods++;
        if (numWoods == RemainingWoods)
        {
            completeWoods = true;
        }
    }
    private void WordRecognizer(PhraseRecognizedEventArgs word)
    {
        // Debug.Log(word.text);
        // Calling the Hold or Shoot function
        wordToAction[word.text].Invoke();

        if (word.text == "cerdo") //para colocar objeto cerdito
        {
            numWordRecognized = 0;
        }
        else if (word.text == "cristal") //para colocar objeto vidrio
        {
            numWordRecognized = 1;
        }
        else if (word.text == "madera") //para colocar objeto madera
        {
            numWordRecognized = 2;
        }
        else if (word.text == "down") //para colocar objeto y soltar, ya no reconoce el aruco para mover el objeto
        {
            numWordRecognized = 3;
        }
    }
    // Aruco functions
    private void FindAruco(Mat frame, Mat grayFrame)
    {
        // Detect and draw markers
        CvAruco.DetectMarkers(grayFrame, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
        Vector3 destPos = new Vector3(0, 0, 0);
        GameObject[] ground = GameObject.FindGameObjectsWithTag("Ground");

        if (ids.Length != 0)
        {
            // Market detected
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
            // Debug.Log(distance);

            // Final vector for movement
            destPos = new Vector3(x_des, y_des, z_des);
        }
        if (numObject == 0 && numPigs <= RemainingPigs) //CERDITOS
        {
            if (arrPigs[numPigs - 1] != null) // destPos es (0,0,0)
            {
                arrPigs[numPigs - 1].GetComponent<Pig>().destPos = destPos;
                arrPigs[numPigs - 1].GetComponent<Pig>().groundPos = ground[0].GetComponent<Transform>().position;
            }
        }
        else if (numObject == 1 && numIces <= RemainingIces) //ICES
        {
            if (arrIces[numIces - 1] != null) // destPos es (0,0,0)
            {
                arrIces[numIces - 1].GetComponent<Ice>().destPos = destPos;
                arrIces[numIces - 1].GetComponent<Ice>().groundPos = ground[0].GetComponent<Transform>().position;
            }
        }
        else if (numObject == 2 && numWoods <= RemainingWoods) //WOODS
        {
            if (arrWoods[numWoods - 1] != null) // destPos es (0,0,0)
            {
                arrWoods[numWoods - 1].GetComponent<Wood>().destPos = destPos;
                arrWoods[numWoods - 1].GetComponent<Wood>().groundPos = ground[0].GetComponent<Transform>().position;
            }
        }
    }
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
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        tex.Stop();
    }
}
