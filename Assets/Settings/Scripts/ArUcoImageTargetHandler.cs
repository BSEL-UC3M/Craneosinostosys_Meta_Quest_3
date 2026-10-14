using UnityEngine;
using System.Globalization;

public class ArUcoImageTargetHandler : MonoBehaviour
{
    [Header("Referencia al ScreenManager")]
    public ScreenManager screenManager;
    public GameObject modelsParent;

    [Header("Marker configuration")]
    public bool isLeftMarker = true;

    [Header("Transformada, mm")]
    public string transformCRS0020 = "0.27,0.28,0.92,-21.55,0.51,-0.85,0.11,157.80,0.82,0.44,-0.38,-37.29,0,0,0,1";

    public string transformCRS0021 = "-0.44,0.88,-0.19,-107.80,0.83,0.31,-0.47,76.41,-0.35,-0.36,-0.86,-18.47,0,0,0,1";

    public string transformCRS0022 = "0.14,-0.31,0.94,27.68,0.56,-0.75,-0.33,128.35,0.81,0.58,0.07,-41.68,0,0,0,1";

    private string _currentTransformMatrixString = "";
    private string _lastPatientName = "";

    [Header("Smoothing")]
    [Range(0.01f, 1f)] public float positionSmoothSpeed = 0.05f;
    [Range(0.01f, 1f)] public float rotationSmoothSpeed = 0.05f;
    public float teleportThreshold = 0.3f;

    private Vector3 _smoothedPosition;
    private Quaternion _smoothedRotation;
    private bool _initialized = false;
    private GameObject _currentParent;

    private ArUcoMarkerBorder markerBorder;

    private Vector3 _rightPosition;
    private Quaternion _rightRotation;

    private readonly Quaternion _baseRotation = Quaternion.Euler(90, 0, 0);

    void Start()
    {
        markerBorder = GetComponent<ArUcoMarkerBorder>();

        if (!isLeftMarker)
        {
            _currentTransformMatrixString = transformCRS0020;
            ComputeRightPose();
        }
    }

    private string GetMatrixForCurrentPatient(GameObject parent)
    {
        if (parent == null)
            return transformCRS0020;

        string patientName = parent.name;

        if (patientName.Contains("CRS0020"))
            return transformCRS0020;

        if (patientName.Contains("CRS0021"))
            return transformCRS0021;

        if (patientName.Contains("CRS0022"))
            return transformCRS0022;

        Debug.LogWarning("No se ha encontrado matriz para el paciente: " + patientName + ". Se usará CRS0020 por defecto.");
        return transformCRS0020;
    }

    private void ComputeRightPose()
    {
        Matrix4x4 m = ParseMatrix(_currentTransformMatrixString);

        // === 1. POSICIÓN ESTILO VUFORIA ===
        Vector3 translation = m.GetColumn(3);

        Vector3 vuforiaStylePosition = new Vector3(
             translation.x / 1000f,
            -translation.y / 1000f,
             translation.z / 1000f
        );

        // === 2. ROTACIÓN ESTILO VUFORIA ===
        Matrix4x4 rotationMatrix = Matrix4x4.identity;
        rotationMatrix.SetColumn(0, m.GetColumn(0));
        rotationMatrix.SetColumn(1, m.GetColumn(1));
        rotationMatrix.SetColumn(2, m.GetColumn(2));
        rotationMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));

        Vector3 forward = new Vector3(
            rotationMatrix.m02,
            rotationMatrix.m12,
            rotationMatrix.m22
        );

        Vector3 upwards = new Vector3(
            rotationMatrix.m01,
            rotationMatrix.m11,
            rotationMatrix.m21
        );

        Quaternion rotation = Quaternion.LookRotation(forward, upwards);

        Quaternion vuforiaStyleRotation = Quaternion.Euler(
            -rotation.eulerAngles.x,
             rotation.eulerAngles.y,
            -rotation.eulerAngles.z
        );

        // Matriz local que funcionaría si el marcador fuera un ImageTarget de Vuforia
        Matrix4x4 vuforiaLocalMatrix = Matrix4x4.TRS(
            vuforiaStylePosition,
            vuforiaStyleRotation,
            Vector3.one
        );

        // Corrección entre el sistema local de Vuforia y el sistema local de ArUco
        Matrix4x4 arucoBaseCorrection = Matrix4x4.TRS(
            Vector3.zero,
            _baseRotation,
            Vector3.one
        );

        // Convertimos la transformación local de Vuforia al sistema local de ArUco
        Matrix4x4 rightModelMatrix = arucoBaseCorrection * vuforiaLocalMatrix;

        Vector4 pos = rightModelMatrix.GetColumn(3);
        _rightPosition = new Vector3(pos.x, pos.y, pos.z);
        _rightRotation = rightModelMatrix.rotation;

        Debug.Log("=== RIGHT MARKER DEBUG - BASE * VUFORIA ===");
        Debug.Log("Original translation mm: " + translation);
        Debug.Log("Vuforia style position m: " + vuforiaStylePosition);
        Debug.Log("Vuforia style rotation: " + vuforiaStyleRotation.eulerAngles);
        Debug.Log("Final right position: " + _rightPosition);
        Debug.Log("Final right rotation: " + _rightRotation.eulerAngles);
    }

    private GameObject GetActiveParent()
    {
        if (screenManager != null)
        {
            GameObject active = screenManager.GetCurrentPatientParent();
            if (active != null) return active;
        }

        return modelsParent;
    }

    public void OnMarkerDetected()
    {
        GameObject parent = GetActiveParent();
        if (parent == null) return;

        if (!isLeftMarker)
        {
            string patientName = parent.name;
            string selectedMatrix = GetMatrixForCurrentPatient(parent);

            if (patientName != _lastPatientName || selectedMatrix != _currentTransformMatrixString)
            {
                _lastPatientName = patientName;
                _currentTransformMatrixString = selectedMatrix;
                ComputeRightPose();
                _initialized = false;
            }
        }

        if (parent != _currentParent)
        {
            _currentParent = parent;
            _initialized = false;
        }

        parent.transform.SetParent(transform, false);

        Vector3 rawPosition;
        Quaternion rawRotation;

        if (isLeftMarker)
        {
            rawPosition = Vector3.zero;
            rawRotation = _baseRotation;
        }
        else
        {
            rawPosition = _rightPosition;
            rawRotation = _rightRotation;
        }

        if (!_initialized)
        {
            _smoothedPosition = rawPosition;
            _smoothedRotation = rawRotation;
            _initialized = true;
        }
        else
        {
            if (Vector3.Distance(rawPosition, _smoothedPosition) > teleportThreshold)
            {
                _smoothedPosition = rawPosition;
                _smoothedRotation = rawRotation;
            }
            else
            {
                _smoothedPosition = Vector3.Lerp(
                    _smoothedPosition,
                    rawPosition,
                    positionSmoothSpeed
                );

                _smoothedRotation = Quaternion.Slerp(
                    _smoothedRotation,
                    rawRotation,
                    rotationSmoothSpeed
                );
            }
        }

        parent.transform.localPosition = _smoothedPosition;
        parent.transform.localRotation = _smoothedRotation;
        parent.transform.localScale = Vector3.one;

        if (markerBorder != null)
            markerBorder.SetDetected();
    }

    public void OnMarkerLost()
    {
        _initialized = false;

        if (markerBorder != null)
            markerBorder.SetLost();
    }

    private Matrix4x4 ParseMatrix(string s)
    {
        string[] values = s.Split(',');
        Matrix4x4 m = Matrix4x4.identity;

        if (values.Length == 16)
        {
            float[] f = new float[16];

            for (int i = 0; i < 16; i++)
                f[i] = float.Parse(values[i], CultureInfo.InvariantCulture);

            // Misma matriz que en el código de tu tutora.
            // El string está en formato por filas.
            m.SetRow(0, new Vector4(f[0], f[1], f[2], f[3]));
            m.SetRow(1, new Vector4(f[4], f[5], f[6], f[7]));
            m.SetRow(2, new Vector4(f[8], f[9], f[10], f[11]));
            m.SetRow(3, new Vector4(f[12], f[13], f[14], f[15]));
        }
        else
        {
            Debug.LogError("La matriz no tiene 16 valores. Revisa transformMatrixString.");
        }

        return m;
    }
}