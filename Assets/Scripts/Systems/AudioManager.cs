using UnityEngine;

namespace ShowcaseGame
{
	public enum SFXType
	{
		KeyFound,
		WinGame,
		ShootingFast,
		ShootingBazooka,
		ShootingRailGun,
		ChestOpen,
		Jump,
		Blood,
		Footstep,
		PowerUpSprint,
		PowerUpJump,
		Hit,
	}
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] private AudioClip keySound = null;
		[SerializeField] private AudioClip winSound = null;
		[SerializeField] private AudioSource thisAudioSource = null;
		[SerializeField] private AudioDatabase audioDatabase = null;

		private void Start()
		{
			Signals.Get<KeyFoundSignal>().AddListener(PlayKeyFx);
			Signals.Get<WinScreenShownSignal>().AddListener(PlayWinFx);
		}

		private void OnDestroy()
		{
			Signals.Get<KeyFoundSignal>().RemoveListener(PlayKeyFx);
			Signals.Get<WinScreenShownSignal>().RemoveListener(PlayWinFx);
		}

		public void PlaySFx(Vector3 position, SFXType type)
		{
			AudioSource.PlayClipAtPoint(audioDatabase.GetRandomClipByType(type), position);
		}

		public void PlaySFx(SFXType type)
		{
			thisAudioSource.clip = audioDatabase.GetRandomClipByType(type);
			thisAudioSource.Play();
		}

		private void PlayKeyFx(Vector3 position)
		{
			AudioSource.PlayClipAtPoint(keySound, position);
		}

		private void PlayWinFx()
		{
			thisAudioSource.clip = winSound;
			thisAudioSource.Play();
		}
	}

}