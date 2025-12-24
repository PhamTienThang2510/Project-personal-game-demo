using UnityEngine;
using System.Collections;

public class SlimeMovement : MonoBehaviour
{
    [Header("Slime Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDistance = 0.3f;
    [SerializeField] private float knockbackDuration = 0.1f;

    public Transform target;
    private Rigidbody2D rb;
    private bool isKnockback;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag(CONSTANT.PLAYER_TAG)?.transform;
    }

    void FixedUpdate()
    {
        if (isKnockback) return;

        SlimeMove();
    }

    private void SlimeMove()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        spriteRenderer.flipX = direction.x < 0;
    }

    // 💥 Gọi khi slime bị đánh
    public void Knockback(Vector2 hitDirection)
    {
        if (isKnockback) return;
        StartCoroutine(KnockbackRoutine(hitDirection));
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        isKnockback = true;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + dir.normalized * knockbackDistance;

        float t = 0f;
        while (t < knockbackDuration)
        {
            t += Time.fixedDeltaTime;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t / knockbackDuration);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        isKnockback = false;
    }
}
