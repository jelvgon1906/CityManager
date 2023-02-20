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

    public TextMeshProUGUI statsText, timeText;

    public static City instance;
    public List<Building> buildings = new List<Building>();

    //DayCicle
    [SerializeField] float curDayTime;
    [SerializeField] float dayTime = 1440;//24 horas
    [SerializeField] GameObject sun;
    [SerializeField] GameObject luz1, luz2, luz3, luz4;
    string time;
    float timeMultiplier = 1f;
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
    public void OnPlaceBuilding(Building building)
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

        time = (minutes.ToString("00") + ":00" /*+ seconds.ToString("00")*/);
        UpdateTimeText();
        if (curDayTime >= dayTime)
        {
            curDayTime = 0;
            EndTurn();
            lucesoff();

        }
        if(curDayTime >= (dayTime/2)){
            
            luceson();
        } 

        sun.transform.rotation = Quaternion.Euler((curDayTime / dayTime) * 360 + 270, 0f, 0f);

        /*RenderSettings.skybox.SetFloat("");*/
    }

    private void lucesoff()
    {
            luz1.gameObject.SetActive(false);
        
            luz2.gameObject.SetActive(false);

            luz3.gameObject.SetActive(false);
        
            luz4.gameObject.SetActive(false);
        
    }
    private void luceson()
    {
        luz1.gameObject.SetActive(true);

        luz2.gameObject.SetActive(true);

        luz3.gameObject.SetActive(true);

        luz4.gameObject.SetActive(true);

    }

    public void EndTurn()
    {
        day++;

        /*CalculateMoney();*/
        CalculatePopulation();
        CalculateJobs();
        /*CalculateFood();*/

        UpdateStatText();
    }

    private void UpdateStatText()
    {
        statsText.text = String.Format("Dinero: {0} Poblacion: {1}/{2} Trabajos: {3}/{4} Comida: {5}", new object[6] {money.ToString("0000"), curPopulation.ToString("00"), maxPopulation.ToString("00"), curJobs.ToString("00"), maxJobs.ToString("00"), curFood.ToString("0000") });
    }
    private void UpdateTimeText()
    {
        timeText.text = String.Format("{0} x {1} \n Day: {2}", new object[3] {  time, timeMultiplier, day });
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

        foreach (Building building in buildings)
            money -= building.preset.costPerTurn;
    }

    public void OnClickPlusMultiplier()
    {
        if (timeMultiplier < 3f)
        {
            timeMultiplier++;
        }
    }

    public void OnClickMinusMultiplier()
    {
        if (timeMultiplier > 0.25f)
        {
            timeMultiplier = timeMultiplier - 0.5f;
        }
    }

    public void OnClickPlayStopMultiplier()
    {
        if(timeMultiplier != 0f ) 
        {
            timeMultiplier = 0f;
        }
        else
        {
            timeMultiplier = 1f; 
        }
       
    }
}
