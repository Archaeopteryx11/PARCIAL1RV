using UnityEngine;

public enum TargetType
{
    Positive,
    Negative,
    Rare
}

public enum RotationMode
{
    RandomSpin,
    SurfaceAligned
}

public class TargetObject : MonoBehaviour
{
    public TargetType type;
    public RotationMode rotationMode;

    public int points = 10;
    public float spinSpeed = 60f;

    private Transform sphereCenter;

    // Llamado por TargetSpawner justo después de instanciar
    public void SetCenter(Transform center)
    {
        sphereCenter = center;
    }

    void Start()
    {
        // Fallback: si nadie llamó SetCenter, busca por tag
        if (sphereCenter == null)
        {
            GameObject centerObj = GameObject.FindWithTag("SphereCenter");
            if (centerObj != null)
                sphereCenter = centerObj.transform;
            else if (rotationMode == RotationMode.SurfaceAligned)
                Debug.LogWarning("[TargetObject] No se encontró el centro de la esfera para SurfaceAligned.");
        }
    }

    void Update()
    {
        if (rotationMode == RotationMode.RandomSpin)
        {
            transform.Rotate(Random.insideUnitSphere * spinSpeed * Time.deltaTime);
        }
        else if (rotationMode == RotationMode.SurfaceAligned && sphereCenter != null)
        {
            Vector3 dir = (transform.position - sphereCenter.position).normalized;
            transform.up = dir;
        }
    }

    public void Hit()
    {
        if (type == TargetType.Positive)
            GameManager.Instance.AddPoints(points);

        if (type == TargetType.Negative)
            GameManager.Instance.AddPoints(-points);

        if (type == TargetType.Rare)
            GameManager.Instance.AddPoints(points * 3);

        Destroy(gameObject);
    }
}