using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ModelControlPanel : MonoBehaviour
{
    [Header("Reference to ScreenManager")]
    public ScreenManager uiManager;

    [Header("Control Bar")]
    public GameObject controlBar;
    public TextMeshProUGUI modelLabel;
    public Toggle visibilityToggle;
    public Slider transparencySlider;

    [Header("Botones de modelo")]
    public Button btn_Skull;
    public Button btn_CuttingLines;
    public Button btn_SupportLeft;
    public Button btn_SupportRight;

    [Header("Sprites de modelo (UI)")]
    public Sprite spriteSkull;
    public Sprite spriteCuttingLines;
    public Sprite spriteSupportLeft;
    public Sprite spriteSupportRight;

    [Header("Materiales OPACOS")]
    public Material skull_OP;
    public Material cuttingLines_OP;
    public Material supports_OP;

    [Header("Materiales TRANSPARENTES")]
    public Material skull_TR;
    public Material cuttingLines_TR;
    public Material supports_TR;

    private string selectedPartType = "";
    private string selectedPartName = "";

    private Dictionary<string, bool> visibilityState = new Dictionary<string, bool>
    {
        { "Skull",        true },
        { "CuttingLines", true },
        { "SupportLeft",  true },
        { "SupportRight", true }
    };

    // ─── Paleta UI ───────────────────────────────────────────────────────────────
    static readonly Color ColorFondo = new Color(0.96f, 0.97f, 0.98f, 1f);
    static readonly Color ColorAccento = new Color(0.13f, 0.70f, 0.67f, 1f);
    static readonly Color ColorAcentoOsc = new Color(0.00f, 0.55f, 0.55f, 1f);
    static readonly Color ColorTextoOscuro = new Color(0.05f, 0.10f, 0.10f, 1f);
    static readonly Color ColorBtnNormal = Color.white;
    static readonly Color ColorBtnActivo = new Color(0.80f, 0.95f, 0.93f, 1f);
    static readonly Color ColorControlBar = new Color(0.93f, 0.95f, 0.95f, 1f);

    void Start()
    {
        if (transparencySlider != null)
        {
            transparencySlider.minValue = 0f;
            transparencySlider.maxValue = 1f;
            transparencySlider.value = 1f;
        }

        ForceSliderColors();
        AplicarFondoControlBar();
        AplicarEstiloBotones();
        TraducirTextosInterfaz();
        AumentarControlesParaQuest();

        if (btn_Skull != null)
            btn_Skull.onClick.AddListener(() => OnModelSelected("Skull", "Cráneo"));

        if (btn_CuttingLines != null)
            btn_CuttingLines.onClick.AddListener(() => OnModelSelected("CuttingLines", "Líneas de corte"));

        if (btn_SupportLeft != null)
            btn_SupportLeft.onClick.AddListener(() => OnModelSelected("SupportLeft", "Guía AR izquierda"));

        if (btn_SupportRight != null)
            btn_SupportRight.onClick.AddListener(() => OnModelSelected("SupportRight", "Guía AR derecha"));

        if (visibilityToggle != null)
            visibilityToggle.onValueChanged.AddListener(OnVisibilityChanged);

        if (transparencySlider != null)
            transparencySlider.onValueChanged.AddListener(OnTransparencyChanged);

        Transform panel = controlBar != null ? controlBar.transform.parent : null;
        if (panel != null)
        {
            Image panelImg = panel.GetComponent<Image>();
            if (panelImg != null)
                panelImg.color = ColorFondo;
        }
    }

    void LateUpdate()
    {
        foreach (var kvp in visibilityState)
        {
            Renderer[] renderers = GetRenderers(kvp.Key);
            if (renderers == null) continue;

            foreach (Renderer r in renderers)
                r.enabled = kvp.Value;
        }
    }

    void OnModelSelected(string partType, string partName)
    {
        selectedPartType = partType;
        selectedPartName = partName;

        if (modelLabel != null)
            modelLabel.text = partName;

        if (visibilityToggle != null)
            visibilityToggle.SetIsOnWithoutNotify(visibilityState[partType]);

        // Leer alpha actual del material para posicionar el slider
        Renderer[] renderers = GetRenderers(partType);
        if (renderers != null && renderers.Length > 0 && transparencySlider != null)
        {
            float a = renderers[0].material.HasProperty("_Color") ? renderers[0].material.color.a : 1f;
            transparencySlider.SetValueWithoutNotify(a);
        }

        ResaltarBoton(partType);
    }

    void ResaltarBoton(string partType)
    {
        SetBtnColor(btn_Skull, partType == "Skull");
        SetBtnColor(btn_CuttingLines, partType == "CuttingLines");
        SetBtnColor(btn_SupportLeft, partType == "SupportLeft");
        SetBtnColor(btn_SupportRight, partType == "SupportRight");
    }

    void SetBtnColor(Button btn, bool activo)
    {
        if (btn == null) return;

        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = activo ? ColorBtnActivo : ColorBtnNormal;
    }

    void OnVisibilityChanged(bool isVisible)
    {
        if (selectedPartType == "") return;

        visibilityState[selectedPartType] = isVisible;
    }

    // ─── Transparencia por intercambio de materiales ─────────────────────────────

    void OnTransparencyChanged(float alpha)
    {
        if (selectedPartType == "") return;

        Renderer[] renderers = GetRenderers(selectedPartType);
        if (renderers == null) return;

        bool opaco = (alpha >= 0.99f);
        Material baseMat = opaco ? GetOpaque(selectedPartType) : GetTransparent(selectedPartType);
        if (baseMat == null) return;

        foreach (Renderer r in renderers)
        {
            // Asignar el material correspondiente
            r.material = baseMat;

            // Si es transparente, ajustar el alpha al valor del slider
            if (!opaco && r.material.HasProperty("_Color"))
            {
                Color c = r.material.color;
                c.a = alpha;
                r.material.color = c;
            }
        }
    }

    Material GetOpaque(string partType)
    {
        switch (partType)
        {
            case "Skull": return skull_OP;
            case "CuttingLines": return cuttingLines_OP;
            case "SupportLeft": return supports_OP;
            case "SupportRight": return supports_OP;
            default: return null;
        }
    }

    Material GetTransparent(string partType)
    {
        switch (partType)
        {
            case "Skull": return skull_TR;
            case "CuttingLines": return cuttingLines_TR;
            case "SupportLeft": return supports_TR;
            case "SupportRight": return supports_TR;
            default: return null;
        }
    }

    // ─── Estilos UI ──────────────────────────────────────────────────────────────

    void AplicarFondoControlBar()
    {
        if (controlBar == null) return;

        Image img = controlBar.GetComponent<Image>();
        if (img == null)
            img = controlBar.AddComponent<Image>();

        img.color = ColorControlBar;

        if (modelLabel != null)
        {
            modelLabel.color = ColorTextoOscuro;
            modelLabel.fontSize = 16f;
            modelLabel.fontStyle = FontStyles.Bold;
        }
    }

    void AplicarEstiloBotones()
    {
        AplicarEstiloBoton(btn_Skull, spriteSkull, "Cráneo");
        AplicarEstiloBoton(btn_CuttingLines, spriteCuttingLines, "Líneas de corte");
        AplicarEstiloBoton(btn_SupportLeft, spriteSupportLeft, "Guía AR izquierda");
        AplicarEstiloBoton(btn_SupportRight, spriteSupportRight, "Guía AR derecha");
    }

    void AplicarEstiloBoton(Button btn, Sprite sprite, string nombre)
    {
        if (btn == null) return;

        Image btnImg = btn.GetComponent<Image>();
        if (btnImg != null)
            btnImg.color = ColorBtnNormal;

        ColorBlock cb = btn.colors;
        cb.normalColor = ColorBtnNormal;
        cb.highlightedColor = new Color(0.88f, 0.97f, 0.96f, 1f);
        cb.pressedColor = new Color(0.75f, 0.93f, 0.91f, 1f);
        cb.selectedColor = ColorBtnActivo;
        btn.colors = cb;

        Transform imgT = btn.transform.Find("ImgModelo");
        if (imgT != null)
        {
            Image img = imgT.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
                img.preserveAspect = true;
                img.color = Color.white;
            }
        }

        Transform labelT = btn.transform.Find("LabelModelo");
        if (labelT != null)
        {
            TextMeshProUGUI label = labelT.GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = nombre;
                label.fontSize = 13f;
                label.fontStyle = FontStyles.Bold;
                label.color = ColorTextoOscuro;
                label.alignment = TextAlignmentOptions.Center;
            }
        }
    }

    void TraducirTextosInterfaz()
    {
        if (controlBar == null) return;

        // Texto asociado al toggle, si existe como hijo del VisibilityToggle
        Transform toggleLabelT = controlBar.transform.Find("VisibilityToggle/Label");
        if (toggleLabelT != null)
        {
            TextMeshProUGUI txt = toggleLabelT.GetComponent<TextMeshProUGUI>();
            if (txt != null)
                txt.text = "Visible";
        }

        // Etiqueta de visibilidad, si existe en la ControlBar
        Transform visLabelT = controlBar.transform.Find("VisLabel");
        if (visLabelT != null)
        {
            TextMeshProUGUI txt = visLabelT.GetComponent<TextMeshProUGUI>();
            if (txt != null)
                txt.text = "Visibilidad";
        }

        // Etiqueta de transparencia, si existe en la ControlBar
        Transform transLabelT = controlBar.transform.Find("TransLabel");
        if (transLabelT != null)
        {
            TextMeshProUGUI txt = transLabelT.GetComponent<TextMeshProUGUI>();
            if (txt != null)
                txt.text = "Transparencia";
        }

        // Texto inicial del modelo seleccionado
        if (modelLabel != null && string.IsNullOrWhiteSpace(modelLabel.text))
            modelLabel.text = "Seleccione un modelo";
    }

    void ForceSliderColors()
    {
        if (transparencySlider == null) return;

        Transform bg = transparencySlider.transform.Find("Background");
        if (bg != null)
        {
            Image img = bg.GetComponent<Image>();
            if (img != null)
                img.color = new Color(0.85f, 0.90f, 0.90f, 1f);
        }

        Transform fillArea = transparencySlider.transform.Find("Fill Area");
        if (fillArea != null)
        {
            Transform fill = fillArea.Find("Fill");
            if (fill != null)
            {
                Image img = fill.GetComponent<Image>();
                if (img != null)
                    img.color = ColorAcentoOsc;
            }
        }

        Transform handleArea = transparencySlider.transform.Find("Handle Slide Area");
        if (handleArea != null)
        {
            Transform handle = handleArea.Find("Handle");
            if (handle != null)
            {
                Image img = handle.GetComponent<Image>();
                if (img != null)
                    img.color = ColorAccento;
            }
        }
    }

    void AumentarControlesParaQuest()
    {
        // IMPORTANTE:
        // El checkbox y su label se han ajustado manualmente en el Inspector.
        // No los modificamos aquí para no sobrescribir esos valores.

        // Aumentar manejador del slider de transparencia
        if (transparencySlider != null)
        {
            RectTransform sliderRect = transparencySlider.GetComponent<RectTransform>();
            if (sliderRect != null)
            {
                // Aumenta solo la altura del área interactiva del slider.
                sliderRect.sizeDelta = new Vector2(sliderRect.sizeDelta.x, 36f);
            }

            Transform handleArea = transparencySlider.transform.Find("Handle Slide Area");
            if (handleArea != null)
            {
                Transform handle = handleArea.Find("Handle");
                if (handle != null)
                {
                    RectTransform handleRect = handle.GetComponent<RectTransform>();
                    if (handleRect != null)
                        handleRect.sizeDelta = new Vector2(36f, 36f);

                    Image handleImg = handle.GetComponent<Image>();
                    if (handleImg != null)
                        handleImg.raycastTarget = true;
                }
            }
        }
    }

    // ─── Part lookup ─────────────────────────────────────────────────────────────

    Renderer[] GetRenderers(string partType)
    {
        GameObject patientParent = (uiManager != null) ? uiManager.GetCurrentPatientParent() : null;
        if (patientParent == null) return null;

        string patientID = patientParent.name.Replace("ModelsParent_", "");
        string partName = GetPartName(patientID, partType);

        Transform part = FindDeep(patientParent.transform, partName);
        if (part == null) return null;

        return part.GetComponentsInChildren<Renderer>(true);
    }

    Transform FindDeep(Transform parent, string name)
    {
        if (parent.name == name) return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindDeep(child, name);
            if (result != null)
                return result;
        }

        return null;
    }

    string GetPartName(string patientID, string partType)
    {
        switch (partType)
        {
            case "Skull": return "Skull_" + patientID;
            case "CuttingLines": return "CuttingLines_" + patientID;
            case "SupportLeft": return "MarkerSupport_Left_" + patientID;
            case "SupportRight": return "MarkerSupport_Right_" + patientID;
            default: return "";
        }
    }
}