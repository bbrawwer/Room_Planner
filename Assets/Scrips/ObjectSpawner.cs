using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    public FurnitureMenu furnitureMenu;
    public Transform plane;
    private Camera mainCamera;
    private GameObject selectedObject;
    private bool isObjectSelected = false;
    private Vector3 offset;

    public static List<GameObject> createdObjects = new(); // глобальный стек всех объектов

    private GameObject currentFloor;

    [Header("Настройки спавна")]
    public float minDistance = 2.0f;
    public float gridSize = 1.0f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EditModeManager.Instance.IsEditing && !IsPointerOverUI())
        {
            SpawnObjectAtMousePosition();
        }

        if (Input.GetMouseButtonDown(1) && !EditModeManager.Instance.IsEditing && !IsPointerOverUI())
        {
            SelectObject();
        }

        if (EditModeManager.Instance.IsEditing && Input.GetMouseButtonDown(0))
        {
            SelectObjectInEditMode();
        }

        if (isObjectSelected && Input.GetKey(KeyCode.LeftShift) && EditModeManager.Instance.IsEditing)
        {
            if (Input.GetMouseButton(1))
            {
                MoveObjectWithMouse();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && selectedObject != null && EditModeManager.Instance.IsEditing)
        {
            selectedObject.transform.Rotate(0f, 90f, 0f, Space.Self);
            MessageDisplay.Instance.ShowMessage("Объект повёрнут");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleEditingMode();
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            UndoLastCreatedObject();
        }
    }

    private void ToggleEditingMode()
    {
        EditModeManager.Instance.ToggleEditing();

        if (EditModeManager.Instance.IsEditing)
            MessageDisplay.Instance.ShowMessage("Режим редактирования: ВКЛ");
        else
            MessageDisplay.Instance.ShowMessage("Режим редактирования: ВЫКЛ");

        if (!EditModeManager.Instance.IsEditing)
        {
            isObjectSelected = false;
            selectedObject = null;
        }
    }

    private void SpawnObjectAtMousePosition()
    {
        if (furnitureMenu.selectedPrefab == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == plane.gameObject || hit.collider.CompareTag("Floor"))
            {
                Vector3 spawnPosition = hit.point;

                if (furnitureMenu.selectedPrefab.name.Contains("Пол"))
                {
                    SpawnFloor(spawnPosition);
                    MessageDisplay.Instance.ShowMessage("Пол добавлен");
                    return;
                }

                if (currentFloor != null)
                {
                    spawnPosition.y = currentFloor.transform.position.y;
                }

                spawnPosition = SnapToGrid(spawnPosition);

                if (!IsPositionValid(spawnPosition, furnitureMenu.selectedPrefab))
                {
                    MessageDisplay.Instance.ShowMessage("Слишком близко к объекту того же типа");
                    return;
                }

                GameObject newObject = Instantiate(furnitureMenu.selectedPrefab, spawnPosition, Quaternion.identity);
                newObject.name = furnitureMenu.selectedPrefab.name;

                createdObjects.Add(newObject);

                var sel = newObject.GetComponent<SelectableObject>();
                if (sel != null)
                {
                    sel.prefabName = furnitureMenu.selectedPrefab.name;
                    newObject.tag = "Spawnable";
                }

                MessageDisplay.Instance.ShowMessage("Объект добавлен");
            }
        }
    }

    private void SpawnFloor(Vector3 spawnPosition)
    {
        if (currentFloor == null)
        {
            currentFloor = Instantiate(furnitureMenu.selectedPrefab, spawnPosition, Quaternion.identity);
            currentFloor.layer = LayerMask.NameToLayer("Floor");
            currentFloor.tag = "Floor";

            createdObjects.Add(currentFloor);
        }
    }

    private void SelectObjectInEditMode()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (var obj in createdObjects)
            {
                if (hit.collider.gameObject == obj)
                {
                    selectedObject = obj;
                    isObjectSelected = true;
                    offset = selectedObject.transform.position - hit.point;
                    MessageDisplay.Instance.ShowMessage($"Выбран: {selectedObject.name}");
                    break;
                }
            }
        }
    }

    private void SelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (var obj in createdObjects)
            {
                if (hit.collider.gameObject == obj)
                {
                    selectedObject = obj;
                    isObjectSelected = true;
                    offset = selectedObject.transform.position - hit.point;
                    MessageDisplay.Instance.ShowMessage($"Выбран: {selectedObject.name}");
                    break;
                }
            }
        }
    }

    private void MoveObjectWithMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == plane.gameObject || hit.collider.CompareTag("Floor"))
            {
                Vector3 newPosition = hit.point + offset;
                newPosition.y = selectedObject.transform.position.y;
                selectedObject.transform.position = newPosition;

                var sel = selectedObject.GetComponent<SelectableObject>();
                if (sel != null)
                {
                    sel.UpdateOutlineColor();
                }
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        position.x = Mathf.Round(position.x / gridSize) * gridSize;
        position.z = Mathf.Round(position.z / gridSize) * gridSize;
        return position;
    }

    private bool IsPositionValid(Vector3 position, GameObject prefab)
    {
        foreach (var obj in createdObjects)
        {
            if (obj == null) continue;

            if (obj.name.Contains(prefab.name))
            {
                if (Vector3.Distance(position, obj.transform.position) < minDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UndoLastCreatedObject()
    {
        if (createdObjects.Count > 0)
        {
            GameObject last = createdObjects[^1];
            createdObjects.RemoveAt(createdObjects.Count - 1);
            if (last != null) Destroy(last);
            MessageDisplay.Instance.ShowMessage("Отменено: последний объект удалён");
        }
        else
        {
            MessageDisplay.Instance.ShowMessage("Нет объектов для удаления");
        }
    }
}
