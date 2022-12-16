using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class Player1 : MonoBehaviourPunCallbacks
{
    GameObject bird;
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

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        tex = new WebCamTexture(devices[0].name);
        tex.Play();

        view = GetComponent<PhotonView>();

        // Referring the instanced object
        bird = GameObject.Find("Slingshot Bird(Clone)");
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
            FindAruco(frame, grayFrame);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        tex.Stop();
    }

    private void FindAruco(Mat frame, Mat grayFrame)
    {
        // Detect and draw markers
        CvAruco.DetectMarkers(grayFrame, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

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
            Vector3 destPos = new Vector3(x_des, y_des, z_des);
            //Debug.Log(destPos);

            // Pass the movement to the game object
            bird.GetComponent<Bird>().destPos = destPos;
        }
        else
        {
            // Final vector for movement
            Vector3 destPos = new Vector3(0, 0, 0);
            //Debug.Log(destPos);
            bird.GetComponent<Bird>().destPos = destPos;
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
}
