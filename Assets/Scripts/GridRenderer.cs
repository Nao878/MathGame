using UnityEngine;
using TMPro;

/// <summary>
/// Grid Renderer component.
/// Draws grid (graph paper), X/Y axes, and coordinate labels.
/// </summary>
public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float gridMin = -5f;
    [SerializeField] private float gridMax = 5f;
    [SerializeField] private float gridStep = 1f;

    [Header("Color Settings")]
    [SerializeField] private Color gridColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color axisColor = new Color(1f, 1f, 1f, 1f);

    [Header("Line Width")]
    [SerializeField] private float gridLineWidth = 0.02f;
    [SerializeField] private float axisLineWidth = 0.05f;

    [Header("Label Settings")]
    [SerializeField] private float labelFontSize = 0.3f;
    [SerializeField] private Color labelColor = Color.white;

    private void Start()
    {
        DrawGrid();
        DrawAxes();
        DrawLabels();
    }

    private void DrawGrid()
    {
        // Draw vertical lines
        for (float x = gridMin; x <= gridMax; x += gridStep)
        {
            CreateLine($"GridV_{x}", new Vector3(x, gridMin, 0), new Vector3(x, gridMax, 0), gridColor, gridLineWidth);
        }

        // Draw horizontal lines
        for (float y = gridMin; y <= gridMax; y += gridStep)
        {
            CreateLine($"GridH_{y}", new Vector3(gridMin, y, 0), new Vector3(gridMax, y, 0), gridColor, gridLineWidth);
        }
    }

    private void DrawAxes()
    {
        // X-axis
        CreateLine("AxisX", new Vector3(gridMin, 0, 0), new Vector3(gridMax, 0, 0), axisColor, axisLineWidth);

        // Y-axis
        CreateLine("AxisY", new Vector3(0, gridMin, 0), new Vector3(0, gridMax, 0), axisColor, axisLineWidth);
    }

    private void DrawLabels()
    {
        // X-axis labels
        for (float x = gridMin; x <= gridMax; x += gridStep)
        {
            if (x != 0) // Skip origin
            {
                CreateLabel($"LabelX_{x}", new Vector3(x, -0.3f, 0), x.ToString());
            }
        }

        // Y-axis labels
        for (float y = gridMin; y <= gridMax; y += gridStep)
        {
            if (y != 0) // Skip origin
            {
                CreateLabel($"LabelY_{y}", new Vector3(-0.3f, y, 0), y.ToString());
            }
        }

        // Origin label
        CreateLabel("LabelOrigin", new Vector3(-0.3f, -0.3f, 0), "0");
    }

    private void CreateLine(string name, Vector3 start, Vector3 end, Color color, float width)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.sortingOrder = -10; // Place in background
        lr.useWorldSpace = true;
    }

    private void CreateLabel(string name, Vector3 position, string text)
    {
        GameObject labelObj = new GameObject(name);
        labelObj.transform.SetParent(transform);
        labelObj.transform.position = position;

        // Use TextMesh for world-space text
        TextMesh textMesh = labelObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 32;
        textMesh.characterSize = labelFontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = labelColor;

        // Add MeshRenderer sorting
        MeshRenderer meshRenderer = labelObj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = -5;
        }
    }
}
