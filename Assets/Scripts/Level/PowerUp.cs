using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public enum PowerUPType
	{
		DoubleJump,
		Sprint
	}
	public class PowerUpAddedSignal : ASignal<PowerUPType> { }
	public class PowerUp : MonoBehaviour
	{
		public PowerUPType powerUPType;
		[SerializeField] private MeshRenderer thisMeshRenderer = null;
		[SerializeField] private Material sprintMaterial = null;
		[SerializeField] private Material jumpMaterial = null;
		private PowerUpPool powerUpPool;

		private void Start()
		{
			transform.DORotate(new Vector3(0, 359, 0), 1.5f, RotateMode.FastBeyond360)
				.SetLoops(int.MaxValue, LoopType.Restart)
				.SetEase(Ease.Linear)
				.SetId(MethodNamesDatabase.tweenId);
		}

		public void ConfigureType(PowerUPType type)
		{
			powerUPType = type;
			thisMeshRenderer.material = type == PowerUPType.DoubleJump ? jumpMaterial : sprintMaterial;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				Signals.Get<PowerUpAddedSignal>().Dispatch(powerUPType);
				powerUpPool.Despawn(this);
			}
		}

		public class PowerUpPool : MonoMemoryPool<Vector3, PowerUPType, PowerUp>
		{
			protected override void OnCreated(PowerUp item)
			{
				item.powerUpPool = this;
			}

			protected override void OnDespawned(PowerUp item)
			{
				DOTween.Kill(MethodNamesDatabase.tweenId);
				item.gameObject.SetActive(false);
			}

			protected override void Reinitialize(Vector3 pos, PowerUPType type, PowerUp powerUp)
			{
				powerUp.transform.position = pos;
				powerUp.ConfigureType(type);
				powerUp.gameObject.SetActive(true);
			}
		}
	}

}