using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    [Header("Buttons")]
    public Button fixationButton;
    public Button smoothPursuitButton;
    public Button saccadeButton;

    private Dictionary<string, Button> sceneButtonMap;
    private static HashSet<string> completedScenes = new HashSet<string>();

    void Start()
    {
        // Map buttons to scene names
        sceneButtonMap = new Dictionary<string, Button>
        {
            { "Fixation", fixationButton },
            { "smoothpursuit", smoothPursuitButton },
            { "saccade", saccadeButton }
        };

        // Check PlayerPrefs for last played scene
        string lastScene = PlayerPrefs.GetString("LastScene", "");
        if (!string.IsNullOrEmpty(lastScene))
        {
            completedScenes.Add(lastScene);
            PlayerPrefs.DeleteKey("LastScene");
        }

        // Disable completed scene buttons and color red
        foreach (var kvp in sceneButtonMap)
        {
            if (completedScenes.Contains(kvp.Key))
            {
                kvp.Value.interactable = false;
                kvp.Value.GetComponent<Image>().color = Color.red;
            }
        }

        // Check if all 3 scenes are completed
        if (completedScenes.Count >= 3)
        {
            Debug.Log("All scenes completed. Quitting and clearing PlayerPrefs...");
            PlayerPrefs.DeleteAll(); // reset saved data
            completedScenes.Clear(); // reset runtime memory

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // will trigger restart if wrapped externally
#endif
        }
    }

    public void sceneChanger_fixation()
    {
        PlayerPrefs.SetString("LastScene", "Fixation");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Fixation");
    }

    public void sceneChanger_smooth_pursuit()
    {
        PlayerPrefs.SetString("LastScene", "smoothpursuit");
        PlayerPrefs.Save();
        SceneManager.LoadScene("smoothpursuit");
    }

    public void sceneChanger_saccade()
    {
        PlayerPrefs.SetString("LastScene", "saccade");
        PlayerPrefs.Save();
        SceneManager.LoadScene("saccade");
    }
}
