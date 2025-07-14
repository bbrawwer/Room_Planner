using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public Transform modelHolder; // Пустышка, куда будем вставлять модель

    public void SetModel(GameObject modelPrefab)
    {
        GameObject model = Instantiate(modelPrefab, modelHolder.position, modelHolder.rotation, modelHolder);
    }
}
