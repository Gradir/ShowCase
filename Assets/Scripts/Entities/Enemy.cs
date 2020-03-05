using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using Zenject;

namespace ShowcaseGame
{
	public class Enemy : MonoBehaviour
	{
		[SerializeField] private AudioClip[] bloodSFX = null;
		[SerializeField] private AudioClip[] dyingSFX = null;
		[SerializeField] private Rigidbody[] rigidbodies = null;
		[SerializeField] private Collider[] ragDollColliders = null;
		[SerializeField] private AudioSource thisAudiosource = null;
		[SerializeField] private AICharacterControl aICharacter = null;
		[SerializeField] private NavMeshAgent agent = null;
		[SerializeField] private Animator thisAnimator = null;
		[SerializeField] private Rigidbody thisRigidbody = null;
		[SerializeField] private Rigidbody rigidbodyToPull = null;
		[SerializeField] private CapsuleCollider capsuleCollider = null;
		public float maxHp = 500;
		public float hp;
		public EnemyPool enemyMemoryPool;
		private Player player;
		private float cooldown = 2f;
		private bool attackCooldowned = false;
		private bool playerStillInRange = false;
		private bool isDying = false;

		[Inject]
		private void Init(Player p)
		{
			player = p;
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				playerStillInRange = true;
				if (attackCooldowned == false)
				{
					thisAnimator.SetTrigger(MethodNamesDatabase.attackingString);
					attackCooldowned = true;
					Invoke(MethodNamesDatabase.endCooldownString, cooldown);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				playerStillInRange = false;
			}
		}

		private void EndCooldown()
		{
			attackCooldowned = false;
			thisAnimator.ResetTrigger(MethodNamesDatabase.attackingString);
		}

		public void DealDamageToPlayer()
		{
			if (playerStillInRange)
			{
				Signals.Get<HurtPlayerSignal>().Dispatch();
			}
		}

		public void ModifyHp(WeaponType weaponType, float amount)
		{
			if (isDying)
			{
				return;
			}
			hp += amount;
			if (amount < 0)
			{
				thisAudiosource.clip = (bloodSFX[Random.Range(0, bloodSFX.Length)]);
				thisAudiosource.Play();
			}
			if (hp <= 0)
			{
				var heading = rigidbodyToPull.transform.position - player.projectileStartPoint.position;
				var distance = heading.magnitude;
				var direction = heading / distance;
				Die(direction);
			}
		}

		private void TurnRagdollOnOrOff(bool on)
		{
			capsuleCollider.enabled = !on;
			foreach (var r in rigidbodies)
			{
				r.isKinematic = !on;
			}
			foreach (var c in ragDollColliders)
			{
				c.enabled = on;
			}
		}

		private void Die(Vector3 directionToThrow)
		{
			isDying = true;
			agent.enabled = false;
			thisAnimator.enabled = false;
			aICharacter.enabled = false;
			thisRigidbody.isKinematic = true;
			TurnRagdollOnOrOff(true);

			thisAudiosource.clip = (dyingSFX[Random.Range(0, dyingSFX.Length)]);
			thisAudiosource.Play();

			Signals.Get<EnemyKilledSignal>().Dispatch(gameObject.GetInstanceID());

			rigidbodyToPull.AddForce(directionToThrow * 20);
			Invoke(MethodNamesDatabase.killMeString, 5f);
		}

		private void KillMe()
		{
			TurnRagdollOnOrOff(false);
			transform.DOMoveY(-5, 8f).OnComplete(() => enemyMemoryPool.Despawn(this)).SetId(MethodNamesDatabase.movingBodyString);
		}

		public class EnemyPool : MonoMemoryPool<Vector3, Enemy>
		{
			protected override void OnCreated(Enemy item)
			{
				item.enemyMemoryPool = this;
			}

			protected override void OnDespawned(Enemy item)
			{
				DOTween.Kill(MethodNamesDatabase.movingBodyString);
				/*
				item.bloodSFX = null;
				item.dyingSFX = null;
				item.rigidbodies = null;
				item.ragDollColliders = null;
				item.thisAudiosource = null;
				item.aICharacter = null;
				item.thisAnimator = null;
				item.thisRigidbody = null;
				item.rigidbodyToPull = null;
				item.player = null;
				*/
				item.attackCooldowned = false;
				item.playerStillInRange = false;
				item.isDying = false;
				item.gameObject.SetActive(false);
			}

			protected override void Reinitialize(Vector3 pos, Enemy enemy)
			{
				enemy.attackCooldowned = false;
				enemy.playerStillInRange = false;
				enemy.isDying = false;
				enemy.thisRigidbody.isKinematic = false;
				enemy.thisAnimator.enabled = true;
				enemy.TurnRagdollOnOrOff(false);
				enemy.hp = enemy.maxHp;
				enemy.aICharacter.enabled = true;
				enemy.aICharacter.SetTarget(enemy.player.transform);
				enemy.transform.position = pos;

				enemy.agent.enabled = true;
				enemy.gameObject.SetActive(true);
			}
		}
	}
}