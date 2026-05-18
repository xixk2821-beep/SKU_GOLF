using UnityEngine;

public class ClubHeadVelocity : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public Vector3 PreviousPosition { get; private set; }
    public Vector3 CurrentPosition { get; private set; }

    void Start()
    {
        PreviousPosition = transform.position;
        CurrentPosition = transform.position;
    }

    void LateUpdate()
    {
        PreviousPosition = CurrentPosition;
        CurrentPosition = transform.position;

        Velocity = (CurrentPosition - PreviousPosition) / Time.deltaTime;
    }
}