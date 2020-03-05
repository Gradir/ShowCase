using DG.Tweening;
using UnityEngine;

namespace ShowcaseGame
{
	public class KeyFoundSignal : ASignal<Vector3> { }
	public class Key : MonoBehaviour
	{
		private void Start()
		{
			transform.DORotate(new Vector3(0, 359, 0), 1.5f, RotateMode.FastBeyond360)
				.SetLoops(int.MaxValue, LoopType.Restart)
				.SetEase(Ease.Linear)
				.SetId(MethodNamesDatabase.tweenId);
		}

		private void OnDestroy()
		{
			DOTween.Kill(MethodNamesDatabase.tweenId);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				Signals.Get<KeyFoundSignal>().Dispatch(transform.position);
				Destroy(gameObject);
			}
		}
	} 
}
