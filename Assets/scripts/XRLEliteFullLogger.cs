using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class XREliteFullLogger : MonoBehaviour
{
    [System.Serializable]
    public class ObjectData
    {
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ, rotW;
        public float dirX, dirY, dirZ;

        public ObjectData(Transform transform)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 direction = transform.forward;

            posX = position.x;
            posY = position.y;
            posZ = position.z;

            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
            rotW = rotation.w;

            dirX = direction.x;
            dirY = direction.y;
            dirZ = direction.z;
        }

        public string ToCsvString()
        {
            return $"{posX},{posY},{posZ},{rotX},{rotY},{rotZ},{rotW},{dirX},{dirY},{dirZ}";
        }
    }

    [System.Serializable]
    public class FrameData
    {
        public string sceneName;
        public float elapsedTime;
        public ObjectData leftEye, rightEye, centerEye;

        public FrameData(string scene, float time, ObjectData left, ObjectData right, ObjectData center)
        {
            sceneName = scene;
            elapsedTime = time;
            leftEye = left;
            rightEye = right;
            centerEye = center;
        }

        public string ToCsvString()
        {
            return $"{sceneName},{elapsedTime:F4},{leftEye.ToCsvString()},{rightEye.ToCsvString()},{centerEye.ToCsvString()}";
        }
    }

    private static List<FrameData> allData = new(); // static to persist across scenes
    private string filePath;
    private float startTime;

    [Header("Transforms (Eye + Head)")]
    public Transform leftEyeTransform;
    public Transform rightEyeTransform;
    public Transform centerEyeTransform;

    private string csvHeader = "SceneName,ElapsedTime," +
                               "LeftPosX,LeftPosY,LeftPosZ,LeftRotX,LeftRotY,LeftRotZ,LeftRotW,LeftDirX,LeftDirY,LeftDirZ," +
                               "RightPosX,RightPosY,RightPosZ,RightRotX,RightRotY,RightRotZ,RightRotW,RightDirX,RightDirY,RightDirZ," +
                               "CenterPosX,CenterPosY,CenterPosZ,CenterRotX,CenterRotY,CenterRotZ,CenterRotW,CenterDirX,CenterDirY,CenterDirZ";

    void Awake()
    {
        // Only allow one logger to persist
        if (FindObjectsOfType<XREliteFullLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        startTime = Time.time;
        string fileName = $"HTCXR_EyeHead_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"📦 Logging to: {filePath}");
    }

    void Update()
    {
        if (!leftEyeTransform || !rightEyeTransform || !centerEyeTransform) return;

        float t = Time.time - startTime;
        string scene = SceneManager.GetActiveScene().name;

        FrameData current = new FrameData(
            scene,
            t,
            new ObjectData(leftEyeTransform),
            new ObjectData(rightEyeTransform),
            new ObjectData(centerEyeTransform)
        );

        allData.Add(current);
    }

    void OnApplicationQuit()
    {
        SaveToCSV();
    }

    public void SaveToCSV()
    {
        if (allData.Count == 0) return;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(csvHeader);
            foreach (var frame in allData)
                writer.WriteLine(frame.ToCsvString());
        }

        Debug.Log($"✅ All scene data saved to {filePath}");
    }
}
