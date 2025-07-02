using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class SmoothPursuitLogger : MonoBehaviour
{
    List<dataStructure.FrameData> allFrames = new List<dataStructure.FrameData>(); // static to persist across scenes
    private string filePath;
    private float startTime;
    public string fileName;

    [Header("Transforms (Eye + Head)")]
    public Transform leftEyeTransform;
    public Transform rightEyeTransform;
    public Transform centerEyeTransform;

    private string csvHeader = "SceneName,ElapsedTime," +
                               "LeftPosX,LeftPosY,LeftPosZ,LeftRotX,LeftRotY,LeftRotZ,LeftRotW,LeftDirX,LeftDirY,LeftDirZ," +
                               "RightPosX,RightPosY,RightPosZ,RightRotX,RightRotY,RightRotZ,RightRotW,RightDirX,RightDirY,RightDirZ," +
                               "CenterPosX,CenterPosY,CenterPosZ,CenterRotX,CenterRotY,CenterRotZ,CenterRotW,CenterDirX,CenterDirY,CenterDirZ";

    // void Awake()
    // {
    //     // Only allow one logger to persist
    //     if (FindObjectsOfType<XREliteFullLogger>().Length > 1)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     DontDestroyOnLoad(this.gameObject);
    // }

    // void Start()
    // {
    //     startTime = Time.time;
    //     string fileNameNew = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
    //     if (string.IsNullOrEmpty(Application.persistentDataPath))
    //     {
    //         Debug.LogError("Application.persistentDataPath is null or empty. Cannot create log file path.");
    //         filePath = fileNameNew; // fallback to just the filename
    //     }
    //     else
    //     {
    //         filePath = Path.Combine(Application.persistentDataPath, fileNameNew);
    //     }
    //     Debug.Log($"📦 Logging to: {filePath}");
    // }


    void Awake()
    {
        // Only allow one logger to persist
        if (FindObjectsOfType<SmoothPursuitLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        if (string.IsNullOrEmpty(fileName))
            fileName = "EyeLog";

        string guid = Guid.NewGuid().ToString();
        string fileNameNew = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}_{guid}.csv";

        if (string.IsNullOrEmpty(Application.persistentDataPath))
        {
            Debug.LogError("⚠️ Application.persistentDataPath is null or empty. Using current directory.");
            filePath = Path.Combine(Directory.GetCurrentDirectory(), fileNameNew);
        }
        else
        {
            filePath = Path.Combine(Application.persistentDataPath, fileNameNew);
        }

        Debug.Log($"📦 [Awake] Logging to: {filePath}");
    }
    // Subscribe to scene unload event to save data
    // This ensures data is saved when the scene is unloaded
    // This is useful for multi-scene setups where data needs to persist
    // across scene transitions
    // This is useful for multi-scene setups where data needs to persist

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveToCSV();
    }

    // ...existing code...

    void Start()
    {
        startTime = Time.time;
        Debug.Log($"📦 Logging to: {filePath}");
    }
    void Update()
    {
        if (!leftEyeTransform || !rightEyeTransform || !centerEyeTransform) return;

        float t = Time.time - startTime;
        string scene = SceneManager.GetActiveScene().name;

        dataStructure.FrameData current = new dataStructure.FrameData(
            scene,
            t,
            new dataStructure.ObjectData(leftEyeTransform),
            new dataStructure.ObjectData(rightEyeTransform),
            new dataStructure.ObjectData(centerEyeTransform)
        );

        allFrames.Add(current);
    }

    public void SaveToCSV()
    {
        if (allFrames.Count == 0) return;

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine(csvHeader);
            foreach (var frame in allFrames)
                writer.WriteLine(frame.ToCsvString());
        }

        Debug.Log($"✅ All scene data saved to {filePath}");

    }
    
}
