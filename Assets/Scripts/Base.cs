using UnityEngine;

/// <summary>
/// Base (defense target) component.
/// A blue square placed at coordinates (0, 0).
/// </summary>
public class Base : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private Color baseColor = Color.blue;
    [SerializeField] private float size = 0.8f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        SetupVisuals();
        SetupCollider();
    }

    private void SetupVisuals()
    {
        // Add or get SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Create white square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        spriteRenderer.color = baseColor;
        spriteRenderer.sortingOrder = 1;

        // Set size
        transform.localScale = new Vector3(size, size, 1);
    }

    private void SetupCollider()
    {
        // Add BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When enemy touches the base
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("DAMAGE! Enemy reached the base!");
            Destroy(other.gameObject);
        }
    }
}
