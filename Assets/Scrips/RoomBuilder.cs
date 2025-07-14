using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomBuilder : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject roomPanel;
    public TMP_InputField widthInput;
    public TMP_InputField lengthInput;
    public TMP_InputField wallHeightInput;
    public Button createButton;
    public Button openRoomButton;

    [Header("Префабы")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    private GameObject currentFloor;
    private List<GameObject> currentWalls = new();

    void Start()
    {
        roomPanel.SetActive(false);
        openRoomButton.onClick.AddListener(ShowRoomPanel);
        createButton.onClick.AddListener(CreateRoom);
    }

    void ShowRoomPanel()
    {
        roomPanel.SetActive(true);
    }

    void CreateRoom()
    {
        if (!float.TryParse(widthInput.text, out float width) ||
            !float.TryParse(lengthInput.text, out float length) ||
            !float.TryParse(wallHeightInput.text, out float wallHeight))
        {
            Debug.LogWarning("Некорректный ввод!");
            return;
        }

        roomPanel.SetActive(false);

        CreateFloor(width, length);
        CreateWalls(width, length, wallHeight);
    }

    void CreateFloor(float width, float length)
    {
        if (currentFloor != null) Destroy(currentFloor);

        float floorY = 1.8f;
        Vector3 position = new Vector3(0, floorY, 0);

        currentFloor = Instantiate(floorPrefab, position, Quaternion.identity);
        currentFloor.transform.localScale = new Vector3(width, 0.2f, length);
        currentFloor.tag = "Floor";

        ObjectSpawner.createdObjects.Add(currentFloor);
    }

    void CreateWalls(float width, float length, float height)
    {
        currentWalls.ForEach(Destroy);
        currentWalls.Clear();

        float wallThickness = 0.2f;
        float floorY = 1.8f;
        float wallY = floorY + height / 2f;

        Vector3[] positions = new Vector3[]
        {
            new Vector3(0, wallY, length / 2f),
            new Vector3(0, wallY, -length / 2f),
            new Vector3(-width / 2f, wallY, 0),
            new Vector3(width / 2f, wallY, 0),
        };

        Vector3[] scales = new Vector3[]
        {
            new Vector3(width, height, wallThickness),
            new Vector3(width, height, wallThickness),
            new Vector3(wallThickness, height, length),
            new Vector3(wallThickness, height, length),
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, positions[i], Quaternion.identity);
            wall.transform.localScale = scales[i];
            wall.name = "Wall_" + i;
            wall.tag = "Spawnable";

            var selectable = wall.GetComponent<SelectableObject>();
            if (selectable != null)
                selectable.prefabName = "wall";

            ObjectSpawner.createdObjects.Add(wall);
            currentWalls.Add(wall);
        }
    }
}
