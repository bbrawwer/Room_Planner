using UnityEngine;

public class ScaleHandle : MonoBehaviour
{
    public enum HandleType { Wall, Floor }
    public HandleType handleType;

    public Transform target;

    private Vector3 startMousePos;
    private Vector3 startScale;
    private bool dragging = false;
    private bool isXAxis;
    private Renderer rend;

    private float pressStartTime;
    private float pressDelay = 0.3f;
    private bool waitingForDrag = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material = new Material(rend.material);

        if (target != null)
        {
            Vector3 offset = transform.position - target.position;
            isXAxis = Mathf.Abs(offset.x) >= Mathf.Abs(offset.z);
        }
    }

    void Update()
    {
        // Нажали правую кнопку мыши над этим объектом
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                pressStartTime = Time.time;
                waitingForDrag = true;
                dragging = false;
            }
        }

        if (waitingForDrag && Input.GetMouseButton(1))
        {
            if (Time.time - pressStartTime >= pressDelay)
            {
                startMousePos = Input.mousePosition;
                startScale = target.localScale;
                dragging = true;
                waitingForDrag = false;

                Debug.Log("Начали масштабирование (" + (isXAxis ? "X" : "Z") + ")");
            }
        }

        if (dragging && Input.GetMouseButton(1))
        {
            float delta = (Input.mousePosition - startMousePos).x * 0.01f;
            Vector3 newScale = startScale;

            if (isXAxis)
                newScale.x = Mathf.Max(0.1f, startScale.x + delta);
            else
                newScale.z = Mathf.Max(0.1f, startScale.z + delta);

            target.localScale = newScale;
        }

        if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
            waitingForDrag = false;
        }

        // 📌 Автоматическое перемещение хендлера при масштабировании
        if (target != null)
        {
            Vector3 offset = Vector3.zero;
            float padding = 0.1f;

            if (handleType == HandleType.Wall)
            {
                if (isXAxis)
                {
                    // Хендлер по X → снизу
                    offset = -target.up * target.localScale.y / 2f + target.forward * 0.01f;
                }
                else
                {
                    // Хендлер по Z → справа
                    offset = target.right * target.localScale.x / 2f;
                }
            }
            else // Floor
            {
                if (isXAxis)
                    offset = target.right * target.localScale.x / 2f;
                else
                    offset = target.forward * target.localScale.z / 2f;
            }

            transform.position = target.position + offset + Vector3.up * padding;
        }
    }

    void OnMouseEnter()
    {
        if (rend != null)
            rend.material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        if (rend != null)
            rend.material.color = Color.red;
    }
}
