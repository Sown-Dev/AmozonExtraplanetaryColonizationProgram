using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;

        // Find the boolean field
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditional.ConditionBool);
        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            // Show the field only if the boolean is true
            if (conditionProperty.boolValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            Debug.LogWarning($"ConditionalField: Couldn't find a boolean property with name {conditional.ConditionBool}");
            EditorGUI.PropertyField(position, property, label, true); // Fallback to showing the field
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditional.ConditionBool);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            // Show height only if the condition is met
            return conditionProperty.boolValue ? EditorGUI.GetPropertyHeight(property, label) : 0f;
        }

        return EditorGUI.GetPropertyHeight(property, label); // Fallback if no condition
    }
}