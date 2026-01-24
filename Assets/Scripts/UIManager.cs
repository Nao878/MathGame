using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Manager component.
/// Handles expression input and graph generation button.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField expressionInput;
    [SerializeField] private Button fireButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Game References")]
    [SerializeField] private GraphGenerator graphGenerator;

    private void Start()
    {
        // Add button listener
        if (fireButton != null)
        {
            fireButton.onClick.AddListener(OnFireButtonClicked);
        }

        // Initial text
        if (statusText != null)
        {
            statusText.text = "Enter expression and FIRE!";
        }

        // Allow Enter key to fire
        if (expressionInput != null)
        {
            expressionInput.onSubmit.AddListener(OnInputSubmit);
        }
    }

    private void OnFireButtonClicked()
    {
        FireGraph();
    }

    private void OnInputSubmit(string text)
    {
        FireGraph();
    }

    private void FireGraph()
    {
        if (expressionInput == null || graphGenerator == null)
        {
            Debug.LogError("UIManager: Required references not set");
            return;
        }

        string expression = expressionInput.text.Trim();

        if (string.IsNullOrEmpty(expression))
        {
            UpdateStatus("Please enter an expression");
            return;
        }

        bool success = graphGenerator.GenerateGraph(expression);

        if (success)
        {
            UpdateStatus($"Graph created: {expression}");
            // Clear input
            expressionInput.text = "";
            // Return focus to input field
            expressionInput.Select();
            expressionInput.ActivateInputField();
        }
        else
        {
            UpdateStatus($"Error: Invalid expression");
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
