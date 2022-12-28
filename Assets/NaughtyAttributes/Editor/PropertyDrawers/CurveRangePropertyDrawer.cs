using UnityEngine;
using UnityEditor;

namespace NaughtyAttributes.Editor
{
	[CustomPropertyDrawer(typeof(CurveRangeAttribute))]
	public class CurveRangePropertyDrawer : PropertyDrawerBase
	{
		private CurveRangeAttribute _cachedCurveRangeAttribute;
		
		protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
		{
			float propertyHeight = property.propertyType == SerializedPropertyType.AnimationCurve
				? GetPropertyHeight(property)
				: GetPropertyHeight(property) + GetHelpBoxHeight();

			return propertyHeight;
		}

		protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(rect, label, property);

			// Check user error
			if (property.propertyType != SerializedPropertyType.AnimationCurve)
			{
				string message = string.Format("Field {0} is not an AnimationCurve", property.name);
				DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
				return;
			}

			if (_cachedCurveRangeAttribute == null)
				_cachedCurveRangeAttribute = PropertyUtility.GetAttribute<CurveRangeAttribute>(property);
			
			var curveRanges = new Rect(
				_cachedCurveRangeAttribute.Min.x,
				_cachedCurveRangeAttribute.Min.y,
				_cachedCurveRangeAttribute.Max.x - _cachedCurveRangeAttribute.Min.x,
				_cachedCurveRangeAttribute.Max.y - _cachedCurveRangeAttribute.Min.y);

            EditorGUI.CurveField(
                rect,
                property,
                _cachedCurveRangeAttribute.Color == EColor.Clear ? Color.green : _cachedCurveRangeAttribute.Color.GetColor(),
                curveRanges,
                label);

			EditorGUI.EndProperty();
		}
	}
}
