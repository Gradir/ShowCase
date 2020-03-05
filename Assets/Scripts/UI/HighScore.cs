using System;

namespace ShowcaseGame
{
	public class Highscore : IComparable<Highscore>
	{
		public string playName;
		public int points;
		public float time;
		public Highscore(string name, int points, float time)
		{
			playName = name;
			this.points = points;
			this.time = time;
		}

		public int CompareTo(Highscore other)
		{
			if (points == other.points)
			{
				return time.CompareTo(other.time);
			}
			return other.points.CompareTo(points);
		}
	}
}