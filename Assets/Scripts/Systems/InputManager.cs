using UnityEngine;
using Zenject;

namespace ShowcaseGame
{
	public class InputManager : MonoBehaviour
	{
		private GameDirector gameDirector;
		private Player player;
		private bool inputBlocked = false;
		private bool gameStarted = false;
		private bool gamePaused = false;
		private bool winScreenShown = false;

		[Inject]
		private void Init(GameDirector gameDirector, Player player)
		{
			this.gameDirector = gameDirector;
			this.player = player;
		}

		private void Start()
		{
			inputBlocked = true;
			Signals.Get<GameStartedSignal>().AddListener(ReactOnGameStarted);
			Signals.Get<WinScreenShownSignal>().AddListener(ReactOnWinScreen);
			Signals.Get<GamePausedSignal>().AddListener(ReactOnGamePaused);
			Signals.Get<GameUnPausedSignal>().AddListener(ReactOnGameUnPaused);
		}

		private void OnDestroy()
		{
			Signals.Get<GameStartedSignal>().RemoveListener(ReactOnGameStarted);
			Signals.Get<WinScreenShownSignal>().RemoveListener(ReactOnWinScreen);
			Signals.Get<GamePausedSignal>().RemoveListener(ReactOnGamePaused);
			Signals.Get<GameUnPausedSignal>().RemoveListener(ReactOnGameUnPaused);
		}

		private void ReactOnGamePaused()
		{
			Cursor.lockState = CursorLockMode.Confined;
			gamePaused = true;
		}

		private void ReactOnGameUnPaused()
		{
			Cursor.lockState = CursorLockMode.Locked;
			gamePaused = false;
		}

		private void ReactOnWinScreen()
		{
			inputBlocked = true;
			winScreenShown = true;
		}

		private void ReactOnGameStarted()
		{
			inputBlocked = false;
			gameStarted = true;
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape) && gameStarted == true)
			{
				if (gamePaused)
				{
					Signals.Get<GameUnPausedSignal>().Dispatch();
				}
				else
				{
					Signals.Get<GamePausedSignal>().Dispatch();
				}
			}
			if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
			{
				if (gameStarted == false)
				{
					Signals.Get<GameStartedSignal>().Dispatch();
				}
				else if (winScreenShown)
				{
					gameDirector.RestartGame();
				}
			}
			if (inputBlocked == true)
			{
				return;
			}
			if (Input.GetKey(KeyCode.Alpha1))
			{
				Signals.Get<SwitchedWeaponSignal>().Dispatch(WeaponType.FastGun);
			}
			if (Input.GetKey(KeyCode.Alpha2))
			{
				Signals.Get<SwitchedWeaponSignal>().Dispatch(WeaponType.RailGun);
			}
			if (Input.GetKey(KeyCode.Alpha3))
			{
				Signals.Get<SwitchedWeaponSignal>().Dispatch(WeaponType.Bazooka);
			}
			if (Input.GetMouseButton(0))
			{
				player.Shooting();
			}
		}
	}
}