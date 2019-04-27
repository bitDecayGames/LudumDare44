using UnityEngine;

public class MusicSelector : MonoBehaviour
{
    public Songs.SongName Song;
    
    private void Awake()
    {
        FMODMusicPlayer.Instance.PlaySongIfNoneAreCurrentlyPlaying(Song);
    }
}