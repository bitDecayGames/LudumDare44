using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DelayedSceneTransition : MonoBehaviour
{
    public Scenes.SceneEnum Scene;
    public float DelayInSeconds;

    private void Update()
    {
        DelayInSeconds -= Time.deltaTime;
        if (DelayInSeconds <= 0)
        {
            FadeToBlack.Instance.FadeOut(1f, () => SceneManager.LoadScene(Scene.ToString()));
        }
    }
}