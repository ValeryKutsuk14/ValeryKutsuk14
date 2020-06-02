using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    const float pattern_position = 62.907f;
    public int how_many_spawned = 1;

    public GameObject[] Patterns;

    private Transform self_transform;

    private void Start() {
        self_transform = transform;
    }

    private void Update() {
        //Debug.Log(Vector3.Distance(self_transform.position,new Vector3(pattern_position*how_many_spawned,self_transform.position.y,self_transform.position.z)));
        if(Vector3.Distance(self_transform.position,new Vector3(self_transform.position.x,self_transform.position.y,pattern_position*how_many_spawned)) < 50f){
        GameObject go = Instantiate(Patterns[Random.Range(0,Patterns.Length)],new Vector3(0,0,pattern_position*how_many_spawned),Quaternion.Euler(0,-90f,0));
        //go.transform.GetChild(0).GetComponent<PickupObject>().TurnOnOff();
            how_many_spawned += 1;
        }
        
        
    }
}
