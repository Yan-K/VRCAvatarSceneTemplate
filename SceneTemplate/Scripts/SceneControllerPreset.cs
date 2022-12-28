// SceneControllerPreset.cs
// Author: Yan-K, Vistanz

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SceneController))]
public class SceneControllerPreset : MonoBehaviour
{
#if UNITY_EDITOR
	
	// ----------------- Internal Objects
	[InfoBox("Internal settings, do not modify. \n這是內部設定，請勿更動。", EInfoBoxType.Warning)]
	[SerializeField]
	internal SceneController sceneController;
	
	[SerializeField]
	internal LightingPreset[] lightingPreset = SceneCameraControllerEditor.LightingPresetReset();
	
	[SerializeField]
	internal PostProcessProfile[] postProfiles;
	
#endif
}

#if UNITY_EDITOR

// ----------------- Global Fields
[Serializable]
internal struct LightingPreset
{
	[Range(-180F, 180F)] public float horizontal;
	[Range(-180F, 180F)] public float vertical;

	public LightingPreset(float lightingHorizontal, float lightingVertical)
	{
		horizontal = lightingHorizontal;
		vertical = lightingVertical;
	}
}

// ----------------- Custom Editor
[CustomEditor(typeof(SceneControllerPreset))]
public class SceneCameraControllerEditor : Editor
{
	bool openSettings;
	
	public override void OnInspectorGUI()
	{
		var target = this.target as SceneControllerPreset;

		DrawUILine(Color.gray, 2, 20);

		EditorGUILayout.LabelField("Lighting Preset", EditorStyles.boldLabel);
		DrawLighting(target, serializedObject);
		
		DrawUILine(Color.gray, 2, 20);
		
		EditorGUILayout.LabelField("Post Processing Preset", EditorStyles.boldLabel);
		DrawPostToggle(target);
		
		DrawUILine(Color.gray, 2, 20);

		serializedObject.ApplyModifiedProperties();
		if (openSettings = EditorGUILayout.Foldout(openSettings, "Internal Settings"))
		{
			DrawDefaultInspector();
		}
	}
	
	// ----------------- Lighting

	static void DrawLighting(SceneControllerPreset target, SerializedObject serializedObject)
	{
		var presets = serializedObject.FindProperty("lightingPreset");
		
		EditorGUILayout.BeginHorizontal();
		
		for (int i = 0; i < presets.arraySize; i++)
		{
			if (i > 0 && i % 3 == 0)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
			EditorGUILayout.BeginVertical();
			var preset = presets.GetArrayElementAtIndex(i);
			var horizontalField = preset.FindPropertyRelative(nameof(LightingPreset.horizontal));
			var verticalField = preset.FindPropertyRelative(nameof(LightingPreset.vertical));
			EditorGUIUtility.labelWidth = 1.0f;
			EditorGUILayout.PropertyField(horizontalField);
			EditorGUILayout.PropertyField(verticalField);
			if (GUILayout.Button("Use Preset"))
			{
				target.sceneController.directLightHorizontal = horizontalField.floatValue;
				target.sceneController.directLightVertical = verticalField.floatValue;
				target.sceneController.OnLightingRotationChanged();
			}
			EditorGUILayout.EndVertical();
			EditorGUIUtility.labelWidth = 0.0f;
		}
		
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Reset Presets"))
		{
			Undo.RecordObject(target, "Reset Lighting Preset");
			target.lightingPreset = LightingPresetReset();
		}
	}

	// ----------------- Post

	static void DrawPostToggle(SceneControllerPreset target)
	{
		EditorGUILayout.BeginHorizontal();
		
		var postObjects = target.sceneController.postObjects;
		var postVolumes = Array.ConvertAll(postObjects, obj => obj.GetComponent<PostProcessVolume>());
		
		for (int i = 0; i < postVolumes.Length; i++) 
		{
			if (i > 0 && i % 3 == 0) 
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
			EditorGUILayout.BeginVertical();
			var profile = postVolumes[i].profile;
			EditorGUI.BeginChangeCheck();
			profile = EditorGUILayout.ObjectField(profile, typeof(PostProcessProfile), false) as PostProcessProfile;
			if (EditorGUI.EndChangeCheck()) postVolumes[i].profile = profile;
			EditorGUILayout.EndVertical();
		}
		
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Reset Presets")) 
		{
			Undo.RecordObjects(postVolumes, "Reset Post Processing Preset");
			for (int i = 0, l = Mathf.Min(postVolumes.Length, target.postProfiles.Length); i < l; i++)
				postVolumes[i].profile = target.postProfiles[i];
		}
	}

	// ----------------- Generic Function
	
	internal static LightingPreset[] LightingPresetReset() => new[]
	{
		new LightingPreset(120F, 50F),
		new LightingPreset(0F, 15F),
		new LightingPreset(90F, 0F),
	};
	
	public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
	{
		Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
		r.height = thickness;
		r.y+=padding/2;
		r.x-=2;
		r.width +=6;
		EditorGUI.DrawRect(r, color);
	}
}
#endif