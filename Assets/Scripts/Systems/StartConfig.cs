using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace ShowcaseGame
{
	public class StartConfig : MonoBehaviour
	{
		private string hiScoresPath;

		private static StartConfig _instance;
		private string gameName;
		private List<Highscore> allHighScores = new List<Highscore>();
		private HighscoresPanel hScores;

		void Awake()
		{
			hiScoresPath = Application.dataPath + MethodNamesDatabase.pathString;
			if (_instance != null && _instance != this)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				_instance = this;
				DontDestroyOnLoad(this);
			}
			CheckHighscoresFileAndCreate();
		}

		public static StartConfig GetStartConfig()
		{
			if (_instance == null)
			{
				var ob = new GameObject(MethodNamesDatabase.startConfigName);
				ob.AddComponent<StartConfig>();
				return ob.GetComponent<StartConfig>();
			}
			return _instance;
		}

		public void ChangeGameName(TMP_InputField field)
		{
			gameName = field.text;
		}

		public void SetOldGameName()
		{
			var field = FindObjectOfType<TMP_InputField>();
			if (field != null)
			{
				field.text = gameName;
			}
		}

		public void SaveHiScore(int score, float time)
		{
			if (gameName == null || gameName == string.Empty)
			{
				gameName = MethodNamesDatabase.defaultGameName;
			}
			string previousScores = ReadHighscores();
			File.WriteAllText(hiScoresPath, previousScores + 
				StringDatabase.hashChar + 
				gameName + 
				StringDatabase.colonChar +
				score.ToString() + 
				StringDatabase.hashChar + 
				time.ToString(StringDatabase.hashHashString));
		}

		private string ReadHighscores()
		{
			return File.ReadAllText(hiScoresPath);
		}

		private void CheckHighscores()
		{
			string[] hiscoresPlays = ReadHighscores().Split(new char[] { StringDatabase.hashChar, StringDatabase.colonChar }, StringSplitOptions.RemoveEmptyEntries);
			allHighScores.Clear();

			for (int i = 0; i < hiscoresPlays.Length; i += 3)
			{
				allHighScores.Add(new Highscore(hiscoresPlays[i], int.Parse(hiscoresPlays[i + 1]), float.Parse(hiscoresPlays[i + 2])));
			}
		}

		public void FillHiScores(HighscoresPanel panel)
		{
			hScores = panel;
			SetOldGameName();
			CheckHighscores();
			foreach (Transform c in hScores.transform)
			{
				if (c.name != MethodNamesDatabase.clearString)
				{
					Destroy(c.gameObject);
				}
			}
			if (hScores == null)
			{
				return;
			}
			if (allHighScores.Count == 0)
			{
				hScores.Fill(string.Empty, MethodNamesDatabase.emptyHighScoreList, string.Empty, string.Empty);
				return;
			}
			allHighScores.Sort();
			for (int i = 0; i < allHighScores.Count; i++)
			{
				var score = allHighScores[i];
				if (hScores.transform.childCount < 10)
				{
					hScores.Fill((i + 1).ToString(), score.playName, score.points.ToString(), score.time.ToString(StringDatabase.hashHashString) + StringDatabase.sChar);
				}
				else
				{
					break;
				}
			}
		}

		private void CheckHighscoresFileAndCreate()
		{
			if (File.Exists(hiScoresPath) == false)
			{
				File.Create(hiScoresPath);
			}
		}

		public void ClearHighscores()
		{
			File.WriteAllText(hiScoresPath, string.Empty);
			FillHiScores(hScores);
		}
	}
}