using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcaseGame
{
	public class PointsUIUpdateSignal : ASignal<int> { }
	public class SwitchedWeaponSignal : ASignal<WeaponType> { }
	public class UIManager : MonoBehaviour
	{
		public RectTransform crossHair;
		[SerializeField] private GameObject[] healthIcons = null;
		[SerializeField] private Image[] keyIcons = null;
		[SerializeField] private TextMeshProUGUI pointsCountText = null;
		[SerializeField] private GameObject pauseScreen = null;
		[SerializeField] private GameObject highScores = null;
		[SerializeField] private GameObject startScreen = null;
		[Header("Win Screen")]
		[SerializeField] private TextMeshProUGUI elapsedTimeText = null;
		[SerializeField] private TextMeshProUGUI pointsCountText2 = null;
		[SerializeField] private GameObject winScreen = null;
		[Header("Notifications")]
		[SerializeField] private TextMeshProUGUI notificationText = null;
		[SerializeField] private float timeToHideNotification = 5f;
		[Header("PowerUps")]
		[SerializeField] private GameObject sprintPowerUp = null;
		[SerializeField] private GameObject doubleJumpPowerUp = null;
		[Header("Weapons")]
		[SerializeField] private Image[] weaponIcons = null;
		private float timeForIconTween = 0.25f;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Confined;
			SwitchWeapon(WeaponType.FastGun);
			ResetPowerUpIcons();
			Signals.Get<PointsUIUpdateSignal>().AddListener(UpdatePoints);
			Signals.Get<PowerUpAddedSignal>().AddListener(AddPowerUp);
			Signals.Get<HurtPlayerSignal>().AddListener(HurtPlayer);
			Signals.Get<GameStartedSignal>().AddListener(StartTheGameBySignal);
			Signals.Get<SwitchedWeaponSignal>().AddListener(SwitchWeapon);
			Signals.Get<GamePausedSignal>().AddListener(ShowPauseMenu);
			Signals.Get<GameUnPausedSignal>().AddListener(ContinueBySignal);
		}

		private void OnDestroy()
		{
			Signals.Get<PointsUIUpdateSignal>().RemoveListener(UpdatePoints);
			Signals.Get<PowerUpAddedSignal>().RemoveListener(AddPowerUp);
			Signals.Get<HurtPlayerSignal>().RemoveListener(HurtPlayer);
			Signals.Get<GameStartedSignal>().RemoveListener(StartTheGameBySignal);
			Signals.Get<SwitchedWeaponSignal>().RemoveListener(SwitchWeapon);
			Signals.Get<GamePausedSignal>().RemoveListener(ShowPauseMenu);
			Signals.Get<GameUnPausedSignal>().RemoveListener(ContinueBySignal);
		}

		public void StartTheGame()
		{
			Signals.Get<GameStartedSignal>().Dispatch();
		}

		public void ShowPauseMenu()
		{
			pauseScreen.SetActive(true);
			highScores.SetActive(true);
		}

		public void ReactOnUnpause()
		{
			highScores.SetActive(false);
			pauseScreen.SetActive(false);
		}

		public void ExitGame()
		{
			Application.Quit();
		}

		private void ContinueBySignal()
		{
			Time.timeScale = 1;
			pauseScreen.SetActive(false);
			highScores.SetActive(false);
		}

		public void ContinueClicked()
		{
			Signals.Get<GameUnPausedSignal>().Dispatch();
		}

		public void NewGameClicked()
		{
			//Game director...
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}

		private void StartTheGameBySignal()
		{
			Cursor.lockState = CursorLockMode.Locked;
			startScreen.SetActive(false);
			highScores.SetActive(false);
		}

		public void ChangeGameName(TMP_InputField tMP_InputField)
		{
			StartConfig.GetStartConfig().ChangeGameName(tMP_InputField);
		}

		private void HurtPlayer()
		{
			foreach (var h in healthIcons)
			{
				if (h.activeSelf)
				{
					DOVirtual.DelayedCall(0.25f, () => DeactivateHealthIcon(h));
					h.transform.DOScale(0f, 0.2f).SetEase(Ease.OutCirc).SetUpdate(true);
					break;
				}
			}
		}

		private void DeactivateHealthIcon(GameObject icon)
		{
			icon.SetActive(false);
		}

		private void ResetPowerUpIcons()
		{
			sprintPowerUp.SetActive(false);
			doubleJumpPowerUp.SetActive(false);
		}

		private void AddPowerUp(PowerUPType type)
		{
			switch (type)
			{
				case PowerUPType.DoubleJump:
					doubleJumpPowerUp.SetActive(true);
					break;
				case PowerUPType.Sprint:
					sprintPowerUp.SetActive(true);
					break;
			}
		}

		public void UpdateCollectedKeys(int keys)
		{
			for (int i = 0; i < keys; i++)
			{
				if (i > keyIcons.Length - 1)
				{
					return;
				}
				if (keyIcons[i].enabled == false)
				{
					keyIcons[i].enabled = true;
				}
			}
		}

		public void UpdatePoints(int currentPoints)
		{
			pointsCountText.text = currentPoints.ToString();
		}

		public void ShowNotification(string textToShow)
		{
			notificationText.enabled = true;
			notificationText.text = textToShow;
			Invoke(MethodNamesDatabase.hideNotification, timeToHideNotification);
		}

		private void HideNotification()
		{
			notificationText.enabled = false;
		}

		public void ShowWinScreen(float elapsedTime)
		{
			Signals.Get<WinScreenShownSignal>().Dispatch();
			pointsCountText2.text = pointsCountText.text;
			winScreen.SetActive(true);
			elapsedTimeText.text = elapsedTime.ToString(StringDatabase.doubleHash) + StringDatabase.sChar;
		}

		private void SwitchWeapon(WeaponType type)
		{
			for (int i = 0; i < 3; i++)
			{
				if ((int)type == i)
				{
					weaponIcons[i].transform.DOScale(1, timeForIconTween).SetEase(Ease.OutBounce);
					weaponIcons[i].DOColor(Color.white, timeForIconTween).SetEase(Ease.OutBounce);
				}
				else
				{
					weaponIcons[i].transform.DOScale(0.6f, timeForIconTween);
					weaponIcons[i].DOColor(Color.grey, timeForIconTween);
				}
			}
		}
	}

}