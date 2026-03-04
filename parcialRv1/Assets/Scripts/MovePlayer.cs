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

    private void Awake()
    {
        inputActions = new NIA();
        character = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        inputActions.PlayerMove.Enable();

        inputActions.PlayerMove.Move.performed += OnMove;
        inputActions.PlayerMove.Move.canceled += OnMove;

        inputActions.PlayerMove.Jump.performed += OnJump;

        inputActions.PlayerMove.Run.performed += OnRun;
        inputActions.PlayerMove.Run.canceled += OnRun;

    }

    private void OnDisable()
    {
        inputActions.PlayerMove.Disable();

        inputActions.PlayerMove.Move.performed -= OnMove;
        inputActions.PlayerMove.Move.canceled -= OnMove;

        inputActions.PlayerMove.Jump.performed -= OnJump;

        inputActions.PlayerMove.Run.performed -= OnRun;
        inputActions.PlayerMove.Run.canceled -= OnRun;
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
            // 1. Lógica de Rotación:
            // Calculamos el ángulo hacia donde queremos mirar
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // Creamos una rotación suave (Lerp) hacia ese ángulo
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 2. Movimiento:
            // Movemos al personaje en la dirección en la que está mirando ahora
            character.Move(moveDirection * targetSpeed * Time.deltaTime);
        }

        //gravedad y salto
        velocity.y += gravity * Time.deltaTime;
        character.Move(velocity * Time.deltaTime);

        UpdateAnimations();

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