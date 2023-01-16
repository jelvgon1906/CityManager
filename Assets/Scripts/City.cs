using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class City : MonoBehaviour
{
    public int money;
    public int day;
    public int curPopulation;
    public int curJobs;
    public int curFood;
    public int maxPopulation;
    public int maxJobs;
    public int incomePerJob;

    public TextMeshProUGUI statsText;

    public List<Building> buildings = new List<Building>();

    public static City instance;

    //DayCicle
    public float curDayTime;
    public float dayTime;
    public GameObject sun;
    string time;
    public float timeMultiplier = 1;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateStatText();
    }

    private void FixedUpdate()
    {
        DayCicle();
    }

    //called when we place down a building
    public void OnPlaceBuilding (Building building)
    {
        buildings.Add(building);

        money -= building.preset.cost;

        maxPopulation += building.preset.population;
        maxJobs += building.preset.jobs;

        UpdateStatText();
    }

    //called when we bulldoze a building
    public void OnRemoveBuilding(Building building)
    {
        buildings.Remove(building);

        maxPopulation -= building.preset.population;
        maxJobs -= building.preset.jobs;
        Destroy(building.gameObject);

        UpdateStatText();
    }

    public void DayCicle()
    {
        curDayTime += Time.deltaTime * timeMultiplier * 60;

        int minutes = (int)curDayTime / 60;
        int seconds = (int)curDayTime % 60;

        time = (minutes.ToString("00") + ":" + seconds.ToString("00"));
        UpdateStatText();
        if (curDayTime >= dayTime)
        {
            curDayTime = 0;
            EndTurn();
        }

        sun.transform.rotation = Quaternion.Euler((curDayTime / dayTime) * 360, 0f, 0f);

        /*RenderSettings.skybox.SetFloat("");*/
    }

    public void EndTurn()
    {
        day++;

        CalculateMoney();
        CalculatePopulation();
        CalculateJobs();
        CalculateFood();

        UpdateStatText();
    }

    private void UpdateStatText()
    {
        statsText.text = String.Format("Day: {0} Money: {1} Pop: {2}/{3} Jobs: {4}/{5} Food: {6} Time: {7} Multy: x{8}", new object[9] {day, money, curPopulation, maxPopulation, curJobs, maxJobs, curFood, time, timeMultiplier});
    } 

    private void CalculateFood()
    {
        curFood = 0;

        foreach (Building building in buildings)
            curFood += building.preset.food;
    }

    private void CalculateJobs()
    {
        curJobs = Mathf.Min(curPopulation, maxJobs);
    }

    private void CalculatePopulation()
    {
        if (curFood >= curPopulation && curPopulation < maxPopulation)
        {
            curFood -= curPopulation / 4;
            curPopulation = Mathf.Min(curPopulation + (curFood / 4 ) , maxPopulation);
        }
        else if (curFood < curPopulation)
        {
            curPopulation = curFood;
        }
    }

    private void CalculateMoney()
    {
        money += curJobs * incomePerJob;

        foreach(Building building in buildings) 
            money -= building.preset.costPerTurn;
    }

    public void OnClickPlusMultiplier()
    {
        if (timeMultiplier < 2)
        {
            timeMultiplier = timeMultiplier * 2;
        }
    }

    public void OnClickMinusMultiplier()
    {
        if (timeMultiplier > 0.25)
        {
            timeMultiplier = timeMultiplier / 2;
        }
    }

    public void OnClickPlayStopMultiplier()
    {
        if(timeMultiplier != 0 ) 
        {
            timeMultiplier = 0;
        }
        else
        {
            timeMultiplier = 1; 
        }
       
    }
}
