using UnityEngine;

public class rotacion : MonoBehaviour
{
    public float[] correctRotations; // Ángulos en los que la pieza es válida (ej: 0, 180)
    [SerializeField] public bool isPlaced = false;
    float currentRotation = 0;
    PuzzleManager puzzleManager;


    void Start()
    {
        puzzleManager = Object.FindFirstObjectByType<PuzzleManager>();

        int rand = Random.Range(0, 4);
        currentRotation = rand * 90;
        transform.eulerAngles = new Vector3(0, 0, currentRotation);

        CheckStatus();
    }

    void Update()
    {
        if (puzzleManager != null && puzzleManager.nivelTerminado)
            return;

        if (Object.FindFirstObjectByType<Reloj>().pausado) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.gameObject == gameObject)
            {
                RotarPieza();
            }
        }
    }

    
        void RotarPieza()
        {
            transform.Rotate(0, 0, 90);
            CheckStatus();
            Object.FindFirstObjectByType<PuzzleManager>().CheckWinCondition();
        }

    public void CheckStatus()
    {
        float z = transform.eulerAngles.z;

        // Normalizar a múltiplos de 90
        z = Mathf.Round(z / 90f) * 90f;
        z = z % 360;

        isPlaced = false;

        foreach (float angle in correctRotations)
        {
            if (Mathf.Approximately(z, angle))
            {
                isPlaced = true;
                break;
            }
        }
    }
}