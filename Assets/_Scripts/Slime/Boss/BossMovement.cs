using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : SlimeMovementBase
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] private Transform AttackPoint;
    enum State
    {
        Idle,
        Boss_Attack_1,
        Boss_Attack_2
    }

    [Header("State Control")]
    [SerializeField] float minStateDuration = 20f;
    private State currentState = State.Idle;
    private float stateEnterTime;

    [Header("Idle")]
    [SerializeField] float idleMoveSpeed = 2f;

    [Header("Dash")]
    [SerializeField] float dashDistance = 3f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashCooldown = 1.5f;
    float lastDashTime;

    [Header("Circle Shot")]
    [SerializeField] string bulletKey = CONSTANT.Slime_Green_Attack;
    [SerializeField] int bulletCount = 12;
    [SerializeField] float shotInterval = 2f;
    float lastShotTime;

    bool isBusy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag(CONSTANT.PLAYER_TAG)?.transform;

        ChangeState(State.Idle);
    }

    void FixedUpdate()
    {
        if (isKnockback || target == null) return;

        switch (currentState)
        {
            case State.Idle:
                IdleUpdate();
                break;

            case State.Boss_Attack_1:
                DashUpdate();
                break;

            case State.Boss_Attack_2:
                CircleShotUpdate();
                break;
        }
    }

    #region STATE UPDATE

    void IdleUpdate()
    {
        MoveToPlayer(idleMoveSpeed);

        if (CanChangeState())
            ChangeState(State.Boss_Attack_1);
    }

    void DashUpdate()
    {
        if (!isBusy && Time.time - lastDashTime > dashCooldown)
            StartCoroutine(DashRoutine());

        if (CanChangeState())
            ChangeState(State.Boss_Attack_2);
    }

    void CircleShotUpdate()
    {
        if (Time.time - lastShotTime > shotInterval)
        {
            SpawnCircleBullets();
            lastShotTime = Time.time;
        }

        if (CanChangeState())
            ChangeState(State.Idle);
    }

    #endregion

    #region ROUTINES

    IEnumerator DashRoutine()
    {
        isBusy = true;
        lastDashTime = Time.time;

        animator.Play(State.Boss_Attack_1.ToString());

        Vector2 start = rb.position;
        Vector2 dir = ((Vector2)target.position - start).normalized;
        Vector2 end = start + dir * dashDistance;

        while (Vector2.Distance(rb.position, end) > 0.05f)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position,
                end,
                dashSpeed * Time.fixedDeltaTime));

            spriteRenderer.flipX = dir.x < 0;
            yield return new WaitForFixedUpdate();
        }

        isBusy = false;
    }

    #endregion

    #region HELPER

    void MoveToPlayer(float speed)
    {
        animator.Play(State.Idle.ToString());

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        spriteRenderer.flipX = dir.x < 0;
    }

    void SpawnCircleBullets()
    {
        float step = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject go = PoolManager.Instance.Spawn(
                bulletKey,
                AttackPoint.position);

            if (go != null)
            {
                var enemyAttack = go.GetComponent<Enemy_Attack>();
                if (enemyAttack != null)
                {
                    enemyAttack.Init(dir); // ✅ FIX: dùng hướng vòng tròn
                }
            }
        }
    }

    bool CanChangeState()
    {
        return Time.time - stateEnterTime >= minStateDuration;
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        stateEnterTime = Time.time;
        isBusy = false;

        animator.Play(newState.ToString());
    }

    #endregion

    #region KNOCKBACK

    public override void Knockback(Vector2 hitDirection)
    {
        if (isKnockback) return;
        StartCoroutine(KnockbackRoutine(hitDirection));
    }

    IEnumerator KnockbackRoutine(Vector2 dir)
    {
        isKnockback = true;

        Vector2 start = rb.position;
        Vector2 end = start + dir.normalized * knockbackDistance;

        float t = 0;
        while (t < knockbackDuration)
        {
            t += Time.fixedDeltaTime;
            rb.MovePosition(Vector2.Lerp(start, end, t / knockbackDuration));
            yield return new WaitForFixedUpdate();
        }

        isKnockback = false;
    }

    #endregion
}
