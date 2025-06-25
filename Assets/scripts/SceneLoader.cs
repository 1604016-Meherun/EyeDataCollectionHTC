using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // public SceneLoader sceneLoader;

    void Start()
    {
       // sceneLoader.LoadNewScene("fixation");
       Debug.Log("Button Clicked! Attempting to load scene...");
    }

    public void sceneChanger_fixation()
    {
        Debug.Log("Button Clicked! Attempting to load scene...");
        SceneManager.LoadScene("Fixation");
    }

    public void sceneChanger_smooth_pursuit()
    {
        Debug.Log("Button Clicked! Attempting to load scene...");
        SceneManager.LoadScene("smoothpursuit");
    }

    public void sceneChanger_saccade()
    {
        Debug.Log("Button Clicked! Attempting to load scene...");
        SceneManager.LoadScene("saccade");
    }
}
