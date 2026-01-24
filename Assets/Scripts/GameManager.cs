using UnityEngine;

/// <summary>
/// Game Manager component.
/// Handles initialization and tag/layer setup.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridRenderer gridRenderer;
    [SerializeField] private GraphGenerator graphGenerator;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Base playerBase;
    [SerializeField] private UIManager uiManager;

    private void Awake()
    {
        // Warning for missing tags
        CheckRequiredTags();
    }

    private void Start()
    {
        Debug.Log("=== 2D Graph Math Tower Defense ===");
        Debug.Log("Enter expressions to intercept enemies!");
        Debug.Log("Examples: x, 2*x+1, x^2, 0.5*x, 3");
    }

    private void CheckRequiredTags()
    {
        // Check required tags (must be set in Unity Editor beforehand)
        Debug.Log("Required tags: Enemy, Graph");
        Debug.Log("Set via Unity Editor > Edit > Project Settings > Tags and Layers");
    }

    /// <summary>
    /// Reset the game
    /// </summary>
    public void ResetGame()
    {
        // Destroy all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }

        // Destroy all graphs
        GameObject[] graphs = GameObject.FindGameObjectsWithTag("Graph");
        foreach (var graph in graphs)
        {
            Destroy(graph);
        }

        Debug.Log("Game has been reset");
    }
}
