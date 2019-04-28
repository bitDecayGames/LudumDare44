using System;
public class Scenes
{
	public const string DontDestroyOnLoad = "DontDestroyOnLoad";
	public const string TitleScreen = "TitleScreen";
	public const string Credits = "Credits";
	public const string BitdecaySplash = "BitdecaySplash";
	public const string GameJamSplash = "GameJamSplash";
	public const string NewScene = "NewScene";
	public const string JakesWorld = "JakesWorld";
	public const string LoganWorld = "LoganWorld";
	public const string DebugPlayerAnimations = "DebugPlayerAnimations";
	public const string TannersWorldMine = "TannersWorldMine";
	public enum SceneEnum
	{
		TitleScreen = 98,
		Credits = 206,
		BitdecaySplash = 144,
		GameJamSplash = 253,
		NewScene = 24,
		JakesWorld = 246,
		LoganWorld = 249,
		DebugPlayerAnimations = 103,
		TannersWorldMine = 108,
	}
	public static string GetSceneNameFromEnum(SceneEnum sceneEnum)
	{
		switch (sceneEnum)
		{
			case SceneEnum.TitleScreen:
				return TitleScreen;
			case SceneEnum.Credits:
				return Credits;
			case SceneEnum.BitdecaySplash:
				return BitdecaySplash;
			case SceneEnum.GameJamSplash:
				return GameJamSplash;
			case SceneEnum.NewScene:
				return NewScene;
			case SceneEnum.JakesWorld:
				return JakesWorld;
			case SceneEnum.LoganWorld:
				return LoganWorld;
			case SceneEnum.DebugPlayerAnimations:
				return DebugPlayerAnimations;
			case SceneEnum.TannersWorldMine:
				return TannersWorldMine;
			default:
				throw new Exception("Unable to resolve scene name for: " + sceneEnum);
		}
	}
}
