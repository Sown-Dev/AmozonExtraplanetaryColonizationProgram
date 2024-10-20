using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;

        // Check if we should display the property
        bool showProperty = ShouldShowProperty(property, conditional.ConditionFieldName);

        if (showProperty)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;

        // Determine whether to show the property or not
        bool showProperty = ShouldShowProperty(property, conditional.ConditionFieldName);

        if (showProperty)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return 0; // Hide the property by reducing the height to 0
        }
    }

    private bool ShouldShowProperty(SerializedProperty property, string conditionFieldName)
    {
        // Navigate to the parent serialized property using the full property path
        string propertyPath = property.propertyPath; // e.g., "myNestedObject.someField"
        string conditionPath = propertyPath.Replace(property.name, conditionFieldName); // e.g., "myNestedObject.showField"

        // Find the condition property
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            return conditionProperty.boolValue;
        }

        Debug.LogWarning($"ConditionalField: Couldn't find boolean field '{conditionFieldName}' at path '{conditionPath}'");
        return true; // Default to showing the field if the condition is not found
    }
}
