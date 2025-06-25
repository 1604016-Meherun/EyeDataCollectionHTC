using UnityEngine;
using UnityEngine.SceneManagement;

public class fixation : MonoBehaviour
{

    public XREliteFullLogger logger;  // Make sure it's public or serialized
    public GameObject fixationSphere;
    public float radius = 2f;
    public float duration = 8f;

    void Start()
    {
        if (fixationSphere == null)
        {
            Debug.LogError("Fixation sphere not assigned!");
            return;
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            // Generate a random direction in front of the camera
            Vector2 randomCircle = Random.insideUnitCircle.normalized * radius;
            Vector3 offset = cam.transform.right * randomCircle.x + cam.transform.up * randomCircle.y;
            fixationSphere.transform.position = cam.transform.position + cam.transform.forward * 2f + offset;
        }
        else
        {
            fixationSphere.transform.position = Random.onUnitSphere * radius;
        }

        Invoke(nameof(LoadNextScene), duration);
    }

    void LoadNextScene()
    {
        logger.SaveToCSV();
        SceneManager.LoadScene("EyeTrackerreal");
    }
}
