// SceneController.cs
// Author: Yan-K, Vistanz

using System;
using System.Collections.Generic;
using UnityEngine;
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
	[Range(0.1f, 3f)]
	[SerializeField]
	internal float cameraHeight = 1.0f;
	
	[Range(0.1f, 3f)]
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
	[Range(-180.0f, 180f)]
	[SerializeField]
	internal float directLightHorizontal = 120.0f;
	
	[OnValueChanged("OnLightingRotationChanged")]
	[Label("Lighting Vertical")]
	[Range(-180.0f, 180f)]
	[SerializeField]
	internal float directLightVertical = 50.0f;
	
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
	
	// ----------------- Generic Function
	internal void ToggleObjects(GameObject[] targets, int index)
	{
		for (int i = 0; i < targets.Length; i++)
			targets[i].SetActive(i == index);
	}
	
	internal void OnCameraSwitch()
	{
		ToggleObjects(cameraObjects, cameraDropdown);
	}
	
	internal void OnPostSwitch()
	{
		ToggleObjects(postObjects, postDropdown);
	}
	
	internal void OnPostToggle()
	{
		postParentObject.SetActive(postToggle);
	}
	
	internal void OnLightingRotationChanged()
	{
		var rotation = new Vector3 (0.0f, 0.0f, 0.0f);
		rotation.x = directLightVertical;
		rotation.y = directLightHorizontal;
		rotation.z = 0.0f;
		
		directLightObject.transform.localEulerAngles = rotation;
	}
	
	
	// ----------------- Fields
	internal DropdownList<int> CameraList()
	{
		return new DropdownList<int>()
		{
			{"Face01", 0}, {"Face02", 1}, {"Face03", 2},
			{"Body01", 3}, {"Body02", 4}, {"Body03", 5}, {"Body04", 6},
			{"Back01", 7}, {"Back02", 8}, {"Back03", 9}
		};
	}
	
	internal DropdownList<int> PostList()
	{
		return new DropdownList<int>()
		{
			{"Post01", 0}, {"Post02", 1}, {"Post03", 2},
		};
	}

	#endif
}