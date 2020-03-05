using Zenject;

namespace ShowcaseGame
{
	public class GameSceneDIInstaller : MonoInstaller<GameSceneDIInstaller>
	{
		public GameSceneSettings settings;

		public override void InstallBindings()
		{
			Container.BindInstance(settings);

			Container.Bind<Player>().FromComponentInHierarchy().AsSingle().NonLazy();

			Container.Bind<UIManager>().FromComponentInNewPrefab(settings.uiManager).AsSingle().NonLazy();
			Container.Bind<GameDirector>().FromComponentInNewPrefab(settings.gameDirector).AsSingle().NonLazy();
			Container.Bind<AudioManager>().FromComponentInNewPrefab(settings.audioManager).AsSingle().NonLazy();
			Container.Bind<InputManager>().FromComponentInNewPrefab(settings.inputManager).AsSingle().NonLazy();

			// Memory Pools:
			Container.BindMemoryPool<Enemy, Enemy.EnemyPool>().WithInitialSize(32)
				.FromComponentInNewPrefab(settings.enemyPrefab)
				.UnderTransformGroup(MethodNamesDatabase.enemies); ;
			Container.BindMemoryPool<PowerUp, PowerUp.PowerUpPool>().WithInitialSize(2)
				.FromComponentInNewPrefab(settings.powerUpPrefab)
				.UnderTransformGroup(MethodNamesDatabase.powerUps); ;
			Container.BindMemoryPool<Projectile, Projectile.ProjectilePool>().WithInitialSize(64)
				.FromComponentInNewPrefab(settings.projectilePrefab)
				.UnderTransformGroup(MethodNamesDatabase.projectiles); ;
			Container.BindMemoryPool<Explosion, Explosion.ExplosionPool>().WithInitialSize(8)
				.FromComponentInNewPrefab(settings.explosionPrefab)
				.UnderTransformGroup(MethodNamesDatabase.explosions); ;
		}
	}
}
