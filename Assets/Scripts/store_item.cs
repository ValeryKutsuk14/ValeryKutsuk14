using UnityEngine;

public class store_item : MonoBehaviour {
    
    public void BUY()
    {
        GameManager.instance.BUY(gameObject);
    }
}