using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SmoothSphereMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float radius = 1.5f;
    public XREliteFullLogger logger;  // Make sure it's public or serialized
    public float speed = 0.5f; // Movement speed

    [Header("Scene Settings")]
    [SerializeField] private float duration = 30f; // Scene duration in seconds

    private Vector3 noiseOffsets;
    // Store the initial position as the base position
    Vector3 basePosition;

    void Start()
    {
        // Store the initial position as the base position
        basePosition = transform.position;

        // Use different random offsets for each axis to decouple movement
        noiseOffsets = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(100f, 200f),
            Random.Range(200f, 300f)
        );

        StartCoroutine(RunSceneForDuration());
    }

    void Update()
    {
        float t = Time.time * speed;

        // Smooth, decoupled Perlin noise movement in 3D
        Vector3 pos = new Vector3(
            (Mathf.PerlinNoise(t + noiseOffsets.x, 0f) - 0.5f) * 2f * radius,
            (Mathf.PerlinNoise(t + noiseOffsets.y, 1f) - 0.5f) * 2f * radius,
            (Mathf.PerlinNoise(t + noiseOffsets.z, 2f) - 0.5f) * 2f * radius
        );

        transform.position = basePosition + pos;
    }

    private IEnumerator RunSceneForDuration()
    {
        yield return new WaitForSeconds(duration);
        logger.SaveToCSV();
        SceneManager.LoadScene("EyeTrackerreal");
    }
}
