using TMPro;
using UnityEngine;

namespace ShowcaseGame
{
	public class HiScoreUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI numbering = null;
		[SerializeField] private TextMeshProUGUI gameNameText = null;
		[SerializeField] private TextMeshProUGUI pointsText = null;
		[SerializeField] private TextMeshProUGUI timeStringText = null;

		public void AddHiScore(string index, string gameName, string points, string timeString)
		{
			numbering.text = index;
			gameNameText.text = gameName;
			pointsText.text = points;
			timeStringText.text = timeString;
		}
	}
}