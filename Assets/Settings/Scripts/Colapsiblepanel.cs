using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollapsiblePanel : MonoBehaviour
{
    [Header("Panel Elements")]
    public GameObject panelContent;
    public Button toggleButton;
    public TextMeshProUGUI toggleText;

    private bool isExpanded = false;

    void Start()
    {
        if (panelContent == null) Debug.LogError("[CollapsiblePanel] PanelContent no asignado!");
        if (toggleButton == null) Debug.LogError("[CollapsiblePanel] ToggleButton no asignado!");

        panelContent.SetActive(false);
        UpdateText();
        toggleButton.onClick.AddListener(TogglePanel);
        Debug.Log("[CollapsiblePanel] Inicializado correctamente");
    }

    void TogglePanel()
    {
        isExpanded = !isExpanded;
        Debug.Log("[CollapsiblePanel] Toggle! isExpanded = " + isExpanded);
        panelContent.SetActive(isExpanded);
        UpdateText();
    }

    void UpdateText()
    {
        if (toggleText != null)
            toggleText.text = isExpanded ? "▲ Modelos" : "▼ Modelos";
    }
}