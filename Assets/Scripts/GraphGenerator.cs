using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Graph Generator component.
/// Creates graphs (LineRenderer + EdgeCollider2D) from mathematical expressions.
/// </summary>
public class GraphGenerator : MonoBehaviour
{
    [Header("Graph Settings")]
    [SerializeField] private float xMin = -5f;
    [SerializeField] private float xMax = 5f;
    [SerializeField] private float xStep = 0.1f;

    [Header("Display Settings")]
    [SerializeField] private Color graphColor = Color.red;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float fadeTime = 0.5f;

    /// <summary>
    /// Generate a graph from an expression string.
    /// </summary>
    /// <param name="expression">Expression string (e.g., "x", "2*x + 1", "x^2")</param>
    /// <returns>Whether generation was successful</returns>
    public bool GenerateGraph(string expression)
    {
        List<Vector3> points = new List<Vector3>();

        // Evaluate expression for each x value
        for (float x = xMin; x <= xMax; x += xStep)
        {
            double? yResult = ExpressionParser.Evaluate(expression, x);

            if (yResult == null)
            {
                Debug.LogError($"Graph generation failed: Expression '{expression}' evaluation failed");
                return false;
            }

            float y = (float)yResult.Value;

            // Only add points within valid range (exclude infinity and out of range)
            if (!float.IsNaN(y) && !float.IsInfinity(y) && y >= -100 && y <= 100)
            {
                points.Add(new Vector3(x, y, 0));
            }
        }

        if (points.Count < 2)
        {
            Debug.LogError($"Graph generation failed: Not enough valid points");
            return false;
        }

        // Create graph object
        GameObject graphObj = new GameObject($"Graph_{expression}");
        graphObj.layer = LayerMask.NameToLayer("Graph");

        // Add LineRenderer
        LineRenderer lr = graphObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = graphColor;
        lr.endColor = graphColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.sortingOrder = 5;
        lr.useWorldSpace = true;

        // Add EdgeCollider2D for physics
        EdgeCollider2D edgeCollider = graphObj.AddComponent<EdgeCollider2D>();
        Vector2[] points2D = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            points2D[i] = new Vector2(points[i].x, points[i].y);
        }
        edgeCollider.points = points2D;
        edgeCollider.isTrigger = true;

        // Set tag for graph
        graphObj.tag = "Graph";

        // Start fade out and destroy coroutine
        StartCoroutine(FadeOutAndDestroy(graphObj, lr, duration, fadeTime));

        Debug.Log($"Graph created successfully: {expression}");
        return true;
    }

    private IEnumerator FadeOutAndDestroy(GameObject graphObj, LineRenderer lr, float waitTime, float fadeTime)
    {
        // Wait for duration
        yield return new WaitForSeconds(waitTime - fadeTime);

        // Fade out
        float elapsed = 0f;
        Color startColor = lr.startColor;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lr.startColor = newColor;
            lr.endColor = newColor;
            yield return null;
        }

        // Destroy object
        Destroy(graphObj);
    }
}
