using UnityEngine;
using UnityEngine.InputSystem;

public class interaction : MonoBehaviour
{
    [SerializeField] private LayerMask interactuable;
    private  PlayerInput input;
    private Transform target;

    private void Awake()
    {
        target = transform;
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.actions["Interact"].performed += Interact;
    }

    private void OnDisable()
    {
        input.actions["Interact"].performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");

        //posicion de la raiz es target.position se le sube alrededor de la cintura del jugador
        //se añade un desplazamientopara evita chocar con el objeto
        //se añade la direccion y una mascara
        Physics.Raycast(target.position +(Vector3.up * 0.3f)+(transform.forward * 0.2f),transform.forward, out var hit, 1.5f, interactuable);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
