using UnityEngine;

public class PickupSpawner : MonoBehaviour {
    [Range(0f,1f)]public float spawnChance;

    public GameObject Item;

    private void Start() {
        if(Random.value <= spawnChance)
        {
        Instantiate(Item,transform.position,gameObject.transform.rotation,gameObject.transform);
        }
    }
}