using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

namespace ShowcaseGame
{
	public enum WeaponType
	{
		FastGun,
		RailGun,
		Bazooka
	}
	public class Player : MonoBehaviour
	{
		public Transform projectileStartPoint;
		[SerializeField] private float coolDownWeapon0 = 0.1f;
		[SerializeField] private float coolDownWeapon1 = 2.5f;
		[SerializeField] private float coolDownWeapon2 = 1f;
		[SerializeField] private AudioClip gunshotSound = null;
		[SerializeField] private AudioClip railGunSound = null;
		[SerializeField] private AudioClip bazookaSound = null;
		[SerializeField] private Animator thisAnimator = null;
		[SerializeField] private AudioSource thisAudioSource = null;

		[Header("Projectile Prefabs")]
		[SerializeField] private Projectile projectileBullet = null;
		[SerializeField] private Projectile projectileRocket = null;
		[SerializeField] private LineRenderer railGunLine = null;

		[Header("Third Person Components")]
		[SerializeField] private ThirdPersonCharacter thirdPersonCharacter = null;
		[SerializeField] private ThirdPersonUserControl thirdPersonUserControl = null;

		private RectTransform crossHair;
		private Color2 startLine = new Color2(Color.green, Color.white);
		private Color2 endLine = new Color2(new Color(1,1,1,0), new Color(0, 1, 0, 0));
		private Vector3 offsetY = new Vector3(0, 0.25f, 0);
		private WeaponType weaponUsed = WeaponType.FastGun;
		private Dictionary<WeaponType, float> weaponIdToCooldown = new Dictionary<WeaponType, float>();
		private Dictionary<WeaponType, Projectile> weaponIdToProjectile = new Dictionary<WeaponType, Projectile>();
		private bool shootingCooldowned;
		private Projectile.ProjectilePool projectilePool;



		[Inject]
		private void Init(UIManager uIManager, Projectile.ProjectilePool projectilePool)
		{
			this.projectilePool = projectilePool;
			crossHair = uIManager.crossHair;
		}

		private void Start()
		{
			weaponIdToCooldown.Add(WeaponType.FastGun, coolDownWeapon0);
			weaponIdToCooldown.Add(WeaponType.RailGun, coolDownWeapon1);
			weaponIdToCooldown.Add(WeaponType.Bazooka, coolDownWeapon2);
			weaponIdToProjectile.Add(WeaponType.FastGun, projectileBullet);
			weaponIdToProjectile.Add(WeaponType.Bazooka, projectileRocket);
			Signals.Get<PowerUpAddedSignal>().AddListener(ReactOnPowerUpAdded);
			Signals.Get<SwitchedWeaponSignal>().AddListener(ReactOnWeaponSwitched);
		}

		private void OnDestroy()
		{
			Signals.Get<PowerUpAddedSignal>().RemoveListener(ReactOnPowerUpAdded);
			Signals.Get<SwitchedWeaponSignal>().RemoveListener(ReactOnWeaponSwitched);
		}

		private void ReactOnWeaponSwitched(WeaponType type)
		{
			weaponUsed = type;
		}

		public void Shooting()
		{
			if (shootingCooldowned)
			{
				return;
			}
			shootingCooldowned = true;
			Invoke(MethodNamesDatabase.removeCooldownString, weaponIdToCooldown[weaponUsed]);
			thisAnimator.SetTrigger(MethodNamesDatabase.shootString);

			Ray shootingRay = Camera.main.ScreenPointToRay(crossHair.transform.position);
			Vector3 origin = projectileStartPoint.position;
			Vector3 direction = shootingRay.direction;
			if (weaponUsed == WeaponType.RailGun)
			{
				Vector3[] positions = new Vector3[] { origin, shootingRay.direction * 500 };
				railGunLine.SetPositions(positions);
				railGunLine.enabled = true;
				railGunLine.DOColor(startLine, endLine, 1.5f);
				Invoke(MethodNamesDatabase.hideLineString, 1.5f);
				thisAudioSource.clip = railGunSound;
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(positions[0], positions[1], out hit, LayerMask.NameToLayer("Enemy")))
				{
					var en = hit.collider.GetComponent<Enemy>();
					if (en != null)
					{
						en.ModifyHp(WeaponType.RailGun, -1000);
					}
				}
			}
			else
			{
				projectilePool.Spawn(origin, direction, weaponUsed);
				thisAudioSource.clip = weaponUsed == WeaponType.Bazooka? bazookaSound : gunshotSound;
			}
			thisAudioSource.Play();
		}

		private void HideLine()
		{
			railGunLine.enabled = false;
			railGunLine.DOColor(endLine, startLine, 1.5f);
		}

		private void RemoveCooldown()
		{
			shootingCooldowned = false;
		}

		public void ReactOnPowerUpAdded(PowerUPType powerUPType)
		{
			switch (powerUPType)
			{
				case PowerUPType.DoubleJump:
					thirdPersonCharacter.doubleJumpUnlocked = true;
					break;
				case PowerUPType.Sprint:
					thirdPersonUserControl.sprintUnlocked = true;
					break;
			}
		}
	}

}