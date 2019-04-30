using UnityEngine;

public class PlaySfx : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(soundName);
    }
}