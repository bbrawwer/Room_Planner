using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;
    public float collisionCheckDistance = 0.5f;
    public GameObject[] collisionObjects; // Массив объектов для коллизии

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isFreeCamera = false; // Теперь камера включается и выключается

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isFreeCamera = !isFreeCamera; // Переключение режима
            Cursor.lockState = isFreeCamera ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isFreeCamera;
        }

        if (!isFreeCamera)
            return; // Если не в режиме камеры, ничего не делаем

        RotateCamera();
        MoveCamera();
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(1)) // Правая кнопка мыши
        {
            yaw += rotationSpeed * Input.GetAxis("Mouse X");
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }

    void MoveCamera()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = transform.TransformDirection(direction).normalized * moveSpeed * Time.deltaTime;

        if (!IsColliding(move))
        {
            transform.position += move;
        }

        if (Input.GetKey(KeyCode.E))
        {
            Vector3 upMove = Vector3.up * moveSpeed * Time.deltaTime;
            if (!IsColliding(upMove))
            {
                transform.position += upMove;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            Vector3 downMove = Vector3.down * moveSpeed * Time.deltaTime;
            if (!IsColliding(downMove))
            {
                transform.position += downMove;
            }
        }
    }

    bool IsColliding(Vector3 moveDirection)
    {
        Ray ray = new Ray(transform.position, moveDirection.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, collisionCheckDistance))
        {
            foreach (var obj in collisionObjects)
            {
                if (hit.collider.gameObject == obj)
                {
                    return true; // Столкновение найдено
                }
            }
        }

        return false; // Столкновения нет
    }
}
