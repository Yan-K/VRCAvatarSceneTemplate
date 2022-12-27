// SceneCameraController.cs
// Author: Yan-K, Vistanz
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneCameraController : MonoBehaviour
{
#if UNITY_EDITOR
	
	// ----------------- Lighting Settings
	[SerializeField]
	internal LightingPreset[] lightingPreset = SceneCameraControllerEditor.LightingPresetReset();
	
	// ----------------- Internal Objects
	[SerializeField]
	internal GameObject cameraParentObject;
	
	[SerializeField]
	internal GameObject directLightObject;
	
	[SerializeField]
	internal GameObject postParentObject;
	
	[SerializeField]
	internal GameObject ThumbnailObject;
	
	[SerializeField]
	internal GameObject[] postObjects;
	
	[SerializeField]
	internal PostProcessProfile[] postProfiles;
	
	[SerializeField]
	internal GameObject[] cameraObjects;
#endif
}

#if UNITY_EDITOR

// ----------------- Global Enums and Fields

internal enum CameraEnum {
	Face01, Face02, Face03,
	Body01, Body02, Body03, Body04,
	Back01, Back02, Back03,
}

[Serializable]
internal struct LightingPreset {
	[Range(-180F, 180F)] public float horizontal;
	[Range(-180F, 180F)] public float vertical;

	public LightingPreset(float lightingHorizontal, float lightingVertical) {
		horizontal = lightingHorizontal;
		vertical = lightingVertical;
	}
}

// ----------------- Custom Editor

[CustomEditor(typeof(SceneCameraController))]
public class SceneCameraControllerEditor : Editor {
	bool openLightingPresets;
	bool openPostProcPresets;
	bool openSettings;
	
	public override void OnInspectorGUI() {
		var target = this.target as SceneCameraController;

		EditorGUILayout.LabelField("Camera Control", EditorStyles.boldLabel);
		DrawCameraControl(target);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Lighting Control", EditorStyles.boldLabel);
		DrawLighting(target, serializedObject, ref openLightingPresets);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Post Processing Settings", EditorStyles.boldLabel);
		DrawPostToggle(target, ref openPostProcPresets);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Thumbnail Settings", EditorStyles.boldLabel);
		DrawThumbnail(target);

		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.Space();
		if (openSettings = EditorGUILayout.Foldout(openSettings, "Internal Settings - 內部設定，非必要請勿更動"))
			DrawDefaultInspector();
	}

	// ----------------- Camera

	static void DrawCameraControl(SceneCameraController target) {
		var cameraParentObject = target.cameraParentObject;
		
		if (cameraParentObject == null) {
			EditorGUILayout.HelpBox("Camera parent object is missing", MessageType.Error);
			return;
		}
		
		var cameraParentTransform = cameraParentObject.transform;
		var scale = cameraParentTransform.localScale;
		
		EditorGUI.BeginChangeCheck();
		
		scale.y = EditorGUILayout.Slider("Camera Height", scale.y, 0.1F, 3F);
		scale.z = EditorGUILayout.Slider("Camera Distance", (scale.x + scale.z) / 2F, 0.1F, 3F);
		scale.x = scale.z;
		
		if (EditorGUI.EndChangeCheck()) cameraParentTransform.localScale = scale;
		
		var cameraObjects = target.cameraObjects;
		var cameraObjectsLength = System.Enum.GetValues(typeof(CameraEnum)).Length;
		
		if (cameraObjects.Length < cameraObjectsLength || cameraObjects[9] == null) {
			EditorGUILayout.HelpBox("Camera object is missing", MessageType.Error);
			return;
		}
		
		DrawEnumToggle(typeof(CameraEnum), "Camera Toggle", target.cameraObjects);
	}

	// ----------------- Lighting

	static void DrawLighting(SceneCameraController target, SerializedObject serializedObject, ref bool showPresets) {
		var directLight = target.directLightObject;
		
		if (directLight == null) {
			EditorGUILayout.HelpBox("Lighting object is missing", MessageType.Error);
			return;
		}
		
		var directLightTransform = directLight.transform;
		var rotation = directLightTransform.eulerAngles;
		
		EditorGUI.BeginChangeCheck();
		
		rotation.y = EditorGUILayout.Slider("Lighting Horizontal", Mathf.Repeat(rotation.y + 180F, 360F) - 180F, -180F, 180F);
		rotation.x = EditorGUILayout.Slider("Lighting Vertical", Mathf.Repeat(rotation.x + 180F, 360F) - 180F, -180F, 180F);
		rotation.z = 0;
		
		if (EditorGUI.EndChangeCheck()) directLightTransform.eulerAngles = rotation;

		if (!(showPresets = EditorGUILayout.Foldout(showPresets, "Presets"))) return;

		var presets = serializedObject.FindProperty("lightingPreset");
		
		EditorGUILayout.BeginHorizontal();
		
		for (int i = 0; i < presets.arraySize; i++) {
			if (i > 0 && i % 3 == 0) {
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
				target.directLightObject.transform.eulerAngles = new Vector3(
					verticalField.floatValue,
					horizontalField.floatValue,
					0
				);
			EditorGUILayout.EndVertical();
			EditorGUIUtility.labelWidth = 0.0f;
		}
		
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Reset Presets")) {
			Undo.RecordObject(target, "Reset Lighting Preset");
			target.lightingPreset = LightingPresetReset();
		}
	}

	// ----------------- Post

	static void DrawPostToggle(SceneCameraController target, ref bool showPresets) {
		var postParentObject = target.postParentObject;
		
		if (postParentObject == null) {
			EditorGUILayout.HelpBox("Post processing object is missing", MessageType.Error);
			return;
		}
		
		var enabled = postParentObject.activeSelf;
		
		EditorGUI.BeginChangeCheck();
		
		enabled = EditorGUILayout.Toggle("Post proceesing", enabled);
		
		if (EditorGUI.EndChangeCheck())
			postParentObject.SetActive(enabled);
		
		if (!(showPresets = EditorGUILayout.Foldout(showPresets, "Presets"))) return;

		EditorGUILayout.BeginHorizontal();
		
		var postObjects = target.postObjects;
		var postVolumes = Array.ConvertAll(postObjects, obj => obj.GetComponent<PostProcessVolume>());
		
		for (int i = 0; i < postVolumes.Length; i++) {
			if (i > 0 && i % 3 == 0) {
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
			EditorGUILayout.BeginVertical();
			var profile = postVolumes[i].profile;
			EditorGUI.BeginChangeCheck();
			profile = EditorGUILayout.ObjectField(profile, typeof(PostProcessProfile), false) as PostProcessProfile;
			if (EditorGUI.EndChangeCheck()) postVolumes[i].profile = profile;
			if (GUILayout.Button("Use Preset")) {
				postParentObject.SetActive(true);
				ToggleObjects(postObjects, i);
			}
			EditorGUILayout.EndVertical();
		}
		
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Reset Presets")) {
			Undo.RecordObjects(postVolumes, "Reset Post Processing Preset");
			for (int i = 0, l = Mathf.Min(postVolumes.Length, target.postProfiles.Length); i < l; i++)
				postVolumes[i].profile = target.postProfiles[i];
		}
	}

	// ----------------- Thumbnail

	static void DrawThumbnail(SceneCameraController target) {
		var thumbnailObject = target.ThumbnailObject;
		if (thumbnailObject == null) {
			EditorGUILayout.HelpBox("Thumbnail object is missing", MessageType.Error);
			return;
		}
		var renderer = thumbnailObject.GetComponent<Renderer>();
		if (renderer == null) {
			EditorGUILayout.HelpBox("Thumbnail renderer is missing", MessageType.Error);
			return;
		}
		var sharedMaterial = renderer.sharedMaterial;
		if (sharedMaterial == null) {
			EditorGUILayout.HelpBox("Thumbnail material is missing", MessageType.Error);
			return;
		}
		var thumbnailTexture = sharedMaterial.GetTexture("_MainTex");
		EditorGUI.BeginChangeCheck();
		thumbnailTexture = EditorGUILayout.ObjectField(thumbnailTexture, typeof(Texture2D), false, GUILayout.Width(140), GUILayout.Height(140)) as Texture2D;
		if (EditorGUI.EndChangeCheck())
			sharedMaterial.SetTexture("_MainTex", thumbnailTexture);
	}

	// ----------------- Generic Function

	static void DrawEnumToggle(Type enumType, string label, GameObject[] targets) {
		Enum enumSelected = default;
		for (int i = 0; i < targets.Length; i++)
			if (targets[i].activeSelf) {
				enumSelected = (Enum)Enum.ToObject(enumType, i);
				break;
			}
		EditorGUI.BeginChangeCheck();
		enumSelected = EditorGUILayout.EnumPopup(label, enumSelected);
		if (EditorGUI.EndChangeCheck())
			ToggleObjects(targets, Convert.ToInt32(enumSelected));
	}

	static void ToggleObjects(GameObject[] targets, int index) {
		for (int i = 0; i < targets.Length; i++)
			targets[i].SetActive(i == index);
	}

	internal static LightingPreset[] LightingPresetReset() => new[] {
		new LightingPreset(120F, 50F),
		new LightingPreset(0F, 15F),
		new LightingPreset(90F, 0F),
	};
}
#endif