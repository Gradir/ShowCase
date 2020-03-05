using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class TriedToOpenChestSignal : ASignal { }
	public class Chest : MonoBehaviour
	{
		[SerializeField] private Animator thisAnimator = null;
		[SerializeField] private AudioSource thisAudioSource = null;

		private UIManager uIManager;
		private GameDirector gameDirector;

		[Inject]
		private void Init(UIManager uIManager, GameDirector gameDirector)
		{
			this.uIManager = uIManager;
			this.gameDirector = gameDirector;
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				uIManager.ShowNotification(MethodNamesDatabase.eToInteract);
				if (Input.GetKey(KeyCode.E))
				{
					if (gameDirector.CheckKeys())
					{
						thisAnimator.SetTrigger(MethodNamesDatabase.openString);
						thisAudioSource.Play();
						Invoke(MethodNamesDatabase.dispatchOpenSignal, 2f);
					}
					else
					{
						DispatchOpenSignal();
					}
				}
			}
		}

		private void DispatchOpenSignal()
		{
			Signals.Get<TriedToOpenChestSignal>().Dispatch();
		}
	}

}