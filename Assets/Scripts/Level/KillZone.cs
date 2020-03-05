using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class KillZone : MonoBehaviour
	{
		private Enemy.EnemyPool enemyPool;
		private Projectile.ProjectilePool projectilePool;
		[Inject]
		private void Init(Enemy.EnemyPool enemyPool, Projectile.ProjectilePool projectilePool)
		{
			this.enemyPool = enemyPool;
			this.projectilePool = projectilePool;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
			else
			{
				var en = other.GetComponentInParent<Enemy>();
				var pr = other.GetComponentInParent<Projectile>();
				if (en != null)
				{
					enemyPool.Despawn(en);
				}
				else if (pr != null)
				{
					projectilePool.Despawn(pr);
				}
				else
				{
					Destroy(other.gameObject);
				}
			}
		}
	}
}