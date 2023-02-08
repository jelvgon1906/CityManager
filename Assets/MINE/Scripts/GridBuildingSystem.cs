using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridBuildingSystem : MonoBehaviour
{
    private GridXZ<GridObject> grid;

    //Lista de prefabs
    [SerializeField] private List<BuilldingPreset> builldingPresetList;
    private BuilldingPreset builldingPreset;
    private BuilldingPreset.Dir dir = BuilldingPreset.Dir.Down;

    //Capa colision ray raton
    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();

    public event EventHandler OnSelectedChanged;

    public static GridBuildingSystem Instance { get; private set; }


    public bool currentlyBulldozering;
    public GameObject placementIndicator;
    public GameObject bulldozeIndicator;


    private void Awake()
    {
        //Grid options
        int gridWidth = 30;
        int gridHeight = 30;
        float cellSize = 10f;
        float offsetGrid = (gridWidth / 2) * cellSize;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(-offsetGrid, 0, -offsetGrid), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));

        //List prefabs
        builldingPreset = builldingPresetList[0];

        //Placement
        currentlyBulldozering = false;
    }

    public class GridObject
    {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject placedObject;
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

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject GetPlacedObject()
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
        if (Input.GetMouseButtonDown(0) && !currentlyBulldozering)
        {
            grid.GetXZ(GetMouseWorldPosition(), out int x, out int z); //Localización en el grid

            GridObject gridObject = grid.GetGridObject(x, z);

            List<Vector2Int> gridPositionList = builldingPreset.GetGridPositionList(new Vector2Int(x, z), dir); //Grid positions occupied by building

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
                //Offset de la rotacion
                Vector2Int rotationOffset = builldingPreset.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                //Transform builtTransform = Instantiate(builldingPreset.prefab.transform, /*grid.GetWorldPosition(x, z)*/placedObjectWorldPosition, Quaternion.Euler(0, builldingPreset.GetRotationAngle(dir), 0));

                
                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, builldingPreset);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);//.SetTransform(builtTransform);
                }
            }
            else
            {
                print("You can't build there");
                //TODO: You can't build there
            }

            

        }
        else if (Input.GetMouseButtonDown(0) && currentlyBulldozering)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            if (grid.GetGridObject(mousePosition) != null)
            {
                PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                if (placedObject != null)
                {
                    placedObject.DestroySelf();

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }
        }
        

        //Rotar el edificio
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = BuilldingPreset.GetNextDir(dir);
        }
        //Cambiar Destruir/Construir
        if (Input.GetKeyUp(KeyCode.B))
        {
            ToggleBulldoze();
        }

        //Deseleccionar edificio
        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }

        //Seleccionar edificios
        if (Input.GetKeyDown(KeyCode.Alpha1)) { builldingPreset = builldingPresetList[0]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { builldingPreset = builldingPresetList[1]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { builldingPreset = builldingPresetList[2]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { builldingPreset = builldingPresetList[3]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { builldingPreset = builldingPresetList[4]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { builldingPreset = builldingPresetList[5]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { builldingPreset = builldingPresetList[6]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { builldingPreset = builldingPresetList[7]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { builldingPreset = builldingPresetList[8]; RefreshSelectedObjectType(); if (currentlyBulldozering) { ToggleBulldoze(); } }


        if (!currentlyBulldozering)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.position = GetMouseWorldSnappedPosition();
        }
        else if (currentlyBulldozering)
        {
            bulldozeIndicator.SetActive(true);
            bulldozeIndicator.transform.position = GetMouseWorldSnappedPosition();
        }
    }


    private void DeselectObjectType()
    {
        builldingPreset = null; RefreshSelectedObjectType();
    }
    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
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
            return new Vector3(1000,1000,1000);
        }
    }
    public void ToggleBulldoze()
    {
        placementIndicator.SetActive(false);
        bulldozeIndicator.SetActive(false);
        currentlyBulldozering = !currentlyBulldozering;
    }



    //Ghost TODO

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }
    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (builldingPreset != null)
        {
            Vector2Int rotationOffset = builldingPreset.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedBuilldingPresetRotation()
    {
        if (builldingPreset != null)
        {
            return Quaternion.Euler(0, builldingPreset.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public BuilldingPreset GetPlacedBuilldingPreset()
    {
        return builldingPreset;
    }
}
