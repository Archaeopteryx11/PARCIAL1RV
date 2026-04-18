using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MakeupLayer : MonoBehaviour
{
    [SerializeField] ARFace arFace;
    [SerializeField] Material makeupMaterial;

    MeshRenderer faceRenderer;

    void Awake()
    {
        faceRenderer = arFace.GetComponent<MeshRenderer>();
    }

    public void ApplyMakeup()
    {
        var mats = faceRenderer.materials;
        var newMats = new Material[mats.Length + 1];
        mats.CopyTo(newMats, 0);
        newMats[newMats.Length - 1] = makeupMaterial;
        faceRenderer.materials = newMats;
    }

    public void RemoveMakeup()
    {
        var mats = faceRenderer.materials;
        if (mats.Length <= 1) return;
        var newMats = new Material[mats.Length - 1];
        System.Array.Copy(mats, newMats, newMats.Length);
        faceRenderer.materials = newMats;
    }
}