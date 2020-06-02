using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Range(-360f,360f)]public float speed = 60f; //Скорость вращения
    public bool isFuel; //Это топливо?
    public bool isCoin; //Это монетка?
    public int score; //Сколько добавляет денег

    //public float bounceSpeed = .5f;
   // private float MaxBounce = .15f;
    //private float MinBounce = -.15f;
    //[SerializeField]
    //private float bounceRange;
    //[SerializeField]
    bool addInteger;

    private void Start() {
        addInteger = true;
        TurnOnOff();
    }

    private void OnTriggerEnter(Collider other) { //Когда игрок соприкасается с предметом
        if(CarController.instance != null && !GetComponent<CarController>()){
            if(isFuel){
            CarController.instance.AddFuel();
            Destroy(gameObject);
            }
        }

        if(CarController.instance != null && !GetComponent<CarController>()){
            if(isCoin)
            {
                GameManager.instance.ADD_SCORE(score);
            }
            Destroy(gameObject);
        }
    }

    public void TurnOnOff() //Включить предмет, если бензина мало
    {
        if(GetComponent<MeshRenderer>() != null && GetComponent<BoxCollider>() != null){
        if(isFuel)
        {
            if(GameManager.instance.gameData.fuel < 5f)
            {
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<BoxCollider>().enabled = true; 
            }else{
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false; 
            }
        }else{
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
        }
        }
    }
    private void Update() {
    //    if(addInteger)
    //    {
    //        bounceRange += Time.deltaTime * bounceSpeed;
    //    }else{
    //        bounceRange -= Time.deltaTime * bounceSpeed;
    //    }

    //    if(bounceRange > MaxBounce)
    //    {
    //        addInteger = false;
    //    }
    //    if(bounceRange < MinBounce)
    //    {
    //        addInteger = true;
    //   }

        transform.RotateAround(transform.position,Vector3.up,Time.deltaTime * speed); //Кручение
        float origin = transform.position.y - transform.position.y;
       // transform.position = new Vector3(transform.position.x,transform.position.y + bounceRange,transform.position.z);
    }
}
