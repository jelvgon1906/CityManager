using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGhost : MonoBehaviour
{

    private GameObject visual;
    public BuilldingPreset builldingPreset;

    private void Start()
    {
        RefreshVisual();

    }


    private void LateUpdate()
    {
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedBuilldingPresetRotation(), Time.deltaTime * 15f);
    }

    public void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        builldingPreset = GridBuildingSystem.Instance.GetPlacedBuilldingPreset();

        if (builldingPreset != null)
        {
            visual = Instantiate(builldingPreset.visual, Vector3.zero, Quaternion.identity);
            visual.transform.position = transform.position;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}



