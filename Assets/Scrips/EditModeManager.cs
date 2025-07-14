using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }

    public bool IsEditing { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void ToggleEditing()
    {
        IsEditing = !IsEditing;
        Debug.Log("Режим редактирования: " + (IsEditing ? "ВКЛ" : "ВЫКЛ"));
    }

    public void ForceDisable()
    {
        IsEditing = false;
    }
}
