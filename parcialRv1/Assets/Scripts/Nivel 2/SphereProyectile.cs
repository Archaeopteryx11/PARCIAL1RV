using UnityEngine;

public class SphereProjectile : MonoBehaviour
{
    public float spinSpeed = 500f;

    void Update()
    {
        transform.Rotate(Vector3.right * spinSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TargetObject target = collision.collider.GetComponent<TargetObject>();

        if (target != null)
        {
            target.Hit();
        }

        Destroy(gameObject);
    }
}