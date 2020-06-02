using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour {
    public car_visual_database visualDataBase; //Хранилище всех визуальных частей транспорта
    public static VisualManager instance;
    public GameObject AutumnPanel;
    public GameObject WinterPanel;

    private void Awake() {
        instance = this;
    }

    public void UNLOCK_LEVEL(int level)
    {
        if(level == 1) //Autumn
        {
            GameManager.instance.gameData.level_2 = true;
            GameManager.instance.gameData.score -= 3500;
        }
        if(level == 2) //Winter
        {
            GameManager.instance.gameData.level_3 = true;
            GameManager.instance.gameData.score -= 5500;
        }
        GameManager.instance.SaveGameData();
    }

    public void PLAY_LEVEL(int l)
    {
        GameManager.instance.PLAY_ZONE(l);
    }

    public void QUIT(){
        GameManager.instance.QUIT_FROM_GAME();
    }

    public void DELETE_SAVES()
    {
        GameManager.instance.DeleteGameData();
    }

    private void Update() {
        if(GameManager.instance.gameData.score >= 3500)
        {
            AutumnPanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }else{
            AutumnPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
        if(GameManager.instance.gameData.score >= 5500)
        {
            WinterPanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }else{
            WinterPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }

        if(GameManager.instance.gameData.level_2)
        {
            AutumnPanel.SetActive(false);
        }else{
            AutumnPanel.SetActive(true);
        }   

        if(GameManager.instance.gameData.level_3)
        {
            WinterPanel.SetActive(false);
        }else{
            WinterPanel.SetActive(true);
        }   
    }
}

[System.Serializable]
public class car_visual_database
{

    public List<data> bodies = new List<data>(); //База данных всех моделей корпусов
    public List<data> attachments = new List<data>(); //База данных всех моделей декоративных элементов
    public List<wheel_data> wheels = new List<wheel_data>(); //База данных всех колёс

    [System.Serializable]
    public class data
    { //Данные для БД для корпусов и декоративных моделей
        public string id; //Айдишник модели
        public GameObject model; //Модель
    }

    [System.Serializable]
    public class wheel_data
    { //Класс данных для БД для колёс
        public string id; //Айдишник
        public GameObject LeftBack; //Левое заднее колесо
        public GameObject RightBack; //Правое заднее колесо 
        public GameObject LeftFront; //Левое переднее колесо
        public GameObject RightFront; //Правое переднее колесо
    }
}