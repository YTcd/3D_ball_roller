using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CollisionChecker : MonoBehaviour
{
    [SerializeField]
    private float collisionIgnoreTime = 0.1f;
    [SerializeField]
    private bool isGrounded = false;

    private bool groundContactThisStep = false;
    private float lastJumpedTime = float.NegativeInfinity;

    public bool IsJumpAvaliable => isGrounded && Time.time >= lastJumpedTime + collisionIgnoreTime;

    void FixedUpdate()
    {
        isGrounded = groundContactThisStep;
        groundContactThisStep = false;
    }

    void OnCollisionStay(Collision collision)
    {
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint cp = collision.GetContact(i);
            if (Vector3.Dot(cp.normal, Vector3.up) > 0.7f)
            {
                groundContactThisStep = true;
                break;
            }
        }
    }

    public void WriteLastJumpedTime()
    {
        lastJumpedTime = Time.time;
    }
}
