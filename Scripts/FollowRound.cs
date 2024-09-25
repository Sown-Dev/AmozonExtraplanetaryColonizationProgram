using UnityEngine;

public class FollowRound : MonoBehaviour
{
    [SerializeField]
    private Transform target; // The target object to follow

    [SerializeField]
    private float roundingInterval = 1.0f; // The interval to which positions are rounded

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Calculate rounded position
            float roundedX = RoundToInterval(target.position.x, roundingInterval);
            float roundedY = RoundToInterval(target.position.y, roundingInterval);

            // Update this object's position
            transform.position = new Vector3(roundedX, roundedY, transform.position.z);
        }
    }

    private float RoundToInterval(float value, float interval)
    {
        return Mathf.Round(value / interval) * interval;
    }
}