using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionBool { get; private set; }

    public ConditionalFieldAttribute(string conditionBool)
    {
        this.ConditionBool = conditionBool;
    }
}