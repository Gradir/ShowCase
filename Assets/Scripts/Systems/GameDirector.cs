using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Zenject;

namespace ShowcaseGame
{
	public class GamePausedSignal : ASignal { }
	public class GameUnPausedSignal : ASignal { }
	public class GameStartedSignal : ASignal { }
	public class WinScreenShownSignal : ASignal { }
	/// <summary>
	/// it takes Enemy's gameObject InstanceID as identifier
	/// </summary>
	public class EnemyKilledSignal : ASignal<int> { }
	public class HurtPlayerSignal : ASignal { }
	public class GameDirector : MonoBehaviour
	{
		[SerializeField] private AudioMixer audioMixer = null;
		[SerializeField] private AudioSource thisAudioSource = null;
		[SerializeField] private AudioManager audioManager = null;
		[SerializeField] private float secondsForPointsPenalty = 30f;
		[SerializeField] private int timePenalty = -50;
		[SerializeField] private int pointsForEnemy = 100;
		[SerializeField] private int pointsForKey = 500;
		[SerializeField] private int pointsForWin = 1000;
		[SerializeField] private int maxPowerUps = 2;
		public int keysNeeded = 5;

		private string savePath;
		private List<Platform> platforms = new List<Platform>();
		private UIManager uiManager;
		private Enemy.EnemyPool enemyPool;
		private PowerUp.PowerUpPool powerPool;
		private int platformsCount = 6;
		private Player player;
		private char hashChar;

		// Saves etc.
		private bool powerUpSprintUnlocked = false;
		private bool powerUpDoubleJumpUnlocked = false;
		private List<Enemy> enemiesSpawned = new List<Enemy>();
		private int playerHealth = 5;
		private int keysCollected = 0;
		private int pointsCount = 0;
		private float timeElapsed = 0f;

		[Inject]
		private void Init(UIManager uIManager, AudioManager audioManager, Player p, Enemy.EnemyPool enemyPool, PowerUp.PowerUpPool powerUpPool)
		{
			this.audioManager = audioManager;
			this.enemyPool = enemyPool;
			player = p;
			powerPool = powerUpPool;
			uiManager = uIManager;
		}

		private void Start()
		{
			hashChar = StringDatabase.hashChar;
			Time.timeScale = 0;
			savePath = Application.dataPath + MethodNamesDatabase.savePathString;

			DOTween.SetTweensCapacity(500, 10);
			InvokeRepeating(MethodNamesDatabase.removeString, secondsForPointsPenalty, secondsForPointsPenalty);
			Signals.Get<EnemyKilledSignal>().AddListener(ReactOnEnemyKilled);
			Signals.Get<KeyFoundSignal>().AddListener(ReactOnKeyCollected);
			Signals.Get<TriedToOpenChestSignal>().AddListener(WinGame);
			Signals.Get<HurtPlayerSignal>().AddListener(HurtPlayer);
			Signals.Get<GameStartedSignal>().AddListener(ReactOnGameStarted);
			Signals.Get<WinScreenShownSignal>().AddListener(ReactOnWinScreenShown);
			Signals.Get<GamePausedSignal>().AddListener(ReactOnPause);

			Signals.Get<PowerUpAddedSignal>().AddListener(ReactOnPowerUpAdded);
		}

		private void OnDestroy()
		{
			DOTween.KillAll();
			CancelInvoke();

			Signals.Get<EnemyKilledSignal>().RemoveListener(ReactOnEnemyKilled);
			Signals.Get<KeyFoundSignal>().RemoveListener(ReactOnKeyCollected);
			Signals.Get<TriedToOpenChestSignal>().RemoveListener(WinGame);
			Signals.Get<HurtPlayerSignal>().RemoveListener(HurtPlayer);
			Signals.Get<GameStartedSignal>().RemoveListener(ReactOnGameStarted);
			Signals.Get<WinScreenShownSignal>().RemoveListener(ReactOnWinScreenShown);
			Signals.Get<GamePausedSignal>().RemoveListener(ReactOnPause);

			Signals.Get<PowerUpAddedSignal>().RemoveListener(ReactOnPowerUpAdded);
		}

		private void Update()
		{
			timeElapsed += Time.deltaTime;
		}

		public void RegisterPlatform(Platform p)
		{
			platforms.Add(p);
			if (platforms.Count == platformsCount)
			{
				SpawnPowerUpsOnPlatforms();
			}
		}

		public void RestartGame()
		{
			SceneManager.LoadScene(0);
		}

		private void ReactOnGameStarted()
		{
			Time.timeScale = 1;
			thisAudioSource.Play();
		}

		private void ReactOnWinScreenShown()
		{
			Time.timeScale = 0;
			thisAudioSource.DOFade(0, 1).OnComplete(() => thisAudioSource.Stop()).SetUpdate(true);
		}

		public PowerUp SpawnPowerUp(Vector3 pos, PowerUPType type)
		{
			var p = powerPool.Spawn(pos, type);
			return p;
		}

		public Enemy SpawnEnemy(Vector3 pos)
		{
			var e = enemyPool.Spawn(pos);
			enemiesSpawned.Add(e);
			return e;
		}

		private void HurtPlayer()
		{
			audioManager.PlaySFx(SFXType.Hit);
			playerHealth--;
			if (playerHealth <= 0)
			{
				RestartGame();
			}
		}

		private void SpawnPowerUpsOnPlatforms()
		{
			List<int> chosenPlatformIndexes = new List<int>();
			int iterations = 32;
			for (int i = 0; i < maxPowerUps; i++)
			{
				int randomPlatformIndex = Random.Range(0, platforms.Count);
				while (CheckIfIndexIsPartOfList(randomPlatformIndex, chosenPlatformIndexes) && iterations > 0)
				{
					iterations--;
					randomPlatformIndex = Random.Range(0, platforms.Count);
				}
				chosenPlatformIndexes.Add(randomPlatformIndex);
				Vector3 randomPos = platforms[randomPlatformIndex].GetRandomPositionOnPlatform();
				if (i % 2 == 0)
				{
					// even numbers spawn double jump
					SpawnPowerUp(randomPos, PowerUPType.DoubleJump);
				}
				else
				{
					// odd numbers spawn sprint powerup
					SpawnPowerUp(randomPos, PowerUPType.Sprint);
				}
			}
		}

		private bool CheckIfIndexIsPartOfList(int index, List<int> list)
		{
			foreach (var i in list)
			{
				if (i == index)
				{
					return true;
				}
			}
			return false;
		}

		private void RemovePointsByTime()
		{
			AddOrRemovePoints(timePenalty);
			SendPointsToUIManager();
		}

		private void ReactOnEnemyKilled(int notImportant)
		{
			AddOrRemovePoints(pointsForEnemy);
			// 15% for Slo Mo!
			if (Random.Range(0, 99) < 15)
			{
				DOVirtual.DelayedCall(0.5f, () => TurnSlowMotionOnOrOff(true)).SetUpdate(true);
				DOVirtual.DelayedCall(2.5f, () => TurnSlowMotionOnOrOff(false)).SetUpdate(true);
			}
		}

		// For saves
		public void ReactOnPowerUpAdded(PowerUPType powerUPType)
		{
			switch (powerUPType)
			{
				case PowerUPType.DoubleJump:
					audioManager.PlaySFx(SFXType.PowerUpJump);
					powerUpDoubleJumpUnlocked = true;
					break;
				case PowerUPType.Sprint:
					audioManager.PlaySFx(SFXType.PowerUpSprint);
					powerUpSprintUnlocked = true;
					break;
			}
		}

		private void OnApplicationQuit()
		{
			File.WriteAllText(savePath,
				powerUpSprintUnlocked.ToString() + hashChar +
				powerUpDoubleJumpUnlocked.ToString() + hashChar +
				enemiesSpawned.Count.ToString() + hashChar +
				playerHealth.ToString() + hashChar +
				keysCollected.ToString() + hashChar +
				pointsCount.ToString() + hashChar +
				timeElapsed.ToString() + hashChar
				);
			string previousText = File.ReadAllText(savePath);
			string combinedEnemyHpString = string.Empty;
			foreach (var e in enemiesSpawned)
			{
				combinedEnemyHpString += e.hp.ToString() + hashChar;
			}
			File.WriteAllText(savePath, previousText + combinedEnemyHpString);
		}

		private void ReactOnPause()
		{
			Time.timeScale = 0;
		}

		private void TurnSlowMotionOnOrOff(bool on)
		{
			audioMixer.DOSetFloat(MethodNamesDatabase.pitchString, on ? 0.5f : 1f, 0.4f).SetUpdate(true);

			Time.timeScale = on ? 0.25f : 1f;
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
		}

		private void ReactOnKeyCollected(Vector3 notImportant)
		{
			keysCollected++;
			uiManager.UpdateCollectedKeys(keysCollected);
			AddOrRemovePoints(pointsForKey);
		}

		private void AddOrRemovePoints(int value)
		{
			pointsCount += value;
			SendPointsToUIManager();
		}

		private void SendPointsToUIManager()
		{
			Signals.Get<PointsUIUpdateSignal>().Dispatch(pointsCount);
		}

		public bool CheckKeys()
		{
			if (keysCollected >= keysNeeded)
			{
				return true;
			}
			return false;
		}

		private void WinGame()
		{
			if (CheckKeys())
			{
				AddOrRemovePoints(pointsForWin);
				StartConfig.GetStartConfig().SaveHiScore(pointsCount, timeElapsed);
				uiManager.ShowWinScreen(timeElapsed);
			}
			else
			{
				int diff = keysNeeded - keysCollected;
				if (diff == 1)
				{
					uiManager.ShowNotification(MethodNamesDatabase.lastKeyString);
				}
				else
				{
					uiManager.ShowNotification(MethodNamesDatabase.youNeedString + diff + MethodNamesDatabase.keysString);
				}
			}
		}
	}

}