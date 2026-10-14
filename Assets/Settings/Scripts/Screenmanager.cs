using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages navigation between the Start Screen (patient selection) 
/// and the AR Screen (model control panel).
/// Los colores ya NO se aplican aquí: cada modelo tiene su propio material.
/// </summary>
public class ScreenManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;   // PantallaInicio
    public GameObject arScreen;      // PantallaAR

    [Header("Patient Buttons (Start Screen)")]
    public Button btn_CRS0020;
    public Button btn_CRS0021;
    public Button btn_CRS0022;

    [Header("Back Button (AR Screen)")]
    public Button btnBack;

    [Header("Patient Model Parents")]
    public GameObject modelsParent_CRS0020;
    public GameObject modelsParent_CRS0021;
    public GameObject modelsParent_CRS0022;

    [Header("AR Screen Title")]
    public TextMeshProUGUI arScreenPatientLabel;

    private GameObject currentPatientParent;

    void Start()
    {
        ShowStartScreen();
        SetAllModelsActive(false);

        btn_CRS0020.onClick.AddListener(() => OnPatientSelected(modelsParent_CRS0020, "CRS0020"));
        btn_CRS0021.onClick.AddListener(() => OnPatientSelected(modelsParent_CRS0021, "CRS0021"));
        btn_CRS0022.onClick.AddListener(() => OnPatientSelected(modelsParent_CRS0022, "CRS0022"));

        btnBack.onClick.AddListener(OnBackPressed);

        btn_CRS0021.interactable = (modelsParent_CRS0021 != null);
        btn_CRS0022.interactable = (modelsParent_CRS0022 != null);
    }

    void OnPatientSelected(GameObject patientParent, string patientID)
    {
        if (patientParent == null) return;

        SetAllModelsActive(false);

        patientParent.SetActive(true);
        currentPatientParent = patientParent;

        if (arScreenPatientLabel != null)
            arScreenPatientLabel.text = "Paciente: " + patientID;

        ShowARScreen();

        Debug.Log("[ScreenManager] Patient selected: " + patientID);
    }

    void OnBackPressed()
    {
        if (currentPatientParent != null)
            currentPatientParent.SetActive(false);

        currentPatientParent = null;
        ShowStartScreen();
    }

    void ShowStartScreen()
    {
        startScreen.SetActive(true);
        arScreen.SetActive(false);
    }

    void ShowARScreen()
    {
        startScreen.SetActive(false);
        arScreen.SetActive(true);
    }

    void SetAllModelsActive(bool active)
    {
        if (modelsParent_CRS0020 != null) modelsParent_CRS0020.SetActive(active);
        if (modelsParent_CRS0021 != null) modelsParent_CRS0021.SetActive(active);
        if (modelsParent_CRS0022 != null) modelsParent_CRS0022.SetActive(active);
    }

    /// <summary>
    /// Returns the currently active patient parent.
    /// </summary>
    public GameObject GetCurrentPatientParent()
    {
        return currentPatientParent;
    }
}