using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class SaccadeLogger : MonoBehaviour
{
    // Remove the unused allFrames and ensure correct namespace for FrameData
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
        if (FindObjectsOfType<SaccadeLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        if (string.IsNullOrEmpty(fileName))
            if (string.IsNullOrEmpty(this.fileName))
                this.fileName = "EyeLog";

        string guid = Guid.NewGuid().ToString();
        string fileNameNew = $"{this.fileName}_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid()}.csv";
        if (string.IsNullOrEmpty(Application.persistentDataPath))
        {
            Debug.LogError("⚠️ Application.persistentDataPath is null or empty. Using current directory.");
            this.filePath = Path.Combine(Directory.GetCurrentDirectory(), fileNameNew);
        }
        else
        {
            this.filePath = Path.Combine(Application.persistentDataPath, fileNameNew);
        }

        Debug.Log($"📦 [Awake] Logging to: {this.filePath}");
    }
    // This ensures data is saved when the scene is unloaded
    // This is useful for multi-scene setups where data needs to persist
    // across scene transitions
    // This is useful for multi-scene setups where data needs to persist

    void OnEnable()
    {
        SceneManager.sceneUnloaded += HandleSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= HandleSceneUnloaded;
    }

    private void HandleSceneUnloaded(Scene scene)
    {
        SaveToCSV();
    }
    // ...existing code...

    void Start()
    {
        this.startTime = Time.time;
        Debug.Log($"📦 Logging to: {this.filePath}");
    }
    void Update()
    {
        if (this.leftEyeTransform == null || this.rightEyeTransform == null || this.centerEyeTransform == null) return;

        float t = Time.time - this.startTime;
        string scene = SceneManager.GetActiveScene().name;

        var current = new dataStructure.FrameData(
            scene,
            t,
            new dataStructure.ObjectData(this.leftEyeTransform),
            new dataStructure.ObjectData(this.rightEyeTransform),
            new dataStructure.ObjectData(this.centerEyeTransform)
        );

        allFrames.Add(current);
    }

    public void SaveToCSV()
    {
        if (allFrames.Count == 0) return;

        using (var writer = new StreamWriter(this.filePath))
        {
            writer.WriteLine(this.csvHeader);
            foreach (var frame in allFrames)
                writer.WriteLine(frame.ToCsvString());
        }

        Debug.Log($"✅ All scene data saved to {this.filePath}");
        
    }
    }
