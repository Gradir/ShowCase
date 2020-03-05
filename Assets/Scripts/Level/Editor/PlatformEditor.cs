using UnityEditor;

namespace ShowcaseGame
{
	[CustomEditor(typeof(Platform))]
	[CanEditMultipleObjects]
	public class PlatformEditor : Editor
	{
		private SerializedProperty bounds = null;
		private SerializedProperty limitEnemies = null;
		private SerializedProperty secondsBetweenSpawns = null;
		private SerializedProperty showBounds = null;
		private SerializedProperty spawnEnemiesGradually = null;

		void OnEnable()
		{
			bounds = serializedObject.FindProperty(MethodNamesDatabase.bounds);
			limitEnemies = serializedObject.FindProperty(MethodNamesDatabase.limitEnemies);
			secondsBetweenSpawns = serializedObject.FindProperty(MethodNamesDatabase.secondsBetweenSpawns);
			showBounds = serializedObject.FindProperty(MethodNamesDatabase.showBounds);
			spawnEnemiesGradually = serializedObject.FindProperty(MethodNamesDatabase.spawnEnemiesGradually);
		}

		public override void OnInspectorGUI()
		{
			Platform script = (Platform)target;
			serializedObject.Update();
			EditorGUILayout.PropertyField(bounds);
			EditorGUILayout.PropertyField(limitEnemies);
			script.spawnEnemiesGradually = EditorGUILayout.Toggle(MethodNamesDatabase.spawnEnemiesGraduallyLabel, script.spawnEnemiesGradually);
			if (script.spawnEnemiesGradually)
			{
				EditorGUILayout.PropertyField(secondsBetweenSpawns);
			}
			EditorGUILayout.PropertyField(showBounds);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
