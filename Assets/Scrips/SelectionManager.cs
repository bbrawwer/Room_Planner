using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private SelectableObject selected;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                SelectableObject newSelection = hit.collider.GetComponentInParent<SelectableObject>();

                if (newSelection != null)
                {
                    if (selected != null && selected != newSelection)
                        selected.Deselect();

                    selected = newSelection;
                    selected.Select();
                }
                else if (selected != null)
                {
                    selected.Deselect();
                    selected = null;
                }
            }
        }
    }
}
