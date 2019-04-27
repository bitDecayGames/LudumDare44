using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils {
    public class EasyNavigator : MonoBehaviour {

        public void GoToScene(string sceneName)
        {
            if (!FadeToBlack.Instance.IsFadingOut())
            {
                FadeToBlack.Instance.FadeOut(2f, () => SceneManager.LoadScene(sceneName));
                FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.Sound);
            }
        }
    }
}