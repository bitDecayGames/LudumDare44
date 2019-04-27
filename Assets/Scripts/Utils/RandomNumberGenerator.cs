using System;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Singleton that provides globally random numbers
/// </summary>
public class RandomNumberGenerator
{
	private Random random;
	
	private static RandomNumberGenerator instance;

	public static RandomNumberGenerator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new RandomNumberGenerator();
			}

			return instance;
		}
	}

	private RandomNumberGenerator()
	{
		random = new Random((int)DateTime.Now.Ticks);
	}

	/// <summary>
	/// Returns a 32-bit signed integer that is greater than or equal to 0, and less than maxValue; that is, the range 
	/// of return values ordinarily includes 0 but not maxValue. However, if maxValue equals 0, maxValue is returned.
	/// </summary>
	/// <param name="maxValue"></param>
	/// <returns></returns>
	public int GetRandomInt(int maxValue)
	{
		return random.Next(maxValue);
	}
	
	/// <summary>
	/// Returns a 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of 
	/// return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.
	/// </summary>
	/// <param name="maxValue"></param>
	/// <returns></returns>
	public int GetRandomInt(int minValue, int maxValue)
	{
		return random.Next(minValue, maxValue);
	}
	
	/// <summary>
	/// Returns float that is greater than or equal to minValue, and less than maxValue; that is, the range 
	/// of return values ordinarily includes minValue but not maxValue. However, if maxValue equals 0, maxValue is returned.
	/// </summary>
	/// <param name="maxValue"></param>
	/// <returns></returns>
	public float GetRandomFloat(float minValue, float maxValue)
	{
		var result = (float) random.NextDouble() * (maxValue - minValue) + minValue;
		return result;
	}
	
	/// <summary>
	/// Returns float that is greater than or equal to minValue, and less than maxValue; that is, the range 
	/// of return values ordinarily includes minValue but not maxValue. However, if maxValue equals 0, maxValue is returned.
	/// </summary>
	/// <param name="maxValue"></param>
	/// <returns></returns>
	public Vector3 GetRandomPoint(Vector3 minPoint, Vector3 maxPoint)
	{
		var xResult = (float) random.NextDouble() * (maxPoint.x - minPoint.x) + minPoint.x;
		var yResult = (float) random.NextDouble() * (maxPoint.y - minPoint.y) + minPoint.y;
		return new Vector3(xResult, yResult);
	}
}