using System;
using UnityEngine;

public class Slime : MonoBehaviour, IDamageable, IDameSource    
{
    [Header("Settings")]
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private int Health;

    [Header("Damage Text Settings")]
    [SerializeField] private Vector3 damageTextOffset = new Vector3(0, 0.5f, 0); // Offset 

    private bool isDead;
    private SlimeAnimation slimeAnim;
    private SlimeMovement slimeMovement;
    [Header("Action")]
    public static Action<Vector2> OnSlimeDie;
    public static Action<int> OnSlimeHitPlayer;
    public bool IsDead = false;

    private void Awake()
    {
        slimeAnim = GetComponent<SlimeAnimation>();
        slimeMovement = GetComponent<SlimeMovement>();
    }

    private void OnEnable()
    {
        Health = enemySO.MaxHealth;
        isDead = false;
    }
    private void OnDisable()
    {
        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.UnregisterEnemy();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        Health -= damage;

        slimeAnim?.PlayDamageFlash();
        GameObject DameText = PoolManager.Instance.Spawn(CONSTANT.Fx_Damage_text, transform.position + damageTextOffset);
        DameText.GetComponent<DameTextAnimation>().SetDamage(damage);

        if (slimeMovement != null)
        {
            Vector2 knockDir = (transform.position - slimeMovement.target.position);
            slimeMovement.Knockback(knockDir);
        }

        if (Health <= 0)
            SlimeDie();
    }

    public int SendDame()
    {
        return this.enemySO.damage;
    }

    private void SlimeDie()
    {
        if (isDead) return;

        isDead = true;
        Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);

        PoolManager.Instance.Spawn(CONSTANT.Fx_Damage_Die, pos);
        OnSlimeDie?.Invoke(transform.position);
        slimeAnim?.PlayDeathFade(() =>
        {
            PoolManager.Instance.Despawn(this.transform.name, gameObject);
        });
    }
}