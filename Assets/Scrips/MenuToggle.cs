using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{
    // Публичные переменные для панели меню и кнопки
    public GameObject menuPanel;
    public Button toggleButton;

    // Настройки анимации
    public float openCloseDuration = 0.5f; // Время на открытие/закрытие
    private bool isMenuOpen = false; // Состояние меню (открыто/закрыто)

    // Для анимации положения кнопки
    public RectTransform closeButtonRectTransform;
    public Vector3 closedPosition = new Vector3(500, 500, 0); // Положение в правом верхнем углу
    private Vector3 initialCloseButtonPosition; // Начальная позиция кнопки

    private RectTransform menuRectTransform;

    // Ссылки на ScrollView и Dropdown
    public ScrollRect scrollView;
    public Dropdown dropdown;

    void Start()
    {
        menuRectTransform = menuPanel.GetComponent<RectTransform>();

        // Сохраняем начальную позицию кнопки
        initialCloseButtonPosition = closeButtonRectTransform.localPosition;

        // Начальное положение кнопки - правый верхний угол
        closeButtonRectTransform.localPosition = closedPosition;

        // Добавляем слушатель для кнопки
        toggleButton.onClick.AddListener(ToggleMenu);

        // Начальная настройка (панель скрыта)
        menuPanel.SetActive(false); // Панель скрыта, но кнопка открывания остаётся видимой
    }

    // Функция для открытия/закрытия меню
    void ToggleMenu()
    {
        if (isMenuOpen)
        {
            StartCoroutine(CloseMenu());
        }
        else
        {
            StartCoroutine(OpenMenu());
        }
    }

    // Плавное открытие меню
    System.Collections.IEnumerator OpenMenu()
    {
        menuPanel.SetActive(true); // Сначала делаем панель активной, чтобы она начала анимироваться

        float timeElapsed = 0f;
        Vector2 initialSize = menuRectTransform.sizeDelta;
        Vector2 targetSize = new Vector2(initialSize.x, 400); // Целевая высота панели (открытие)

        // Сохраняем текущие значения отступов, чтобы они не изменялись
        float initialTop = menuRectTransform.offsetMin.y;
        float initialBottom = menuRectTransform.offsetMax.y;

        // Анимация изменения высоты
        while (timeElapsed < openCloseDuration)
        {
            menuRectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, timeElapsed / openCloseDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        menuRectTransform.sizeDelta = targetSize; // Убедимся, что достигли нужного размера

        // Восстанавливаем отступы
        menuRectTransform.offsetMin = new Vector2(menuRectTransform.offsetMin.x, initialTop);
        menuRectTransform.offsetMax = new Vector2(menuRectTransform.offsetMax.x, initialBottom);

        // Принудительно обновляем layout для ScrollView и Dropdown
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuRectTransform);
        if (scrollView != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.GetComponent<RectTransform>());
        }

        if (dropdown != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dropdown.GetComponent<RectTransform>());
        }

        // Перемещаем кнопку на её начальную позицию
        closeButtonRectTransform.localPosition = initialCloseButtonPosition;

        isMenuOpen = true;
    }

    // Плавное закрытие меню
    System.Collections.IEnumerator CloseMenu()
    {
        float timeElapsed = 0f;
        Vector2 initialSize = menuRectTransform.sizeDelta;
        Vector2 targetSize = new Vector2(initialSize.x, 0); // Целевая высота панели (закрытие)

        // Сохраняем текущие значения отступов, чтобы они не изменялись
        float initialTop = menuRectTransform.offsetMin.y;
        float initialBottom = menuRectTransform.offsetMax.y;

        // Анимация изменения высоты
        while (timeElapsed < openCloseDuration)
        {
            menuRectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, timeElapsed / openCloseDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        menuRectTransform.sizeDelta = targetSize; // Убедимся, что достигли нужного размера
        menuPanel.SetActive(false); // Скрываем панель, когда она полностью закрыта

        // Восстанавливаем отступы
        menuRectTransform.offsetMin = new Vector2(menuRectTransform.offsetMin.x, initialTop);
        menuRectTransform.offsetMax = new Vector2(menuRectTransform.offsetMax.x, initialBottom);

        // Принудительно обновляем layout для ScrollView и Dropdown
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuRectTransform);
        if (scrollView != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.GetComponent<RectTransform>());
        }

        if (dropdown != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dropdown.GetComponent<RectTransform>());
        }

        // Перемещаем кнопку в правый верхний угол
        closeButtonRectTransform.localPosition = closedPosition;

        isMenuOpen = false;
    }
}
