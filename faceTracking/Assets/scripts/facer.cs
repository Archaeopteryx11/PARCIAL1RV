using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFace))]
public class FaceObjectController : MonoBehaviour
{
    [Header("Objeto 3D a colocar sobre la cara")]
    [SerializeField] private GameObject faceObject;

    private ARFace _arFace;

    void Awake()
    {
        _arFace = GetComponent<ARFace>();
    }

    void OnEnable()
    {
        _arFace.updated += OnFaceUpdated;
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    void OnDisable()
    {
        _arFace.updated -= OnFaceUpdated;
        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // Ocultar el objeto si la sesión no está lista
        SetObjectVisible(ARSession.state == ARSessionState.SessionTracking);
    }

    private void OnFaceUpdated(ARFaceUpdatedEventArgs args)
    {
        // Mostrar solo si la cara está siendo trackeada
        bool isTracked = _arFace.trackingState == TrackingState.Tracking;
        SetObjectVisible(isTracked);
    }

    private void SetObjectVisible(bool visible)
    {
        if (faceObject != null)
            faceObject.SetActive(visible);
    }
}