using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class Platform : MonoBehaviour
	{
		[SerializeField] private Bounds bounds;
		[SerializeField] private int limitEnemies = 5;
		[SerializeField] private float secondsBetweenSpawns = 3;
		[SerializeField] private bool showBounds = true;
		public bool spawnEnemiesGradually = true;

		private bool coroutineStopped;
		private List<int> enemyIdsBornHere = new List<int>();
		private bool justSpawned = false;
		private GameDirector gameDirector;

		[Inject]
		private void Init(GameDirector gameDirector)
		{
			this.gameDirector = gameDirector;
		}

		private void Start()
		{
			Signals.Get<EnemyKilledSignal>().AddListener(CheckIfEnemyWasBornHere);

			gameDirector.RegisterPlatform(this);
		}

		private void CheckIfEnemyWasBornHere(int enemyId)
		{
			bool found = false;
			foreach (var e in enemyIdsBornHere)
			{
				if (e == enemyId)
				{
					found = true;
					break;
				}
			}
			if (found)
			{
				enemyIdsBornHere.Remove(enemyId);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Player>() && justSpawned == false)
			{
				justSpawned = true;
				TryToSpawnEnemy();
				if (spawnEnemiesGradually)
				{
					StartSpawningEnemies();
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				justSpawned = false;
				if (spawnEnemiesGradually)
				{
					StopSpawningEnemies();
				}
			}
		}

		private void StartSpawningEnemies()
		{
			coroutineStopped = false;
			StartCoroutine(SpawnEnemiesInTime());
		}

		private void StopSpawningEnemies()
		{
			coroutineStopped = true;
			StopCoroutine(SpawnEnemiesInTime());
		}

		private IEnumerator SpawnEnemiesInTime()
		{
			while (enemyIdsBornHere.Count - 1 < limitEnemies && coroutineStopped == false)
			{
				yield return new WaitForSecondsRealtime(secondsBetweenSpawns);
				TryToSpawnEnemy();
			}
		}

		private void TryToSpawnEnemy()
		{
			if (enemyIdsBornHere.Count - 1 >= limitEnemies)
			{
				return;
			}
			
			var enemy = gameDirector.SpawnEnemy(GetRandomPositionOnPlatform());
			enemyIdsBornHere.Add(enemy.GetInstanceID());
		}

		public Vector3 GetRandomPositionOnPlatform()
		{
			return transform.position + RandomPointInBounds(bounds);
		}

		private Vector3 RandomPointInBounds(Bounds bounds)
		{
			return new Vector3(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z)
			);
		}

		private void OnDrawGizmos()
		{
			if (showBounds)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
			}
		}
	}
}