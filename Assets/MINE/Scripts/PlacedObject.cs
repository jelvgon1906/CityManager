using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{

    //TODO

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, BuilldingPreset.Dir dir, BuilldingPreset builldingPreset)
    {
        Transform placedObjectTransform = Instantiate(builldingPreset.prefab.transform, worldPosition, Quaternion.Euler(0, builldingPreset.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.builldingPreset = builldingPreset;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;
    }

    private BuilldingPreset builldingPreset;
    private Vector2Int origin;
    private BuilldingPreset.Dir dir;
    private void Setup(BuilldingPreset builldingPreset, Vector2Int origin, BuilldingPreset.Dir dir)
    {
        this.builldingPreset = builldingPreset;
        this.origin = origin;
        this.dir = dir;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return builldingPreset.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    /*public override string ToString()
    {
        return builldingPreset.nameString;
    }*/

}
