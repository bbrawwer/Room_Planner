using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToSpawn;  // Префаб мебели, который будет появляться в игровом мире
    private bool isPressed = false;    // Флаг, чтобы отслеживать, когда нажатие длительное
    private GameObject draggedObject; // Копия объекта, которую будем перетаскивать
    private Canvas canvas;  // Канвас, на котором будет отображаться перетаскиваемый объект

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();  // Находим канвас в сцене
    }

    // Срабатывает при начале перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Создаём копию объекта, которую будем перетаскивать
        draggedObject = Instantiate(prefabToSpawn);
        draggedObject.transform.SetParent(canvas.transform);  // Ставим объект в канвас
        draggedObject.transform.position = Input.mousePosition;  // Устанавливаем на место курсора
        CanvasGroup canvasGroup = draggedObject.AddComponent<CanvasGroup>();  // Добавляем компонент CanvasGroup для прозрачности и взаимодействия
        canvasGroup.blocksRaycasts = false;  // Отключаем взаимодействие с этим объектом
    }

    // Срабатывает при каждом движении мыши
    public void OnDrag(PointerEventData eventData)
    {
        // Перемещаем копию объекта за курсором
        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }
    }

    // Срабатывает при окончании перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            // Спавним объект в игровом мире на позиции курсора
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spawnPosition.z = 0;  // Убедимся, что объект появляется на одном уровне с камерой

            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Destroy(draggedObject);  // Удаляем перетаскиваемый объект
        }
    }

    // Когда пользователь нажимает на кнопку, проверяем нажатие
    public void OnPointerDown(PointerEventData eventData)
    {
        // Запускаем таймер для долгого нажатия
        isPressed = true;
        StartCoroutine(LongPressHandler());
    }

    // Когда пользователь отпускает кнопку, отменяем флаг
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private IEnumerator LongPressHandler()
    {
        // Ждем 1 секунду (можно подстроить время)
        yield return new WaitForSeconds(1f);

        // Если нажатие длительное, спавним объект
        if (isPressed && prefabToSpawn != null)
        {
            Debug.Log("Долгое нажатие! Добавляем предмет в игровую сцену.");
            SpawnPrefabAtMousePosition();
        }
    }

    // Спавним объект в игровом мире на позиции курсора
    private void SpawnPrefabAtMousePosition()
{
    // Получаем позицию курсора на экране
    Vector3 mousePosition = Input.mousePosition;

    // Преобразуем экранные координаты в мировые
    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    RaycastHit hit;

    // Используем Raycast для нахождения пересечения с объектами на сцене
    if (Physics.Raycast(ray, out hit))
    {
        // Получаем точку пересечения луча с объектом
        Vector3 spawnPosition = hit.point;

        // Если объект в пространстве имеет различную высоту, можно установить y на нужную высоту (например, 0 или другой уровень)
        spawnPosition.y = 0f; // или установите на другой уровень по высоте, если требуется

        // Убедимся, что позиция не выходит за пределы камеры
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(spawnPosition);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            // Позиция находится за пределами камеры, не спавним объект
            Debug.LogWarning("Объект за пределами камеры!");
            return;
        }

        // Инстанцируем объект на найденной позиции
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}

}
