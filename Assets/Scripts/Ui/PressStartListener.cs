using UnityEngine;
using Utils;

public class PressStartListener : MonoBehaviour
{

    public string Level;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.PressStart);
            GetComponent<EasyNavigator>().GoToSceneFadeMusic(Level);
        }
    }
}