using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils {
    public class EasyNavigator : MonoBehaviour
    {
        
        public void GoToScene(string sceneName)
        {
            if (!FadeToBlack.Instance.IsFadingOut())
            {
                FadeToBlack.Instance.FadeOut(2f, () => SceneManager.LoadScene(sceneName));
            }
        }

        public void GoToSceneFadeMusic(string sceneName)
        {
            if (!FadeToBlack.Instance.IsFadingOut())
            {
                FMODMusicPlayer.Instance.StopThenDestroy();
                FadeToBlack.Instance.FadeOut(4f, () => SceneManager.LoadScene(sceneName));
            }
        }
    }
}