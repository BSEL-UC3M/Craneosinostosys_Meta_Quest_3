using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartScreenUI : MonoBehaviour
{
    [Header("Fondo")]
    public Image backgroundImage;

    [Header("Título")]
    public TextMeshProUGUI titulo;

    [Header("Separador")]
    public Image separador;

    [Header("Subtítulo")]
    public Image iconoPaciente;
    public Sprite spritePersona;
    public TextMeshProUGUI subtitulo;

    [Header("Botones de paciente")]
    public Button btn_CRS0020;
    public Button btn_CRS0021;
    public Button btn_CRS0022;

    public Sprite imgCRS0020;
    public Sprite imgCRS0021;
    public Sprite imgCRS0022;

    // ─── Paleta médica ────────────────────────────────────────────────────────
    static readonly Color ColorFondo = new Color(0.96f, 0.97f, 0.98f, 1f); // #F5F7FA blanco grisáceo
    static readonly Color ColorAccento = new Color(0.13f, 0.70f, 0.67f, 1f); // #20B2AA LightSeaGreen
    static readonly Color ColorTextoOscuro = new Color(0.10f, 0.17f, 0.17f, 1f); // #1A2C2C
    static readonly Color ColorTextoGris = new Color(0.55f, 0.60f, 0.60f, 1f);
    static readonly Color ColorTarjeta = Color.white;
    static readonly Color ColorBorde = new Color(0.88f, 0.92f, 0.92f, 1f);

    void Start()
    {
        AplicarFondo();
        AplicarTitulo();
        AplicarSeparador();
        AplicarSubtitulo();
        AplicarTarjeta(btn_CRS0020, imgCRS0020, "CRS0020", true);
        AplicarTarjeta(btn_CRS0021, imgCRS0021, "CRS0021", true);
        AplicarTarjeta(btn_CRS0022, imgCRS0022, "CRS0022", true);
    }

    void AplicarFondo()
    {
        if (backgroundImage != null)
            backgroundImage.color = ColorFondo;
    }

    void AplicarTitulo()
    {
        if (titulo == null) return;
        titulo.text = "Planificación para cirugía de craneosinostosis";
        titulo.fontSize = 26;
        titulo.fontStyle = FontStyles.Bold;
        titulo.color = ColorTextoOscuro;
        titulo.alignment = TextAlignmentOptions.Center;
        titulo.enableWordWrapping = true;
    }

    void AplicarSeparador()
    {
        if (separador != null)
            separador.color = ColorAccento;
    }

    void AplicarSubtitulo()
    {
        if (iconoPaciente != null && spritePersona != null)
        {
            iconoPaciente.sprite = spritePersona;
            iconoPaciente.color = ColorAccento;
        }

        if (subtitulo != null)
        {
            subtitulo.text = "Seleccione un paciente:";
            subtitulo.fontSize = 15;
            subtitulo.color = ColorTextoOscuro;
            subtitulo.alignment = TextAlignmentOptions.Left;
        }
    }

    void AplicarTarjeta(Button btn, Sprite imgModelo, string nombre, bool activo)
    {
        if (btn == null) return;

        Image btnImg = btn.GetComponent<Image>();
        if (btnImg != null) btnImg.color = ColorTarjeta;

        ColorBlock cb = btn.colors;
        cb.normalColor = ColorTarjeta;
        cb.highlightedColor = new Color(0.88f, 0.97f, 0.96f, 1f); // aqua muy suave
        cb.pressedColor = new Color(0.75f, 0.93f, 0.91f, 1f);
        cb.selectedColor = ColorTarjeta;
        btn.colors = cb;

        // Imagen
        Transform imgT = btn.transform.Find("ImgModelo");
        if (imgT != null)
        {
            Image img = imgT.GetComponent<Image>();
            if (img != null) { img.sprite = imgModelo; img.preserveAspect = true; img.color = Color.white; }
        }

        // Nombre
        Transform labelT = btn.transform.Find("LabelNombre");
        if (labelT != null)
        {
            TextMeshProUGUI label = labelT.GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = nombre;
                label.fontSize = 18f;
                label.fontStyle = FontStyles.Bold;
                label.color = activo ? ColorTextoOscuro : ColorTextoGris;
            }
        }

        // Flecha
        Transform flechaT = btn.transform.Find("Flecha");
        if (flechaT != null)
        {
            TextMeshProUGUI flecha = flechaT.GetComponent<TextMeshProUGUI>();
            if (flecha != null)
            {
                flecha.text = "›";
                flecha.fontSize = 30f;
                flecha.color = activo ? ColorAccento : ColorTextoGris;
            }
        }
    }
}