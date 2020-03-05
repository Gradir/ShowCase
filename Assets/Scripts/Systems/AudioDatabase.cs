using System.Collections.Generic;
using UnityEngine;

namespace ShowcaseGame
{
    [CreateAssetMenu(fileName = "Audio Database", menuName = "Databases/Audio Database")]
	public class AudioDatabase : ScriptableObject
	{
		[SerializeField] private List<AudioClip> playerHitSounds = new List<AudioClip>();
		[SerializeField] private AudioClip[] powerUpJump = null;
		[SerializeField] private AudioClip[] powerUpSprint = null;

		public AudioClip GetRandomClipByType(SFXType type)
		{
			switch (type)
			{
				case SFXType.Hit:
					return playerHitSounds[Random.Range(0, playerHitSounds.Count)];
				case SFXType.PowerUpJump:
					return powerUpJump[Random.Range(0, powerUpJump.Length)];
				case SFXType.PowerUpSprint:
					return powerUpSprint[Random.Range(0, powerUpSprint.Length)];
				default:
					return null;
			}
		}
	}
}
