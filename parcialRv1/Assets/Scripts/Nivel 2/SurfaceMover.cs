using UnityEngine;

public class SurfaceMover : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Transform sphereCenter;
    private Vector3 direction;

    // Llamado por TargetSpawner justo después de instanciar
    public void SetCenter(Transform center)
    {
        sphereCenter = center;
    }

    void Start()
    {
        direction = Random.onUnitSphere;

        // Fallback: si nadie llamó SetCenter, busca por tag
        if (sphereCenter == null)
        {
            GameObject centerObj = GameObject.FindWithTag("SphereCenter");
            if (centerObj != null)
                sphereCenter = centerObj.transform;
            else
                Debug.LogWarning("[SurfaceMover] No se encontró el centro de la esfera.");
        }
    }

    void Update()
    {
        if (sphereCenter == null) return;

        Vector3 normal = (transform.position - sphereCenter.position).normalized;

        direction = Vector3.ProjectOnPlane(direction, normal);

        transform.position += direction * moveSpeed * Time.deltaTime;

        transform.position = sphereCenter.position +
            (transform.position - sphereCenter.position).normalized *
            Vector3.Distance(transform.position, sphereCenter.position);
    }
}