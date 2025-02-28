using UnityEngine.UI;

public class ReverseVerticalLayoutGroup : VerticalLayoutGroup
{
    private void ReverseChildren()
    {
        for (var i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(0).SetSiblingIndex(transform.childCount - i);
        }
    }

    public override void CalculateLayoutInputHorizontal()
    {
        ReverseChildren();
        base.CalculateLayoutInputHorizontal();
        ReverseChildren();
    }
}