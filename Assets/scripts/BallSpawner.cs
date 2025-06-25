using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallSpawner : MonoBehaviour
{
    [Header("Sphere Settings")]
    [SerializeField] private GameObject saccadeSphere;
    [SerializeField] private float sphereScale = 0.5f;

    [Header("Timing")]
    [SerializeField] private float totalDuration = 30f;
    [SerializeField] private float switchInterval = 0.05f;

    [Header("Horizontal Movement")]
    [SerializeField] private float horizontalDistance = 5f;
    [SerializeField] private float horizontalJitter = 0.5f;

    [Header("Vertical Movement")]
    [SerializeField] private float verticalDistance = 5f;
    [SerializeField] private float verticalJitter = 0.5f;

    [Header("Z Movement")]
    [SerializeField] private float zAmplitude = 2f;
    [SerializeField] private float zSpeed = 0.5f;

    private float elapsedTime;
    private bool isLeft = true;
    private bool isUp = true;
    public XREliteFullLogger logger;

    private void Start()
    {
        if (saccadeSphere == null)
        {
            Debug.LogError("Sphere not assigned!");
            return;
        }

        saccadeSphere.transform.localScale = Vector3.one * sphereScale;
        StartCoroutine(SaccadePattern());
    }

    private IEnumerator SaccadePattern()
    {
        elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            MoveSphere();
            yield return new WaitForSeconds(switchInterval);
            elapsedTime += switchInterval;
        }
        logger.SaveToCSV();
        saccadeSphere.SetActive(false);
        SceneManager.LoadScene("EyeTrackerreal");
    }

    private void MoveSphere()
    {
        float t = Time.time;
        // Get the main camera's transform
        Transform cam = Camera.main.transform;

        // Set a distance in front of the camera
        float distanceFromCamera = 4f;

        // Calculate base position in front of the camera
        Vector3 basePos = cam.position + cam.forward * distanceFromCamera;

        // Calculate saccade offsets
        float z = Mathf.Sin(t * zSpeed) * zAmplitude;
        float x = (isLeft ? -horizontalDistance : horizontalDistance) + Random.Range(-horizontalJitter, horizontalJitter);
        float y = (isUp ? verticalDistance : -verticalDistance) + Random.Range(-verticalJitter, verticalJitter);

        // Offset from base position in camera's local space
        Vector3 offset = cam.right * x + cam.up * y + cam.forward * z;

        Vector3 nextPos = basePos + offset;

        // Vector3 nextPos = new Vector3(x, y, z);
        saccadeSphere.transform.position = nextPos;

        isLeft = !isLeft;
        isUp = !isUp;

        Debug.Log($"Sphere saccade jump to: {nextPos}");
    }
}
