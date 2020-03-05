using UnityEngine;

namespace ShowcaseGame
{
	[System.Serializable]
	public class GameSceneSettings
	{
		[Header("Managers")]
		public GameObject gameDirector;
		public GameObject audioManager;
		public GameObject inputManager;
		public GameObject uiManager;
		[Header("Prefabs")]
		public GameObject enemyPrefab;
		public GameObject powerUpPrefab;
		public GameObject projectilePrefab;
		public GameObject explosionPrefab;
	}
}