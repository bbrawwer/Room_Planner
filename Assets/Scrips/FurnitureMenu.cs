using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FurnitureMenu : MonoBehaviour
{
    [System.Serializable]
    public class FurnitureItem
    {
        public string Name; 
        public string Type; 
        public GameObject Prefab; 
    }

    [Header("Список мебели")]
    public List<FurnitureItem> furnitureItems = new List<FurnitureItem>(); // Теперь список в инспекторе

    [Header("UI")]
    public GameObject buttonPrefab;
    public Transform contentPanel;
    public Dropdown sortDropdown;
    public GameObject emptyBlock;

    private List<GameObject> spawnedButtons = new List<GameObject>();
    private List<FurnitureItem> allFurnitureItems = new List<FurnitureItem>();
    public GameObject selectedPrefab;

    void Start()
    {
        allFurnitureItems = furnitureItems; // Берём всё из списка
        PopulateMenu();
        sortDropdown.onValueChanged.AddListener(OnSortChanged);
    }

    public void PopulateMenu()
    {
        ClearButtons();

        if (emptyBlock != null)
            emptyBlock.SetActive(false);

        foreach (var item in allFurnitureItems)
        {
            CreateButton(item);
        }
    }

    void CreateButton(FurnitureItem item)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, contentPanel);

        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
            buttonText.text = item.Name;

        Button btn = buttonObj.GetComponent<Button>();
        btn.onClick.AddListener(() => SelectItem(item));

        var dragHandler = buttonObj.GetComponent<FurnitureDragHandler>();
        if (dragHandler != null)
            dragHandler.prefabToSpawn = item.Prefab;

        spawnedButtons.Add(buttonObj);
    }

    void SelectItem(FurnitureItem item)
    {
        Debug.Log("Вы выбрали: " + item.Name);
        selectedPrefab = item.Prefab;
    }

    void OnSortChanged(int index)
    {
        string selectedType = sortDropdown.options[index].text;

        if (selectedType == "Все")
        {
            PopulateMenu();
        }
        else
        {
            var filteredItems = allFurnitureItems.Where(x => x.Type == selectedType).ToList();
            PopulateFilteredMenu(filteredItems);
        }
    }

    void PopulateFilteredMenu(List<FurnitureItem> filteredItems)
    {
        ClearButtons();

        if (emptyBlock != null)
            emptyBlock.SetActive(filteredItems.Count == 0);

        foreach (var item in filteredItems)
        {
            CreateButton(item);
        }
    }

    void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            Destroy(btn);
        }
        spawnedButtons.Clear();
    }

    public void DeleteItem(string itemName)
    {
        var itemToRemove = allFurnitureItems.FirstOrDefault(x => x.Name == itemName);
        if (itemToRemove != null)
        {
            allFurnitureItems.Remove(itemToRemove);
            PopulateMenu();
        }
    }
}
