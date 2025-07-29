using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AlternatingBallsOnce : MonoBehaviour
{
    public GameObject ball1;
    public GameObject ball2;

    private Material mat1, mat2;
    public float radius = 2f;
    public float duration = 8f;

    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private static readonly int ModeProp = Shader.PropertyToID("_Mode");
    private static readonly int SrcBlendProp = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendProp = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWriteProp = Shader.PropertyToID("_ZWrite");

    void Start()
    {
        if (ball1 == null || ball2 == null)
        {
            Debug.LogError("Ball objects not assigned!");
            return;
        }

        mat1 = ball1.GetComponent<Renderer>().material;
        mat2 = ball2.GetComponent<Renderer>().material;

        Transform cam = Camera.main?.transform;
        if (cam != null)
        {
            Vector3 basePosition = cam.position + cam.forward * 2f;
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = cam.right * randomCircle.x + cam.up * randomCircle.y;
            ball1.transform.position = basePosition + offset;
            ball2.transform.position = basePosition - offset;
        }
        else
        {
            Vector3 basePosition = Vector3.forward * 2f;
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = Vector3.right * randomCircle.x + Vector3.up * randomCircle.y;
            ball1.transform.position = basePosition + offset;
            ball2.transform.position = basePosition - offset;
        }

        SetAlpha(mat1, 1f);
        SetAlpha(mat2, 0f);

        StartCoroutine(AlternateFor30Seconds());
        Invoke(nameof(LoadNextScene), duration);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("EyeTrackerreal");
    }

    IEnumerator AlternateFor30Seconds()
    {
        float elapsed = 0f;
        while (elapsed < 30f)
        {
            yield return new WaitForSeconds(10f);
            elapsed += 10f;

            yield return Fade(mat1, 1f, 0f, 2f);
            yield return Fade(mat2, 0f, 1f, 2f);

            SetColor(mat2, 1, 0, 0, 1); yield return new WaitForSeconds(1f); // red
            SetColor(mat2, 1, 1, 0, 1); yield return new WaitForSeconds(1f); // yellow
            SetColor(mat2, 0, 0, 1, 1); yield return new WaitForSeconds(1f); // blue
            elapsed += 5f;

            yield return Fade(mat2, 1f, 0f, 2f);
            yield return Fade(mat1, 0f, 1f, 2f);
            elapsed += 4f;
        }
    }

    IEnumerator Fade(Material mat, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color c = mat.GetColor(ColorProp);
        while (time < duration)
        {
            c.a = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            mat.SetColor(ColorProp, c);
            time += Time.deltaTime;
            yield return null;
        }
        c.a = endAlpha;
        mat.SetColor(ColorProp, c);
    }

    void SetAlpha(Material mat, float alpha)
    {
        Color c = mat.GetColor(ColorProp);
        c.a = alpha;
        mat.SetColor(ColorProp, c);

        // Only set render properties once per material instance
        mat.SetFloat(ModeProp, 2);
        mat.SetInt(SrcBlendProp, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt(DstBlendProp, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt(ZWriteProp, 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    void SetColor(Material mat, float r, float g, float b, float a)
    {
        Color c = new Color(r, g, b, a);
        mat.SetColor(ColorProp, c);
    }
}
