using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionFieldName { get; private set; }

    public ConditionalFieldAttribute(string conditionBool)
    {
        this.ConditionFieldName = conditionBool;
    }
}