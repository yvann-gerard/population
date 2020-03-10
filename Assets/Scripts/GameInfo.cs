using System;
using UnityEngine;

[Serializable]
public class GameInfo : MonoBehaviour
{
	public static Int32 populationCount = 4;
	public static Bounds arenaBounds;
	public static PopulationStat[] populationStats;

	public class PopulationStat
	{
		public enum stat
		{
			speed,
			vision,
			stockage,
			bite,
			agression,
			endurance
		}

		public PopulationStat()
		{
			stats = new int[6];
		}

		public int[] stats;
	}
}
