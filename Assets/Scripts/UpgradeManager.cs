using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Text EngineUI; //Текст улучшения движка
    public Text FuelUI; //Текст улучшения бензина
    public Text CenterOfMassUI; //Текст поворота 

    public static UpgradeManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void CLEAR_GAME_DATA() //Очистка данных
    {
        GameManager.instance.DeleteGameData();
        Application.Quit();
    }

    public void UPGRADE(int index) //Улучшение
    {
        switch (index)
        {
            case 0: //Gear
                if (GameManager.instance.gameData.gearTorque < 1000)
                {
                    GameManager.instance.gameData.score -= 1000;
                    GameManager.instance.gameData.gearTorque += 100;
                    UPDATE_UPGRADES();
                }
                break;

            case 1: //Fuel
                GameManager.instance.gameData.score -= 1000;
                GameManager.instance.gameData.fuel += 10;
                UPDATE_UPGRADES();
                break;

            case 2: //Steering
                if (GameManager.instance.gameData.maxSteeringAngle < 10)
                {
                    GameManager.instance.gameData.score -= 1000;
                    GameManager.instance.gameData.maxSteeringAngle += 1;
                    UPDATE_UPGRADES();
                }
                break;
        }
    }

    public void UPDATE_UPGRADES() //Обновление текстов для апгрейдов
    {
        if (EngineUI != null && FuelUI != null && CenterOfMassUI != null)
        {
            EngineUI.text = GameManager.instance.gameData.gearTorque + " / 1000";
            FuelUI.text = GameManager.instance.gameData.maxFuel + "L";
            CenterOfMassUI.text = GameManager.instance.gameData.maxSteeringAngle + " / 10";
        }
        if (GameManager.instance.gameData.gearTorque < 1000)
        {
            if (GameManager.instance.gameData.score >= 1000)
            {
                EngineUI.transform.parent.GetComponent<Button>().interactable = true;
            }
            else
            {
                EngineUI.transform.parent.GetComponent<Button>().interactable = false;
            }

        }
        else
        {
            EngineUI.transform.parent.GetComponent<Button>().interactable = false;
        }

        if (GameManager.instance.gameData.score >= 1000)
        {
            FuelUI.transform.parent.GetComponent<Button>().interactable = true;
        }
        else
        {
            FuelUI.transform.parent.GetComponent<Button>().interactable = false;
        }

        if (GameManager.instance.gameData.maxSteeringAngle < 10)
        {
            if (GameManager.instance.gameData.score >= 1000)
            {
                CenterOfMassUI.transform.parent.GetComponent<Button>().interactable = true;
            }
            else
            {
                CenterOfMassUI.transform.parent.GetComponent<Button>().interactable = false;
            }

        }
        else
        {
            CenterOfMassUI.transform.parent.GetComponent<Button>().interactable = false;
        }

    }

    public void SAVE_DATA() //Сохранить данные
    {
        GameManager.instance.SaveGameData();
    }
}