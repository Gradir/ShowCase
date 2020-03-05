using UnityEngine;

namespace ShowcaseGame
{
	public class DestroyAfterTime : MonoBehaviour
	{
		private void Start()
		{
			Invoke(MethodNamesDatabase.killMeString, 4f);
		}

		private void KillMe()
		{
			Destroy(gameObject);
		}
	}
}