using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public partial class FMODMusicPlayer : MonoBehaviour
{
	public static string FMODMusicManagerGameObjectName = "FMODMusicManager";
	private static string MusicFolderInFMODBank = "Music";

	public bool PlayOnSceneStart = true;
	public Songs.SongName songName;
	public PLAYBACK_STATE CurrentSongState;
	public bool TriggerSongToStart;
	public bool TriggerSongToStop;
	public ParametersListEnum.Parameters Parameter;
	public float Value;
	public bool SendParameterUpdate;
	public float CurrentVolumeReadOnly;

	private float _originalSongVolume;
	private PLAYBACK_STATE PlaybackStateDataContainer;
	private EventInstance _eventInstance;
	private bool _destroyWhenStopped;

	public enum FMODSongState
	{
		Play = 717427940,
		Stop = 794218840,
		StopImmediate = 697355509
	}
	
	private static FMODMusicPlayer instance;
	public static FMODMusicPlayer Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject();
				instance = gameObject.AddComponent<FMODMusicPlayer>();
				gameObject.name = "FMODMusicPlayer";
			}

			return instance;
		}
	}
	
	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
		
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		
		CurrentSongState = GetAndUpdatePlaybackStateOfSong();
		float tmpVolume;
		RESULT result = _eventInstance.getVolume(out tmpVolume, out CurrentVolumeReadOnly);
		if (result != RESULT.OK)
		{
			Debug.Log("Unable to get volume of track: " + result);
		}

		if (TriggerSongToStart)
		{
			SetPlaybackState(FMODSongState.Play);
			TriggerSongToStart = false;
		}
		if (TriggerSongToStop)
		{
			SetPlaybackState(FMODSongState.Stop);
			TriggerSongToStop = false;
		}

		if (SendParameterUpdate)
		{
			SetParameter(Parameter, Value);
			SendParameterUpdate = false;
		}

		if (_destroyWhenStopped && CurrentSongState == PLAYBACK_STATE.STOPPED)
		{
			Destroy(gameObject);
		}
	}

	private string BuildEventString(Songs.SongName songName)
	{
		return string.Format("event:/{0}/{1}", MusicFolderInFMODBank, songName.ToString());
	}
}
