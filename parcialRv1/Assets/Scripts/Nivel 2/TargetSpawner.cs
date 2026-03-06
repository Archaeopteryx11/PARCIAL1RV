using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Sphere Settings")]
    [Tooltip("Tag del objeto que representa el centro de la esfera. Debe tener el tag 'SphereCenter' en la escena.")]
    public string sphereCenterTag = "SphereCenter";
    public float sphereRadius = 10f;

    [Header("Prefabs")]
    public GameObject positivePrefab;
    public GameObject negativePrefab;
    public GameObject rarePrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;

    private Transform sphereCenter;

    void Start()
    {
        // Busca el centro automáticamente por tag, sin necesitar referencia manual
        GameObject centerObj = GameObject.FindWithTag(sphereCenterTag);
        if (centerObj != null)
        {
            sphereCenter = centerObj.transform;
        }
        else
        {
            // Fallback: usa la posición de este mismo objeto como centro
            Debug.LogWarning($"[TargetSpawner] No se encontró un objeto con el tag '{sphereCenterTag}'. Usando posición propia como centro.");
            sphereCenter = this.transform;
        }

        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);
    }

    void Spawn()
    {
        Vector3 pos = Random.onUnitSphere * sphereRadius + sphereCenter.position;

        int roll = Random.Range(0, 100);

        GameObject prefab;

        if (roll < 60)
            prefab = positivePrefab;
        else if (roll < 90)
            prefab = negativePrefab;
        else
            prefab = rarePrefab;

        GameObject spawned = Instantiate(prefab, pos, Quaternion.identity);

        // Pasa la referencia del centro a los componentes que la necesiten
        SurfaceMover mover = spawned.GetComponent<SurfaceMover>();
        if (mover != null)
            mover.SetCenter(sphereCenter);

        TargetObject target = spawned.GetComponent<TargetObject>();
        if (target != null)
            target.SetCenter(sphereCenter);
    }
}