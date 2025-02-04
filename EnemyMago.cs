using UnityEngine;

public class EnemyBehavior2D : MonoBehaviour
{
    [Header("Configuração do Inimigo")]
    public float moveSpeed = 3f; // Velocidade do inimigo
    public float attackRange = 5f; // Distância máxima para atacar o jogador
    public float teleportChance = 0.3f; // Chance de teleporte ao ser atacado (30%)
    public float health = 50f; // Vida do inimigo

    [Header("Configuração de Ataque")]
    public GameObject projectilePrefab; // Prefab do projétil
    public Transform player; // Referência ao jogador
    public Transform shootPoint; // Local onde o projétil será criado
    public float fireRate = 1f; // Tempo entre os tiros
    private float nextFireTime = 0f; // Quando o próximo tiro pode acontecer

    private Rigidbody2D rb; // Referência ao Rigidbody2D

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Verificar se as referências essenciais estão atribuídas
        if (player == null)
        {
            Debug.LogError("A referência ao Jogador não está atribuída no script EnemyBehavior2D.");
        }
        if (shootPoint == null)
        {
            Debug.LogError("A referência ao Shoot Point não está atribuída no script EnemyBehavior2D.");
        }
        if (projectilePrefab == null)
        {
            Debug.LogError("O Prefab do projétil não está atribuído no script EnemyBehavior2D.");
        }
    }

    private void Update()
    {
        MoveTowardsPlayer();
        AttackPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return; // Impede o erro caso a referência ao jogador esteja faltando

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        if (player == null || shootPoint == null || projectilePrefab == null) return; // Verifica se as referências estão válidas

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            // Instancia o projétil
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            // Calcula a direção do projétil
            Vector2 direction = (player.position - shootPoint.position).normalized;

            // Inicializa o projétil com a direção e velocidade
            Projectile2D projectileScript = projectile.GetComponent<Projectile2D>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(direction, 10f); // Direção e velocidade
            }
            else
            {
                Debug.LogError("O script 'Projectile2D' não está anexado ao projétil.");
            }

            nextFireTime = Time.time + fireRate; // Define o próximo tempo de disparo
            Debug.Log("Inimigo atirou!");
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Chance de teleporte ao ser atacado
        if (Random.value <= teleportChance)
        {
            Teleport();
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Teleport()
    {
        // Teleporta o inimigo para uma posição aleatória próxima ao jogador
        Vector2 randomPosition = (Vector2)player.position + Random.insideUnitCircle * 3f;
        transform.position = randomPosition;
    }

    private void Die()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject);
    }
}
