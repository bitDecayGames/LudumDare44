using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;

public partial class FMODMusicPlayer
{
    public bool SetPlaybackState(FMODSongState state)
    {
        bool songStateModified = false;
		
        GetAndUpdatePlaybackStateOfSong();

        switch (state)
        {
            case FMODSongState.Play:
                if (PlaybackStateDataContainer == PLAYBACK_STATE.STOPPED)
                {
                    _eventInstance = RuntimeManager.CreateInstance(BuildEventString(songName));
                    RESULT result = _eventInstance.start();
                    if (result != RESULT.OK)
                    {
                        throw new Exception(string.Format("Unable to start song: {0}", result));
                    }
                    songStateModified = true;
                }

                break;
            case FMODSongState.Stop:
                if (PlaybackStateDataContainer == PLAYBACK_STATE.PLAYING || PlaybackStateDataContainer == PLAYBACK_STATE.STARTING)
                {
                    RESULT result = _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    if (result != RESULT.OK)
                    {
                        throw new Exception(string.Format("Unable to stop song: {0}", result));
                    }
                    songStateModified = true;
                }

                break;
            case FMODSongState.StopImmediate:
                if (PlaybackStateDataContainer == PLAYBACK_STATE.PLAYING || PlaybackStateDataContainer == PLAYBACK_STATE.STARTING)
                {
                    RESULT result = _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    if (result != RESULT.OK)
                    {
                        throw new Exception(string.Format("Unable to stop song: {0}", result));
                    }
                    songStateModified = true;
                }
    
                break;
            }
        return songStateModified;
    }

    public void StopThenDestroy()
    {   
        _destroyWhenStopped = true;
        RESULT result = _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if (result != RESULT.OK)
        {
            throw new Exception("Unable to tell song to fade out: " + result);
        }
    }
    
    public void SetSong(Songs.SongName songName)
    {
        this.songName = songName;
    }

    public void Mute()
    {
        RESULT result = _eventInstance.getVolume(out _originalSongVolume, out CurrentVolumeReadOnly);
        if (result != RESULT.OK)
        {
            throw new Exception("Unable to get original volume of song before muting: " + result);
        }
        
        result = _eventInstance.setVolume(0);
        if (result != RESULT.OK)
        {
            throw new Exception("Unable to mute song: " + result);
        }
    }
    
    public void Unmute()
    {
        if (_originalSongVolume == 0)
        {
            throw new Exception("Unable to unmute song. Song was never muted or it had a volume of 0 when it was muted");
        }
        
        RESULT result = _eventInstance.setVolume(_originalSongVolume);
        if (result != RESULT.OK)
        {
            throw new Exception("Unable to unmute song: " + result);
        }
    }
    
    public void SetParameter(ParametersListEnum.Parameters parameter, float parameterValue)
    {
        ParameterInstance parameterInstance;
        RESULT result = _eventInstance.getParameter(parameter.ToString(), out parameterInstance);
        if (result != RESULT.OK)
        {
            throw new Exception(string.Format("Error returned when GETTING parameter of FMOD EventInstance: {0}", result));
        }
        result = _eventInstance.setParameterValue(parameter.ToString(), parameterValue);
        if (result != RESULT.OK)
        {
            throw new Exception(string.Format("Error returned when SETTING parameter of FMOD EventInstance: {0}", result));
        }
    }

    public void SetVolume(float volumeLevel)
    {
        RESULT result = _eventInstance.setVolume(volumeLevel);
        if (result != RESULT.OK)
        {
            throw new Exception("Unable to change volume of music: " + result);
        }
    }
}