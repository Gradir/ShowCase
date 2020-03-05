using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class Projectile : MonoBehaviour
	{
		public WeaponType weaponType;
		[SerializeField] private GameObject bulletObject = null;
		[SerializeField] private GameObject rocketObject = null;
		[SerializeField] private Rigidbody thisRigidbody = null;
		[SerializeField] private TrailRenderer trailRenderer = null;
		[SerializeField] private float damage = 100;
		[SerializeField] private float velocityModifier = 10f;
		private bool propellItself;
		private Vector3 cachedDirection;
		private ProjectilePool projectilePool;
		private Explosion.ExplosionPool explosionPool;

		[Inject]
		private void Init(Explosion.ExplosionPool explosionPool)
		{
			this.explosionPool = explosionPool;
		}

		private void Configure(WeaponType type)
		{
			weaponType = type;
			bool isBazooka = type == WeaponType.Bazooka;
			propellItself = isBazooka;
			rocketObject.SetActive(isBazooka);
			bulletObject.SetActive(!isBazooka);
			velocityModifier = isBazooka ? 3000 : 4000;
			damage = isBazooka ? 100 : 25;
			float trailScale = isBazooka ? 1 : 0.1f;
			trailRenderer.startWidth = trailScale;
		}

		private void Update()
		{
			if (propellItself)
			{
				var forwardDirection = transform.position + cachedDirection;
				transform.LookAt(forwardDirection);
				thisRigidbody.AddForce(transform.forward * 10f);
			}
		}

		private void KillMe()
		{
			if (weaponType == WeaponType.Bazooka)
			{
				explosionPool.Spawn(transform.position);

				Collider[] affectedEntities = new Collider[8];
				Physics.OverlapSphereNonAlloc(transform.position, 3f, affectedEntities, LayerMask.GetMask(MethodNamesDatabase.enemyString));
				foreach (var entity in affectedEntities)
				{
					if (entity != null)
					{
						var enemy = entity.GetComponentInParent<Enemy>();
						if (enemy != null)
						{
							enemy.ModifyHp(WeaponType.Bazooka, -GetRandomisedDamage());
						}
					}
				}
			}
			projectilePool.Despawn(this);
		}

		private float GetRandomisedDamage()
		{
			return damage += Random.Range(-10f, 10f);
		}

		public void Shoot(Vector3 direction)
		{
			cachedDirection = direction;
			thisRigidbody.AddForce(direction * velocityModifier);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (gameObject.activeSelf == false)
			{
				return;
			}
			if (weaponType == WeaponType.Bazooka)
			{
				KillMe();
				return;
			}
			var potentialEnemy = other.GetComponent<Enemy>();
			if (potentialEnemy)
			{
				potentialEnemy.ModifyHp(weaponType, -GetRandomisedDamage());
				KillMe();
			}
		}

		public class ProjectilePool : MonoMemoryPool<Vector3, Vector3, WeaponType, Projectile>
		{
			protected override void OnCreated(Projectile item)
			{
				item.projectilePool = this;
			}

			protected override void OnDespawned(Projectile item)
			{
				item.CancelInvoke(MethodNamesDatabase.killMeString);
				item.trailRenderer.Clear();
				item.thisRigidbody.isKinematic = true;
				item.propellItself = false;
				item.enabled = false;
				item.gameObject.SetActive(false);
			}

			protected override void Reinitialize(Vector3 pos, Vector3 dir, WeaponType type, Projectile proj)
			{
				// some things should be in ScriptableObject or something, but well...
				proj.transform.position = pos;
				proj.thisRigidbody.isKinematic = false;
				proj.enabled = true;
				proj.Configure(type);
				proj.gameObject.SetActive(true);
				proj.Invoke(MethodNamesDatabase.killMeString, 8f);
				proj.Shoot(dir);
			}
		}
	}
}