using UnityEngine;

public class BallStopDetector : MonoBehaviour
{
    public Transform playerRoot;
    public float stopSpeedThreshold = 0.35f;
    public float stopTime = 0.5f;

    private Rigidbody rb;
    private float timer = 0f;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isMoving) return;

        if (rb.linearVelocity.magnitude < stopSpeedThreshold)
        {
            timer += Time.deltaTime;

            if (timer >= stopTime)
            {
                TeleportPlayer();
                isMoving = false;
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void StartTracking()
    {
        isMoving = true;
        timer = 0f;
    }

    void TeleportPlayer()
    {
        Vector3 targetPos = transform.position - playerRoot.forward * 0.3f;
        targetPos.y = playerRoot.position.y;
        playerRoot.position = targetPos;
    }
}