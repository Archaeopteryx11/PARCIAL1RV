//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PuzzleCursor : MonoBehaviour
//{
//    public float cellSize = 1f;
//    public Camera puzzleCamera;
//    public SpriteRenderer cursorVisual;

//    private bool usingGamepad = false;

//    void Update()
//    {
//        // Detectar qué dispositivo se usó último
//        if (Gamepad.current != null && (
//            Gamepad.current.dpad.up.wasPressedThisFrame ||
//            Gamepad.current.dpad.down.wasPressedThisFrame ||
//            Gamepad.current.dpad.left.wasPressedThisFrame ||
//            Gamepad.current.dpad.right.wasPressedThisFrame))
//        {
//            usingGamepad = true;
//        }

//        if (Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0.1f)
//        {
//            usingGamepad = false;
//        }

//        // Solo mover con el dispositivo activo
//        if (usingGamepad)
//            HandleGamepad();
//        else
//            HandleMouse();

//        UpdateCursorColor();
//    }

//    void HandleMouse()
//    {
//        if (Mouse.current == null) return;
//        if (puzzleCamera == null) puzzleCamera = Camera.main;

//        Vector3 mouseScreen = Mouse.current.position.ReadValue();
//        mouseScreen.z = Mathf.Abs(puzzleCamera.transform.position.z);
//        Vector2 mouseWorld = puzzleCamera.ScreenToWorldPoint(mouseScreen);

//        transform.position = SnapToGrid(mouseWorld);

//        if (Mouse.current.leftButton.wasPressedThisFrame)
//            TryRotateAt(transform.position);
//    }

//    void HandleGamepad()
//    {
//        if (Gamepad.current == null) return;

//        Vector2 dir = Vector2.zero;

//        if (Gamepad.current.dpad.up.wasPressedThisFrame) dir = Vector2.up;
//        if (Gamepad.current.dpad.down.wasPressedThisFrame) dir = Vector2.down;
//        if (Gamepad.current.dpad.left.wasPressedThisFrame) dir = Vector2.left;
//        if (Gamepad.current.dpad.right.wasPressedThisFrame) dir = Vector2.right;

//        if (dir != Vector2.zero)
//            transform.position += (Vector3)(dir * cellSize);

//        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
//            TryRotateAt(transform.position);
//    }

//    void TryRotateAt(Vector2 worldPos)
//    {
//        Collider2D hit = Physics2D.OverlapPoint(worldPos);
//        if (hit != null)
//        {
//            rotacion pieza = hit.GetComponent<rotacion>();
//            if (pieza != null) pieza.TryRotate();
//        }
//    }

//    Vector2 SnapToGrid(Vector2 pos)
//    {
//        float x = Mathf.Round((pos.x - 0.5f) / cellSize) * cellSize + 0.5f;
//        float y = Mathf.Round((pos.y - 0.5f) / cellSize) * cellSize + 0.5f;
//        return new Vector2(x, y);
//    }

//    void UpdateCursorColor()
//    {
//        if (cursorVisual == null) return;
//        Collider2D hit = Physics2D.OverlapPoint(transform.position);
//        bool hayPieza = hit != null && hit.GetComponent<rotacion>() != null;
//        cursorVisual.color = hayPieza ? new Color(0f, 1f, 0f, 0.6f) : new Color(1f, 1f, 0f, 0.4f);
//    }
//}