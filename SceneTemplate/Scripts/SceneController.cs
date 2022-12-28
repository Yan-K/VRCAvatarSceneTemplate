// SceneController.cs
// Author: Yan-K, Vistanz

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneController : MonoBehaviour
{
	#if UNITY_EDITOR

	// ----------------- Camera Settings
	[HorizontalLine(color: EColor.Gray)]
	[OnValueChanged("OnCameraScaleChanged")]
	[Range(0.1f, 3.0f)]
	[SerializeField]
	internal float cameraHeight = 1.0f;
	
	[OnValueChanged("OnCameraScaleChanged")]
	[Range(0.1f, 3.0f)]
	[SerializeField]
	internal float cameraDistance = 1.0f;
	
	[OnValueChanged("OnCameraSwitch")]
	[Dropdown("CameraList")]
	[Label("Camera")]
	[SerializeField]
	internal int cameraDropdown;
	
	// ----------------- Lighting Settings
	[HorizontalLine(color: EColor.Gray)]
	[OnValueChanged("OnLightingRotationChanged")]
	[Label("Lighting Horizontal")]
	[Range(-180.0f, 180.0f)]
	[SerializeField]
	internal float directLightHorizontal = 120.0f;
	
	[OnValueChanged("OnLightingRotationChanged")]
	[Label("Lighting Vertical")]
	[Range(-180.0f, 180.0f)]
	[SerializeField]
	internal float directLightVertical = 50.0f;
	
	[OnValueChanged("OnLightingIntensityChanged")]
	[Label("Brightness")]
	[Range(0.0f, 3.0f)]
	[SerializeField]
	internal float directLightIntensity = 1.0f;
	
	[OnValueChanged("OnShadowStrengthChanged")]
	[Label("Shadow")]
	[Range(0.0f, 1.0f)]
	[SerializeField]
	internal float shadowIntensity = 0.8f;
	
	[OnValueChanged("OnLightingColorChanged")]
	[Label("Lighting Color")]
	[SerializeField]
	internal Color directLightColor = Color.white;
	
	[OnValueChanged("OnAmbientColorChanged")]
	[Label("Ambient Color")]
	[SerializeField]
	internal Color ambientColor = new Color(0.19f, 0.19f, 0.19f, 1.0f);
	
	// ----------------- Post Processing Settings
	[HorizontalLine(color: EColor.Gray)]
	[OnValueChanged("OnPostToggle")]
	[Label("Post Processing Toggle")]
	[SerializeField]
	internal bool postToggle;

	[OnValueChanged("OnPostSwitch")]
	[Dropdown("PostList")]
	[Label("Post Processing")]
	[SerializeField]
	internal int postDropdown;
	
	// ----------------- Thumbnail Settings
	[HorizontalLine(color: EColor.Gray)]
	[OnValueChanged("OnThumbnialTextureChanged")]
	[ShowAssetPreview(128, 128)]
	[SerializeField]
	internal Texture thumbnailTexture;
	
	// ----------------- Internal Objects
	[HorizontalLine(color: EColor.Gray)]
	[Label("Internal Settings")]
	[SerializeField]
	internal bool internalSettingsWarning;
	
	[InfoBox("Internal settings, do not modify. \n這是內部設定，請勿更動。", EInfoBoxType.Warning)]
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject cameraParentObject;
	
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject cameraObject;

	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject directLightObject;
	
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject postParentObject;
	
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject thumbnailObject;
	
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject[] postObjects;
	
	[ShowIf("internalSettingsWarning")]
	[SerializeField]
	internal GameObject[] cameraObjects;
	
	// ----------------- Reset
	[Button]
	internal void ResetCameraSettings()
	{
		cameraHeight = 1.0f;
		cameraDistance = 1.0f;
		cameraDropdown = 0;
		OnCameraScaleChanged();
		OnCameraSwitch();
	}
	
	[Button]
	internal void ResetLightingSettings()
	{
		directLightHorizontal = 120.0f;
		directLightVertical = 50.0f;
		directLightIntensity = 1.0f;
		shadowIntensity = 0.8f;
		directLightColor = Color.white;
		ambientColor = new Color(0.19f, 0.19f, 0.19f, 1.0f);
		OnLightingRotationChanged();
		OnLightingIntensityChanged();
		OnLightingColorChanged();
		OnShadowStrengthChanged();
		OnAmbientColorChanged();
	}
	
	// ----------------- Generic Function
	internal void ToggleObjects(GameObject[] targets, int index)
	{
		for (int i = 0; i < targets.Length; i++)
			targets[i].SetActive(i == index);
	}
	
	internal void OnCameraSwitch()
	{
		Undo.RecordObjects(cameraObjects, "Camera Switch");
		ToggleObjects(cameraObjects, cameraDropdown);
	}
	
	internal void OnPostSwitch()
	{
		Undo.RecordObjects(postObjects, "Post Processing Switch");
		ToggleObjects(postObjects, postDropdown);
	}
	
	internal void OnPostToggle()
	{
		Undo.RecordObject(postParentObject, "Post Processing Toggle");
		postParentObject.SetActive(postToggle);
	}
	
	internal void OnCameraScaleChanged()
	{
		var newScale = new Vector3 (1.0f, 1.0f, 1.0f);
		newScale.x = cameraDistance;
		newScale.z = cameraDistance;
		newScale.y = cameraHeight;
		
		Undo.RecordObject(cameraParentObject.transform, "Camera Position");
		cameraParentObject.transform.localScale = newScale;
	}
	
	internal void OnLightingRotationChanged()
	{
		var rotation = new Vector3 (0.0f, 0.0f, 0.0f);
		rotation.x = directLightVertical;
		rotation.y = directLightHorizontal;
		rotation.z = 0.0f;
		
		Undo.RecordObject(directLightObject.transform, "Lighting Rotation");
		directLightObject.transform.localEulerAngles = rotation;
	}
	
	internal void OnLightingIntensityChanged()
	{
		Light target = directLightObject.GetComponent<Light>();
		
		Undo.RecordObject(target, "Lighting Settings");
		target.intensity = directLightIntensity;
	}
	
	internal void OnLightingColorChanged()
	{
		Light target = directLightObject.GetComponent<Light>();
		
		Undo.RecordObject(target, "Lighting Settings");
		target.color = directLightColor;
	}
	
	internal void OnShadowStrengthChanged()
	{
		Light target = directLightObject.GetComponent<Light>();
		
		Undo.RecordObject(target, "Shadow Settings");
		target.shadowStrength = shadowIntensity;
	}
	
	internal void OnAmbientColorChanged()
	{
		Camera targetBackground = cameraObject.GetComponent<Camera>();
		
		Undo.RecordObject(targetBackground, "Ambient Settings");
		targetBackground.backgroundColor = ambientColor;
		RenderSettings.ambientLight = ambientColor;
	}
	
	internal void OnThumbnialTextureChanged()
	{
		thumbnailObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", thumbnailTexture);
	}
	
	// ----------------- Fields
	internal DropdownList<int> CameraList()
	{
		return new DropdownList<int>()
		{
			{"Face 01", 0}, {"Face 02", 1}, {"Face 03", 2},
			{"Body 01", 3}, {"Body 02", 4}, {"Body 03", 5}, {"Body 04", 6},
			{"Back 01", 7}, {"Back 02", 8}, {"Back 03", 9}
		};
	}
	
	internal DropdownList<int> PostList()
	{
		return new DropdownList<int>()
		{
			{"Post 01", 0}, {"Post 02", 1}, {"Post 03", 2},
		};
	}

	#endif
}