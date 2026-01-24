using UnityEngine;

/// <summary>
/// Enemy character component.
/// Moves toward origin (0, 0) and is destroyed when touching a graph.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private float size = 0.5f;

    private Vector3 targetPosition = Vector3.zero;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        SetupVisuals();
        SetupCollider();
        SetupRigidbody();
    }

    public void Initialize(Vector3 spawnPosition, float speed)
    {
        transform.position = spawnPosition;
        moveSpeed = speed;
    }

    private void SetupVisuals()
    {
        // Add SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Create circular sprite
        int resolution = 64;
        Texture2D texture = new Texture2D(resolution, resolution);
        float center = resolution / 2f;
        float radius = resolution / 2f - 1;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist < radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
        spriteRenderer.color = enemyColor;
        spriteRenderer.sortingOrder = 10;

        // Set size
        transform.localScale = new Vector3(size, size, 1);
    }

    private void SetupCollider()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;
    }

    private void SetupRigidbody()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        // Move toward origin
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When touching a graph
        if (other.CompareTag("Graph"))
        {
            Debug.Log("Enemy destroyed by graph!");
            Destroy(gameObject);
        }
    }
}
