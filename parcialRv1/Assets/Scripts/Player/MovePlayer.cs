using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    //Referencia al sistema de acciones
    private NIA inputActions;

    //Guardar el movimiento
    private Vector2 moveInput;

    //velocidad del player
    public float speed = 5f;

    //multiplicar la velocidady correr
    public float sprintMultiplier = 2f;

    //velocidad de rotacion
    public float rotationSpeed = 10f;

    //tru o false de la tecla
    private bool isRun;

    //altura del salto o fuerza
    public float jumpHeight;

    //gravedad variable
    private float gravity = -9.8f;

    //vector 3 velcoidad
    private Vector3 velocity;

    //variable del characterController
    private CharacterController character;

    //variable para conectar con el animador
    private Animator animator;

    //variables para camara
    public Transform playerCamera;
    public float mouseSensitivity = 0.15f;
    public float gamepadSensitivity = 120f;
    public float verticalClamp = 60f;

    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isGamepadLook = false;

    private int playerIndex = 0;

    private void Awake()
    {
        
        character = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        playerCamera = GetComponentInChildren<Camera>()?.transform;

        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            playerIndex = pi.playerIndex;
            pi.neverAutoSwitchControlSchemes = true;
        }

    }


    

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (character.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // Si el botón está presionado
        isRun = context.ReadValueAsButton();
    }

    //para mirar, movimiento de camara
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        isGamepadLook = !(context.control?.device is Mouse);

    }

    private void Update()
    {
        if (character.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //calcular la velocidad dependiendo de si corre o no
        float targetSpeed = isRun ? speed * sprintMultiplier : speed;

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        if (moveDirection.magnitude >= 0.1f)
        {

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

            character.Move(move.normalized * targetSpeed * Time.deltaTime);
        }

        HandleLook();

        UpdateAnimations();

    }


    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }

    

    //interaccion con cosas
    public void OnInteract(InputAction.CallbackContext context)
    {
       if (context.performed) TryInteract();
    }

    //rotación de camara
    private void HandleLook()
    {
        if (playerCamera == null) return;

        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        if (isGamepadLook)
        {
            // El gamepad necesita el factor tiempo para ser constante
            mouseX *= gamepadSensitivity * Time.deltaTime;
            mouseY *= gamepadSensitivity * Time.deltaTime;
        }
        else
        {
            // El mouse suele necesitar una sensibilidad más alta o directa
            // Prueba con 0.1f si ves que es muy rápido, o súbelo si es lento
            mouseX *= mouseSensitivity;
            mouseY *= mouseSensitivity;
        }

        // Horizontal: rota el cuerpo del jugador en Y
        transform.Rotate(Vector3.up * mouseX);

        // Vertical: rota solo la cámara
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInput pi = GetComponent<PlayerInput>();

        if (pi.currentControlScheme == "Teclado"
        || Keyboard.current != null && pi.devices.Any(d => d == Keyboard.current))
        {
            pi.SwitchCurrentControlScheme("Teclado",
                Keyboard.current, Mouse.current);
        }
    }

    //funcion para los objetos del mundo
    private void TryInteract()
    {
        if (playerCamera == null) return;

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f))
            {
               if (hit.collider.TryGetComponent<IInteractable>(out var obj))
               {
                    obj.Interact(this);
                    Debug.Log($"[MovePlayer] Interactuando con: {hit.collider.name}");
                }
            }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // En lugar de una sola velocidad, le pasamos los ejes X e Y del input
        // Multiplicamos por 0.5 si camina y por 1 si corre para que coincida con el árbol
        float multiplier = isRun ? 1f : 0.5f;

        animator.SetFloat("velX", moveInput.x * multiplier);
        animator.SetFloat("velY", moveInput.y * multiplier);

        animator.SetBool("isGrounded", character.isGrounded);
    }
}
public interface IInteractable
{
    void Interact(MovePlayer player);
}
