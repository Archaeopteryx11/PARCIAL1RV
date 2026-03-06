using UnityEngine;
using UnityEngine.InputSystem;

public class SphereShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 15f;

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
    }
}