using FMOD.Studio;

public partial class FMODMusicPlayer
{
	public void PlaySongIfNoneAreCurrentlyPlaying(Songs.SongName songName)
	{
		PLAYBACK_STATE playbackState = GetAndUpdatePlaybackStateOfSong();
		if (playbackState == PLAYBACK_STATE.STOPPED)
		{
			SetSong(songName);
			SetPlaybackState(FMODSongState.Play);	
		}
	}
}
