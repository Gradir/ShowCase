using UnityEngine;

namespace ShowcaseGame
{
	public class Weapon : MonoBehaviour
	{
		[SerializeField] private MeshRenderer thisRenderer = null;
		[SerializeField] private Material[] weaponMaterials = null;

		private void Start()
		{
			Signals.Get<SwitchedWeaponSignal>().AddListener(ReactOnWeaponSwitched);
		}

		private void OnDestroy()
		{
			Signals.Get<SwitchedWeaponSignal>().RemoveListener(ReactOnWeaponSwitched);
		}

		private void ReactOnWeaponSwitched(WeaponType type)
		{
			thisRenderer.material = weaponMaterials[(int)type];
		}
	}

}