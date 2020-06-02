using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour {
    public static StoreManager instance;
    public store StoreData;
    public Text StoreScore;

    private void Awake() {
        instance = this;
    }

    private void OnEnable() {
        GameManager.instance.UPDATE_STORE();
    }

    public void CANCEL_PREVIEW()
    {
        GameManager.instance.CANCEL_PREVIEW();
    }

    public void PREVIEW()
    {
        GameManager.instance.PREVIEW();
    }

    public void INSTALL()
    {
        GameManager.instance.INSTALL();
    }

    public void UPDATE_STORE()
    {
        GameManager.instance.UPDATE_STORE();
    }

}

[System.Serializable]
public class store
{ //Класс магазина 

    public GameObject StoreItemPrefab; //Префаб-заготовка объекта-товара для магазина
    public Image StoreItemThumbnail; //Иконка при покупке на котором будет показываться иконка
    public GameObject BuyWindowUI;
    public Button StoreInstallBtn;
    public Button CancelBtn;
    public Transform StoreParentContent; //Объект-родитель куда будут спавниться товары
    public List<shop_item> Store = new List<shop_item>(); //Магазин
    public List<GameObject> spawned_store_items;
    public string id_of_buy;

    [System.Serializable]
    public class shop_item
    { //Класс товара
        public string id; //Айдишник
        public Sprite Icon;
        public int value; //Цена за товар
        public bool isBought; //Куплен ли данный товар?
        public bool body;
        public bool attach;
        public bool wheels;
    }
}