using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class Explosion : MonoBehaviour
	{
		public ExplosionPool explosionPool;

		private void Start()
		{
			Invoke(MethodNamesDatabase.killMeString, 4f);
		}

		private void KillMe()
		{
			explosionPool.Despawn(this);
		}

		public class ExplosionPool : MonoMemoryPool<Vector3, Explosion>
		{
			protected override void OnCreated(Explosion item)
			{
				item.explosionPool = this;
			}

			protected override void OnDespawned(Explosion item)
			{
				item.CancelInvoke(MethodNamesDatabase.killMeString);
				item.gameObject.SetActive(false);
			}

			protected override void Reinitialize(Vector3 pos, Explosion ex)
			{
				ex.transform.position = pos;
			}
		}
	}

}