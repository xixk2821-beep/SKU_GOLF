using UnityEngine;

public class ClubHitController : MonoBehaviour
{
    public SwingInputState swingInput;

    [Header("Hit Detection")]
    public float castRadius = 0.025f;
    public LayerMask ballLayer;
    public float minApproachSpeed = 0.12f;
    public float hitCooldown = 0.15f;

    [Header("Swing Power")]
    public float softSwingSpeed = 2.2f;
    public float fullSwingSpeed = 22.0f;
    public float minForce = 0.15f;
    public float maxForce = 22f;
    public float powerCurveExponent = 4.2f;

    [Header("Lift")]
    public float minUpwardPower = 0.08f;
    public float maxUpwardPower = 0.85f;
    public float liftCurveExponent = 2.2f;

    [Header("Direction")]
    public float swingDirectionWeight = 0.85f;
    public float ballDirectionWeight = 0.15f;

    private Vector3 previousPosition;
    private float lastHitTime;

    void Start()
    {
        previousPosition = transform.position;
    }

    void LateUpdate()
    {
        TryHitBySweep();
        previousPosition = transform.position;
    }

    void TryHitBySweep()
    {
        if (Time.time - lastHitTime < hitCooldown) return;
        if (swingInput == null) return;
        if (!swingInput.IsTriggerPressed) return;

        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - previousPosition;
        float distance = movement.magnitude;

        if (distance < 0.001f) return;

        RaycastHit[] hits = Physics.SphereCastAll(
            previousPosition,
            castRadius,
            movement.normalized,
            distance,
            ballLayer,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length == 0) return;

        RaycastHit hit = hits[0];
        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 velocity = movement / Time.deltaTime;
        float swingSpeed = velocity.magnitude;

        Vector3 toBall = (hit.collider.transform.position - previousPosition).normalized;
        float approachSpeed = Vector3.Dot(velocity, toBall);

        if (approachSpeed < minApproachSpeed) return;

        Vector3 swingDir = Vector3.ProjectOnPlane(velocity, Vector3.up).normalized;
        Vector3 ballDir = Vector3.ProjectOnPlane(toBall, Vector3.up).normalized;

        if (swingDir == Vector3.zero)
            swingDir = transform.forward;

        Vector3 finalFlatDir =
            (swingDir * swingDirectionWeight + ballDir * ballDirectionWeight).normalized;

        float speed01 = Mathf.InverseLerp(softSwingSpeed, fullSwingSpeed, swingSpeed);

        float powerCurve = Mathf.Pow(speed01, powerCurveExponent);
        float liftCurve = Mathf.Pow(speed01, liftCurveExponent);

        float force = Mathf.Lerp(minForce, maxForce, powerCurve);
        float upwardPower = Mathf.Lerp(minUpwardPower, maxUpwardPower, liftCurve);

        Vector3 finalDir = (finalFlatDir + Vector3.up * upwardPower).normalized;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(finalDir * force, ForceMode.Impulse);

        BallStopDetector ballDetector = hit.collider.GetComponent<BallStopDetector>();
        if (ballDetector != null)
        {
            ballDetector.StartTracking();
        }

        lastHitTime = Time.time;
    }
}