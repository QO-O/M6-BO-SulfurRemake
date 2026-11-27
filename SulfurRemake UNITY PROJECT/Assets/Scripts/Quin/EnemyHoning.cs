using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyHoning : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float gravity = -20f;

    [SerializeField] private float activateRange = 30f;
    [SerializeField] private LayerMask obstacleMask;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }
    }

    
    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        bool inRange = distance <= activateRange;
        bool hasLos = HasLineOfSight();

        bool shouldChase = inRange && hasLos && distance > stopDistance;

        Vector3 desiredVelocity = Vector3.zero;
        if (shouldChase)
        {
            Vector3 dir = toPlayer.normalized;
            desiredVelocity = dir * moveSpeed;

            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        currentSpeed = acceleration <= 0f
            ? desiredVelocity.magnitude
            : Mathf.MoveTowards(currentSpeed, desiredVelocity.magnitude, acceleration * Time.deltaTime);

        Vector3 horizontalMove = toPlayer.sqrMagnitude > 0.001f
            ? toPlayer.normalized * currentSpeed
            : Vector3.zero;

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        controller.Move((horizontalMove + velocity) * Time.deltaTime);
    }

    bool HasLineOfSight()
    {
        if (player == null) return false;
        Vector3 origin = transform.position + Vector3.up * 0.25f;
        Vector3 target = player.position + Vector3.up * 0.25f;
        Vector3 dir = target - origin;
        return !Physics.Raycast(origin, dir.normalized, dir.magnitude, obstacleMask);
    }
}
