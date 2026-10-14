using UnityEngine;

/// <summary>
/// Creates a 3D border frame around the ArUco marker.
/// Attach to Marker_Left_Tracked or Marker_Right_Tracked.
/// The border turns green when detected and red when lost.
/// </summary>
public class ArUcoMarkerBorder : MonoBehaviour
{
    // Marker physical size in meters (40mm)
    private float markerSize = 0.03f;

    // Border thickness relative to marker size
    private float borderThickness = 0.003f;

    // The four border quads (top, bottom, left, right)
    private GameObject[] borderQuads = new GameObject[4];

    private Material borderMaterial;

    private Color colorDetected = Color.green;
    private Color colorLost = Color.red;

    void Start()
    {
        CreateBorder();
        SetColor(colorLost); // Start as red (not detected)
    }

    /// <summary>
    /// Creates 4 thin quads forming a square border around the marker.
    /// </summary>
    void CreateBorder()
    {
        // Create a simple unlit material for the border
        borderMaterial = new Material(Shader.Find("Unlit/Color"));
        borderMaterial.color = colorLost;

        float s = markerSize;
        float t = borderThickness;
        float halfS = s / 2f;

        // Each quad: position, scale
        // Top bar
        borderQuads[0] = CreateQuad("Border_Top",
            new Vector3(0, halfS, 0),
            new Vector3(s + t, t, 0.001f));

        // Bottom bar
        borderQuads[1] = CreateQuad("Border_Bottom",
            new Vector3(0, -halfS, 0),
            new Vector3(s + t, t, 0.001f));

        // Left bar
        borderQuads[2] = CreateQuad("Border_Left",
            new Vector3(-halfS, 0, 0),
            new Vector3(t, s, 0.001f));

        // Right bar
        borderQuads[3] = CreateQuad("Border_Right",
            new Vector3(halfS, 0, 0),
            new Vector3(t, s, 0.001f));
    }

    GameObject CreateQuad(string name, Vector3 localPosition, Vector3 localScale)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        quad.name = name;
        quad.transform.SetParent(this.transform);
        quad.transform.localPosition = localPosition;
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = localScale;

        // Remove collider (not needed)
        Destroy(quad.GetComponent<Collider>());

        // Assign border material
        quad.GetComponent<MeshRenderer>().material = borderMaterial;

        return quad;
    }

    /// <summary>
    /// Call this when the marker is detected — turns border green.
    /// </summary>
    public void SetDetected()
    {
        SetColor(colorDetected);
    }

    /// <summary>
    /// Call this when the marker is lost — turns border red.
    /// </summary>
    public void SetLost()
    {
        SetColor(colorLost);
    }

    void SetColor(Color color)
    {
        if (borderMaterial != null)
            borderMaterial.color = color;
    }
}