using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{

    public List<AxleInfo> axleInfos; // Информация о каждой индивидуальной оси
    //public float maxMotorTorque; // Максимальная сила оборотов транспорта
    [Range(0, 25f)] public float maxSteeringAngle; // Максимальный угол наклона транспорта

    public float moveSpeed = 0f; //Текущая скорость движения
    //[SerializeField]
    //private int turnPower = 100; //Сила наклона транспорта
    [Range(0, 1f)] public float startSpeed = 0.5f; //Скорость разгона
   
    [Range(0f, 10f)] public float fuelSpeed = 1f; //Скорость убывания бензина
    public bool move; //Двигается ли машина?
    public bool onGround; //На земле ли машина?
    public bool rotate_right; //Делает ли наклон вправо?
    public bool rotate_left; //Делает ли наклон влево?
    public GameObject BodyModel;
    public GameObject AttachModel;
    public GameObject w_back_l;
    public GameObject w_back_r;
    public GameObject w_front_l;
    public GameObject w_front_r;
    public Button ResumeBtn;
    public GameObject PausePanel;
    public GameObject GUIPanel;

    private GameManager gm;
    private Rigidbody rb;
    bool fuelEnd;
    public bool isDead;


    public static CarController instance;

    private void Start()
    {
        gm = GameManager.instance;
        gm.LoadGameData();
        gm.PlayerCar = gameObject;
        gm.gameData.maxFuel = gm.gameData.fuel; //Храним максимальное количество бензина на время уровня (для прогресс-бара)
        instance = this;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += new Vector3(0, -0.5f, .5f); //Устанавливаем центр тяжести транспорта
        //gm.UPDATE_VISUALS();
        //gm.GENERATE_RANDOM_VISUAL();
        if(SceneManager.GetActiveScene().buildIndex != 0){
        gm.UPDATE_VISUALS();
        }
    }

    public void PAUSE(){ //Пауза
        GameManager.instance.paused = !GameManager.instance.paused;
        if(GameManager.instance.paused)
        {
            PausePanel.SetActive(true);
            GUIPanel.SetActive(false);
        }else{
            PausePanel.SetActive(false);
            GUIPanel.SetActive(true);
        }

        if(isDead)
        {
            Time.timeScale = 0f;
            ResumeBtn.interactable = false;
        }else{
            Time.timeScale = 1f;
            ResumeBtn.interactable = true;
        }
    }

    public void TO_MENU() //Выйти в меню
    {
        gm.gameData.fuel = gm.gameData.maxFuel;
        gm.SaveGameData();
        isDead = false;
        PAUSE();
        
        SceneManager.LoadScene(0);
    }

    public void RESTART_ZONE() //Переиграть сцену
    {
        GameManager.instance.gameData.fuel = GameManager.instance.gameData.maxFuel;
        GameManager.instance.SaveGameData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isDead = false;
        PAUSE();
    }

    // Находим визуал колёс
    // Корректно применяем раскрутку
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void AddFuel() //Восполнить топливо
    {
        gm.gameData.fuel = gm.gameData.maxFuel;
        updateFuelBar();
    }

    void updateFuelBar() //Обновление прогресс-бара
    {
        if (gm.fuelBarUI != null)
        {
            gm.fuelBarUI.fillAmount = gm.gameData.fuel / gm.gameData.maxFuel;
        }
    }

    private void Update()
    {
        if (gm.gameData.fuel <= 0 && fuelEnd) //Отключаем возможность движения при отсутсвии топлива
        {
            move = false;
            gm.gameData.fuel = 0f;
            isDead = true;
            PAUSE();
            gm.gameData.fuel = 0.1f;
            fuelEnd = false;
        }

        if(gm.gameData.fuel <= 0)
        {
            fuelEnd = true;
        }

        if (move && onGround) //Если начинаем движение
        {
            if (gm.gameData.fuel > 0) //Отнимаем бензин
            {
                gm.gameData.fuel -= Time.deltaTime * GameManager.instance.gameData.fuelSpeed;
                updateFuelBar();
            }

            moveSpeed = Mathf.Lerp(moveSpeed, gm.gameData.gearTorque, Time.deltaTime); //Устанавливаем скорость оборотов

        }
        
        if(!move && onGround || move && !onGround || !move && onGround && !rotate_left)
        {

            moveSpeed = Mathf.Lerp(moveSpeed, 0f, 1f);

        }

        if(!move && onGround && rotate_left)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, -gm.gameData.gearTorque, Time.deltaTime); //Устанавливаем скорость оборотов
        }

        if (transform.position.x > 0f || transform.position.x < 0f)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        }

        // transform.rotation *= Quaternion.Euler(transform.rotation.eulerAngles.x,0f,0f);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,0f,0f);

    }

    public void BUTTON_MOVE(bool b)
    {
        move = b;
        BUTTON_RIGHT(true);
    }

    public void BUTTON_LEFT(bool b)
    {
        rotate_left = b;
    }

    public void BUTTON_RIGHT(bool b)
    {
        rotate_right = b;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onGround = false;
    }


    private void FixedUpdate()
    {

        Move();
        //    Turn();
    }

    private void Move()
    {
        float motor = moveSpeed;

        if (!onGround && rotate_left)
        {
            rb.AddTorque(gm.gameData.maxSteeringAngle * 10, 0, 0, ForceMode.Impulse);
        }
        if (!onGround && rotate_right)
        {
            rb.AddTorque(-gm.gameData.maxSteeringAngle * 10, 0, 0, ForceMode.Impulse);
        }
        if (move && transform.eulerAngles.x < -10f)
        {
            rb.AddForce(0, 0, 1000f, ForceMode.Impulse);
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;


            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }


}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
}