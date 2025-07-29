using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Text;

public class FixationLogger : MonoBehaviour
{
    private List<dataStructure.FrameData> allFrames;
    private string filePath;
    private float startTime;
    public string fileName;

    [Header("Transforms (Eye + Head)")]
    public Transform leftEyeTransform;
    public Transform rightEyeTransform;
    public Transform centerEyeTransform;

    private const string csvHeader = "SceneName,ElapsedTime," +
        "LeftPosX,LeftPosY,LeftPosZ,LeftRotX,LeftRotY,LeftRotZ,LeftRotW,LeftDirX,LeftDirY,LeftDirZ," +
        "RightPosX,RightPosY,RightPosZ,RightRotX,RightRotY,RightRotZ,RightRotW,RightDirX,RightDirY,RightDirZ," +
        "CenterPosX,CenterPosY,CenterPosZ,CenterRotX,CenterRotY,CenterRotZ,CenterRotW,CenterDirX,CenterDirY,CenterDirZ";

    private static readonly StringBuilder sb = new StringBuilder(4096);

    void Awake()
    {
        if (FindObjectsOfType<FixationLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        if (string.IsNullOrEmpty(fileName))
            fileName = "EyeLog";

        string guid = Guid.NewGuid().ToString();
        string fileNameNew = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}_{guid}.csv";

        filePath = string.IsNullOrEmpty(Application.persistentDataPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), fileNameNew)
            : Path.Combine(Application.persistentDataPath, fileNameNew);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"📦 [Awake] Logging to: {filePath}");
#endif

        allFrames = new List<dataStructure.FrameData>(10000); // Preallocate for efficiency
    }

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

    void Start()
    {
        startTime = Time.time;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"📦 Logging to: {filePath}");
#endif
    }

    void Update()
    {
        if (!leftEyeTransform || !rightEyeTransform || !centerEyeTransform) return;

        float t = Time.time - startTime;
        string scene = SceneManager.GetActiveScene().name;

        allFrames.Add(new dataStructure.FrameData(
            scene,
            t,
            new dataStructure.ObjectData(leftEyeTransform),
            new dataStructure.ObjectData(rightEyeTransform),
            new dataStructure.ObjectData(centerEyeTransform)
        ));
    }

    public void SaveToCSV()
    {
        if (allFrames.Count == 0) return;

        sb.Clear();
        sb.AppendLine(csvHeader);
        foreach (var frame in allFrames)
            sb.AppendLine(frame.ToCsvString());

        File.WriteAllText(filePath, sb.ToString());

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"✅ All scene data saved to {filePath}");
#endif
    }
}
