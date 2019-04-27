using UnityEngine;

public class MenuController : MonoBehaviour
{
    private void Awake()
    {
        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.Sound);
    }
}