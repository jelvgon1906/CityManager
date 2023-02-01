using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    private GridXZ<GridObject> grid;

    [SerializeField] private BuilldingPreset builldingPreset;

    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();


    private void Awake()
    {
        int gridWidth = 30;
        int gridHeight = 30;
        float cellSize = 10f;
        float offsetGrid = (gridWidth / 2) * cellSize;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(-offsetGrid, 0, -offsetGrid), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }

    public class GridObject
    {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public Transform placedObject;
        public GridObject(GridXZ<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return x + ", " + y;
        }

        public void SetPlacedObject(Transform placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public Transform GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.GetXZ(GetMouseWorldPosition(), out int x, out int z); //Localización en el grid

            GridObject gridObject = grid.GetGridObject(x, z);

            List<Vector2Int> gridPositionList = builldingPreset.GetGridPositionList(new Vector2Int(x, z), BuilldingPreset.Dir.Down); //Grid positions occupied by building

            //Can you build?
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }
            if (canBuild)
            {
                Transform builtTransform = Instantiate(builldingPreset.prefab.transform, grid.GetWorldPosition(x, z), Quaternion.identity);
                
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(builtTransform);
                }
            }
            else
            {
                print("You can't build there");
                //TODO: You can't build there
            }
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
