

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using System;

public class ArucoDetector : MonoBehaviour
{
    // Set the object we are going to control
    [SerializeField] GameObject otherObject;

    // 
    private Vector3 initPos;
    private Vector3 finalPos;

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
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTexture = tex;
        Mat frame = OpenCvSharp.Unity.TextureToMat(tex);

        // Convert image to grasyscale
        Mat grayFrame = new Mat();
        Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);

        findAruco(frame, grayFrame);
    }

    void findAruco(Mat frame, Mat grayFrame)
    {
        // Detect and draw markers
        CvAruco.DetectMarkers(grayFrame, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
        //CvAruco.DrawDetectedMarkers(frame, corners, ids);

        // Create Unity output texture with detected markers
        //Texture newtexture = OpenCvSharp.Unity.MatToTexture(frame);
        //GetComponent<Renderer>().material.mainTexture = newtexture;

        if (ids.Length != 0)
        { // Markers detected
          // Debug.Log("Id: " + ids[0]);
          // Debug.Log(corners[0][0] + "; " + corners[0][1] + "; " + corners[0][2] + "; " + corners[0][3]);

            // Calculate of the marker center
            Point2f center = corners[0][0] + corners[0][2];
            int x = (int)(center.X / 2.0);
            int y = (int)(center.Y / 2.0);
            //Debug.Log(x + " : " + y);

            float x_des, y_des, z_des;
            // x movement
            if (x > 670) { x_des = -.3f; }
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
            if (distance > 15) { z_des = -.15f; }
            else if (distance <= 15 && distance > 10) { z_des = 0; }
            else { z_des = .15f; }
            //Debug.Log(distance);

            // Move the game object associated
            initPos = otherObject.transform.position;
            Vector3 destPos = new Vector3(x_des, y_des, z_des);
            //finalPos = initPos + destPos * Time.deltaTime / 2;
            finalPos = initPos + destPos;
            otherObject.transform.position = finalPos;
            //print(destPos);
        }
    }
    double perimeterCalculate()
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


