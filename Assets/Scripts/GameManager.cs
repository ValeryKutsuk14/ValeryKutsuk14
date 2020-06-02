using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image fuelBarUI; //Прогресс-бар бензина
    public Text scoreUI;
    public game_data gameData; //Игровые данные для загрузки и сохранения
    public GameObject PlayerCar; //Игрок
    public bool paused; //На паузе ли игра?

    string old_body; //Старый визуал корпуса
    string old_attach; //Старый визуал обвеса
    string old_wheels; //Старый визуал колёс

    private string save_path; //Путь сохранения

    public static GameManager instance;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading; //Добавление ивента при загрузке уровня
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading; //Удаление ивента при отключении уровня
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) //Грубо говоря - это callback при загрузке уровня
    {
        scoreUI = SearchTextByTag("text_score");
        fuelBarUI = SearchImageByTag("image_fuel");
        UPDATE_SCORE();
    }

    private Image SearchImageByTag(string tag) //Поиск UI картинки по тэгу
    {
        if (GameObject.FindGameObjectWithTag(tag))
        {
            return GameObject.FindGameObjectWithTag(tag).GetComponent<Image>();
        }
        else
        {
            return null;
        }
    }

    private Text SearchTextByTag(string tag) //Поиск UI текста по тэгу
    {
        if (GameObject.FindGameObjectWithTag(tag))
        {
            return GameObject.FindGameObjectWithTag(tag).GetComponent<Text>();
        }
        else
        {
            return null;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Start()
    {
        save_path = Application.persistentDataPath + "/"; //Задаём путь сохранения
        LoadGameData(); //Загружаем данные игры
        //ADD_SCORE(1000);
        //DontDestroyOnLoad(instance);
        //UPDATE_VISUALS();
    }

    

    public void ADD_SCORE(int score) //Добавить денег
    {
        gameData.score += score;
        UPDATE_SCORE();
    }

    public void UPDATE_SCORE() //обновить текст денег
    {
        if (scoreUI != null)
        {
            scoreUI.text = gameData.score.ToString();
        }
        if(StoreManager.instance != null && StoreManager.instance.StoreScore != null)
        {
            StoreManager.instance.StoreScore.text = gameData.score.ToString();
        }
    }

    public void GENERATE_RANDOM_VISUAL() //Сгенерировать рандомный визуал
    {
        VisualManager vm = VisualManager.instance;
        string[] bodies = new string[vm.visualDataBase.bodies.ToArray().Length];
        string[] attach = new string[vm.visualDataBase.attachments.ToArray().Length];
        string[] wheels = new string[vm.visualDataBase.wheels.ToArray().Length];

        for (int i = 0; i < vm.visualDataBase.bodies.ToArray().Length; i++)
        {
            bodies[i] = vm.visualDataBase.bodies[i].id;
        }
        for (int i = 0; i < vm.visualDataBase.attachments.ToArray().Length; i++)
        {
            attach[i] = vm.visualDataBase.attachments[i].id;
        }
        for (int i = 0; i < vm.visualDataBase.wheels.ToArray().Length; i++)
        {
            wheels[i] = vm.visualDataBase.wheels[i].id;
        }

        gameData.body_model = bodies[UnityEngine.Random.Range(0, bodies.Length)];
        gameData.attachments = attach[UnityEngine.Random.Range(0, attach.Length)];
        gameData.wheels = wheels[UnityEngine.Random.Range(0, wheels.Length)];

        UPDATE_VISUALS();
    }

    public void UPDATE_STORE() //Обновить магазин
    {
        StoreManager sm = StoreManager.instance;
        old_body = gameData.body_model;
        old_attach = gameData.attachments;
        old_wheels = gameData.wheels;

        UPDATE_SCORE();

        if (sm.StoreData.spawned_store_items.Count > 0)
        {
            foreach (var item in sm.StoreData.spawned_store_items)
            {
                Destroy(item);
            }
            sm.StoreData.spawned_store_items.Clear();
        }

        if (sm.StoreData.StoreItemPrefab != null && sm.StoreData.StoreParentContent != null)
        {

            foreach (var item in sm.StoreData.Store)
            {
                GameObject store_item = Instantiate(sm.StoreData.StoreItemPrefab, sm.StoreData.StoreParentContent);
                store_item.name = item.id;
                store_item.transform.GetChild(0).GetComponent<Image>().sprite = item.Icon;
                if (!item.isBought)
                {
                    store_item.transform.GetChild(1).GetComponent<Text>().text = item.value.ToString();
                }
                else
                {
                    store_item.transform.GetChild(1).GetComponent<Text>().text = "Already bought";
                }

                sm.StoreData.spawned_store_items.Add(store_item);
            }
        }
    }

    public void BUY(GameObject obj) //Купить товар
    {
        StoreManager sm = StoreManager.instance;
        UPDATE_SCORE();
        sm.StoreData.BuyWindowUI.SetActive(true);
        sm.StoreData.StoreItemThumbnail.sprite = obj.transform.GetChild(0).GetComponent<Image>().sprite;
        foreach (var item in sm.StoreData.Store)
        {
            if (item.id == obj.name)
            {
                sm.StoreData.id_of_buy = obj.name;
                if (gameData.score >= item.value || item.isBought)
                {
                    sm.StoreData.StoreInstallBtn.interactable = true;
                }
                if (gameData.score < item.value && !item.isBought)
                {
                    sm.StoreData.StoreInstallBtn.interactable = false;
                }

                if (item.isBought)
                {
                    sm.StoreData.BuyWindowUI.transform.GetChild(0).GetComponent<Text>().text = "Already bought";
                }
                else
                {
                    sm.StoreData.BuyWindowUI.transform.GetChild(0).GetComponent<Text>().text = item.value.ToString();
                }
                break;
            }
        }
    }

    public void INSTALL() //Установить\Купить продукт
    {
        StoreManager sm = StoreManager.instance;
        foreach (var item in sm.StoreData.Store)
        {
            if (sm.StoreData.id_of_buy == item.id)
            {
                if (item.body)
                {
                    if (item.isBought)
                    {
                        old_body = item.id;
                        gameData.body_model = item.id;
                        sm.StoreData.CancelBtn.onClick.Invoke();
                    }
                    else
                    {
                        if (gameData.score >= item.value)
                        {
                            gameData.score -= item.value;
                            item.isBought = true;
                            UPDATE_SCORE();
                            old_body = item.id;
                            gameData.body_model = item.id;
                            sm.StoreData.CancelBtn.onClick.Invoke();
                        }
                    }
                }
                if (item.attach)
                {
                    if (item.isBought)
                    {
                        old_attach = item.id;
                        gameData.attachments = item.id;
                        sm.StoreData.CancelBtn.onClick.Invoke();
                    }
                    else
                    {
                        if (gameData.score >= item.value)
                        {
                            gameData.score -= item.value;
                            item.isBought = true;
                            UPDATE_SCORE();
                            old_attach = item.id;
                            gameData.attachments = item.id;
                            sm.StoreData.CancelBtn.onClick.Invoke();
                        }
                    }

                }
                if (item.wheels)
                {
                    if (item.isBought)
                    {
                        old_wheels = item.id;
                        gameData.wheels = item.id;
                        sm.StoreData.CancelBtn.onClick.Invoke();
                    }
                    else
                    {
                        if (gameData.score >= item.value)
                        {
                            gameData.score -= item.value;
                            item.isBought = true;
                            UPDATE_SCORE();
                            old_wheels = item.id;
                            gameData.wheels = item.id;
                            sm.StoreData.CancelBtn.onClick.Invoke();
                        }
                    }

                }
                break;
            }
        }
        SaveGameData();
    }

    public void PREVIEW() //Предпросмотр товара
    {
        StoreManager sm = StoreManager.instance;
        foreach (var item in sm.StoreData.Store)
        {
            if (sm.StoreData.id_of_buy == item.id)
            {
                if (item.body)
                {
                    gameData.body_model = item.id;
                }
                if (item.attach)
                {
                    gameData.attachments = item.id;
                }
                if (item.wheels)
                {
                    gameData.wheels = item.id;
                }
                break;
            }
        }
        UPDATE_VISUALS();
    }

    public void CANCEL_PREVIEW()
    {
        gameData.body_model = old_body;
        gameData.attachments = old_attach;
        gameData.wheels = old_wheels;
        UPDATE_STORE();
        UPDATE_VISUALS();
    }

    public void UPDATE_VISUALS() //Обновить визуалы 
    {
        VisualManager vm = VisualManager.instance;

        var bodies = vm.visualDataBase.bodies;
        var attach = vm.visualDataBase.attachments;
        var wheels = vm.visualDataBase.wheels;

        var car_body = CarController.instance.BodyModel.GetComponent<MeshRenderer>();
        var car_attach = CarController.instance.AttachModel.GetComponent<MeshRenderer>();
        var w_back_l = CarController.instance.w_back_l.GetComponent<MeshRenderer>();
        var w_back_r = CarController.instance.w_back_r.GetComponent<MeshRenderer>();
        var w_front_l = CarController.instance.w_front_l.GetComponent<MeshRenderer>();
        var w_front_r = CarController.instance.w_front_r.GetComponent<MeshRenderer>();

        var car_body_mesh = CarController.instance.BodyModel.GetComponent<MeshFilter>();
        var car_attach_mesh = CarController.instance.AttachModel.GetComponent<MeshFilter>();
        var w_back_l_mesh = CarController.instance.w_back_l.GetComponent<MeshFilter>();
        var w_back_r_mesh = CarController.instance.w_back_r.GetComponent<MeshFilter>();
        var w_front_l_mesh = CarController.instance.w_front_l.GetComponent<MeshFilter>();
        var w_front_r_mesh = CarController.instance.w_front_r.GetComponent<MeshFilter>();

        if (PlayerCar != null)
        {
            foreach (var b in bodies)
            {
                if (gameData.body_model == b.id)
                {
                    car_body.sharedMaterials = b.model.GetComponent<MeshRenderer>().sharedMaterials;
                    car_body_mesh.sharedMesh = b.model.GetComponent<MeshFilter>().sharedMesh;
                    break;
                }
            }
            foreach (var a in attach)
            {
                if (gameData.attachments == a.id)
                {
                    car_attach.sharedMaterials = a.model.GetComponent<MeshRenderer>().sharedMaterials;
                    car_attach_mesh.sharedMesh = a.model.GetComponent<MeshFilter>().sharedMesh;
                    break;
                }
            }
            foreach (var w in wheels)
            {
                if (gameData.wheels == w.id)
                {
                    w_back_l.sharedMaterials = w.RightBack.GetComponent<MeshRenderer>().sharedMaterials;
                    w_back_l_mesh.sharedMesh = w.RightBack.GetComponent<MeshFilter>().sharedMesh;

                    w_back_r.sharedMaterials = w.LeftBack.GetComponent<MeshRenderer>().sharedMaterials;
                    w_back_r_mesh.sharedMesh = w.LeftBack.GetComponent<MeshFilter>().sharedMesh;

                    w_front_l.sharedMaterials = w.RightFront.GetComponent<MeshRenderer>().sharedMaterials;
                    w_front_l_mesh.sharedMesh = w.RightFront.GetComponent<MeshFilter>().sharedMesh;

                    w_front_r.sharedMaterials = w.LeftFront.GetComponent<MeshRenderer>().sharedMaterials;
                    w_front_r_mesh.sharedMesh = w.LeftFront.GetComponent<MeshFilter>().sharedMesh;
                    break;
                }
            }
        }
    }



    public void LoadGameData() //Загрузка игровых данных из файловой системы устройства
    {
        if (File.Exists(save_path + "data.json"))
        {
            gameData = JsonUtility.FromJson<game_data>(File.ReadAllText(save_path + "data.json"));
            Debug.Log("Game data was successfuly loaded!");
        }
    }

    public void PLAY_ZONE(int index) //Загрузить сцену
    {
        SceneManager.LoadScene(index);
    }

    public void DeleteGameData() //Удалить игровые сохранения
    {
        if (File.Exists(save_path + "data.json"))
        {
            File.Delete(save_path + "data.json");
            Debug.Log("Done! Data are deleted!");
        }
        else
        {
            Debug.LogWarning("There is no data files to delete!");
        }

        if (File.Exists(save_path + "store.json"))
        {
            File.Delete(save_path + "store.json");
            Debug.Log("Done! Store data are deleted!");
        }
        else
        {
            Debug.LogWarning("There is no store files to delete!");
        }

        if (File.Exists(save_path + "upgrade.json"))
        {
            File.Delete(save_path + "upgrade.json");
            Debug.Log("Done! Upgrade data are deleted!");
        }
        else
        {
            Debug.LogWarning("There is no upgrade files to delete!");
        }
        Application.Quit();
    }

    private void OnApplicationQuit() //Callback при выходе из игры
    {
        //    SaveGameData(); //Сохранение игры и игровых данных
    }

    public void QUIT_FROM_GAME() //Для кнопок
    {
        Application.Quit();
    }

    public void SaveGameData() //Сохранение данных
    {
        string json_data = JsonUtility.ToJson(gameData);
        File.WriteAllText(save_path + "data.json", json_data);
        Debug.Log("Game data was succesfully saved!");
    }
}

[Serializable]
public class game_data
{
    public int score; //Очки (Деньги)
    public float fuel = 25f; //Максимальный уровень бензина
    public float maxFuel = 0f; //Переменная для хранения максимального уровня бензина на время уровня
    public float fuelSpeed = 0.75f;
    public string body_model; //ID модели корпуса
    public string attachments; //ID модели декоративных элементов
    public string wheels; //ID модели колёс
    public bool level_2; //Открыт ли доступ к уровню ОСЕНЬ
    public bool level_3; //Открыт ли доступ к уровню ЗИМА

    public float gearTorque; //Скорость транспорта
    public float maxSteeringAngle; //Скорость поворота транспорта в воздухе

}