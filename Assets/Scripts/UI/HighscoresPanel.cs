using UnityEngine;

namespace ShowcaseGame
{
	public class HighscoresPanel : MonoBehaviour
	{
		[SerializeField] private HiScoreUI hiScorePrefab = null;

		private void Start()
		{
			StartConfig.GetStartConfig().FillHiScores(this);
		}

		public void Fill(string index, string gameName, string points, string timeString)
		{
			var comp = Instantiate(hiScorePrefab, transform);
			comp.AddHiScore(index, gameName, points, timeString);
		}
	}
}