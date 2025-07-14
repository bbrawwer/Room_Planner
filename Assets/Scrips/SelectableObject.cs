using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public enum ResizableType { None, Wall, Floor }

    [Header("Тип объекта (для масштабирования)")]
    public ResizableType type = ResizableType.None;

    [Header("Подсветка")]
    public Material outlineMaterial;

    [Header("Ручки масштабирования")]
    public GameObject scaleHandlePrefab;

    [HideInInspector] public string prefabName = "";

    private GameObject xHandle, zHandle;
    private Renderer rend;
    private Material outlineInstance;
    private Material originalMaterial;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;
    }

    public void Select()
    {
        if (!EditModeManager.Instance.IsEditing) return;

        if (xHandle != null) Destroy(xHandle);
        if (zHandle != null) Destroy(zHandle);

        if (rend != null && outlineMaterial != null)
        {
            outlineInstance = new Material(outlineMaterial);
            outlineInstance.SetColor("_OutlineColor", Color.green); // начальный цвет

            if (originalMaterial != null && originalMaterial.HasProperty("_MainTex"))
                outlineInstance.SetTexture("_MainTex", originalMaterial.GetTexture("_MainTex"));

            if (originalMaterial != null && originalMaterial.HasProperty("_Color"))
                outlineInstance.SetColor("_Color", originalMaterial.GetColor("_Color"));

            rend.material = outlineInstance;
        }

        if (type != ResizableType.None && scaleHandlePrefab != null)
        {
            xHandle = Instantiate(scaleHandlePrefab);
            xHandle.transform.position = transform.position + transform.right * transform.localScale.x / 2f + Vector3.up * 0.1f;
            xHandle.GetComponent<ScaleHandle>().target = transform;
            xHandle.GetComponent<ScaleHandle>().handleType =
                (type == ResizableType.Wall) ? ScaleHandle.HandleType.Wall : ScaleHandle.HandleType.Floor;

            zHandle = Instantiate(scaleHandlePrefab);
            zHandle.transform.position = transform.position + transform.forward * transform.localScale.z / 2f + Vector3.up * 0.1f;
            zHandle.GetComponent<ScaleHandle>().target = transform;
            zHandle.GetComponent<ScaleHandle>().handleType =
                (type == ResizableType.Wall) ? ScaleHandle.HandleType.Wall : ScaleHandle.HandleType.Floor;
        }

        UpdateOutlineColor(); // первичная проверка
    }

    public void Deselect()
    {
        if (!EditModeManager.Instance.IsEditing) return;

        if (xHandle != null) Destroy(xHandle);
        if (zHandle != null) Destroy(zHandle);

        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial;
    }

    public void UpdateOutlineColor()
    {
        if (rend == null || outlineInstance == null) return;

        bool tooClose = false;
        GameObject[] all = GameObject.FindGameObjectsWithTag("Spawnable");

        foreach (GameObject obj in all)
        {
            if (obj == this.gameObject) continue;

            var other = obj.GetComponent<SelectableObject>();
            if (other != null && other.prefabName == this.prefabName)
            {
                float dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist < 1.2f)
                {
                    tooClose = true;
                    break;
                }
            }
        }

        outlineInstance.SetColor("_OutlineColor", tooClose ? Color.red : Color.green);
    }
}
