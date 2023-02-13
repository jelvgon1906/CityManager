/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyBulldozering;

    private BuilldingPreset curBuildingPreset;

    private float indicatorUpdateRate = 0.05f; //seconds
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;

    public GameObject placementIndicator;
    public GameObject bulldozeIndicator;

    [SerializeField] GameObject curBuilding;


    //called when we press a building UI button
    public void BeginNewBuildingPlacement(BuilldingPreset preset)
    {
        //TODO: make sure we have enough money

        currentlyPlacing = true;
        currentlyBulldozering = false;
        curBuildingPreset = preset;

        placementIndicator.SetActive(true);

        if (curBuilding == null)
            curBuilding = Instantiate(curBuildingPreset.prefab.transform.GetChild(0).gameObject, placementIndicator.transform);
        else
        {
            Destroy((placementIndicator.transform.GetChild(1).gameObject));
            curBuilding = Instantiate(curBuildingPreset.prefab.transform.GetChild(0).gameObject, placementIndicator.transform);
        }
    }

    private void Update()
    {
        //cancel building placement
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuildingPlacement();

        if (Input.GetKeyDown(KeyCode.R))
        {
            curBuilding.transform.Rotate(0, 90, 0);
        }


        //called every 0.05 seconds 
        if (Time.time - lastUpdateTime > indicatorUpdateRate)
        {
            lastUpdateTime = Time.time;

            //get the currently selected tile position
            curIndicatorPos = Selector.instance.GetCurTilePosition();

            //move the placement indicator or bulldoze indicator to the selected tile
            if (currentlyPlacing)
            {
                placementIndicator.transform.position = curIndicatorPos;
            }
            else if (currentlyBulldozering)
                bulldozeIndicator.transform.position = curIndicatorPos;
        }


        //called when we press left mouse button
        if (Input.GetMouseButtonDown(0) && currentlyPlacing && !EventSystem.current.IsPointerOverGameObject())
            PlaceBuilding();
        else if (Input.GetMouseButtonDown(0) && currentlyBulldozering && !EventSystem.current.IsPointerOverGameObject())
            Bulldoze();
    }

    //places down the currently selected building
    private void PlaceBuilding()
    {
        currentlyBulldozering = false;
        curBuilding.transform.Rotate(0, -90, 0);//TODO salen girados?
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curIndicatorPos, curBuilding.transform.rotation);
        curBuilding.transform.Rotate(0, 90, 0);
        City.instance.OnPlaceBuilding(buildingObj.GetComponent<Building>());

        if (!curBuildingPreset.prefab.tag.Equals("Road"))
            CancelBuildingPlacement();
    }

    //called when we place down a building or press Escape
    private void CancelBuildingPlacement()
    {

        currentlyPlacing = false;
        placementIndicator.SetActive(false);

    }

    //deletes the currently selected building
    private void Bulldoze()
    {
        Building buildingToDestroy = City.instance.buildings.Find(x => x.transform.position == curIndicatorPos);

        if (buildingToDestroy != null)
        {
            City.instance.OnRemoveBuilding(buildingToDestroy);
        }
    }

    //turn bulldoze on off
    public void ToggleBulldoze()
    {
        currentlyPlacing = false;
        currentlyBulldozering = !currentlyBulldozering;
        bulldozeIndicator.SetActive(currentlyBulldozering);
    }
}
*/