using System.Collections;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        // StartCoroutine(destroyCoroutine());
        if(CarController.instance != null){
        if (Vector3.Distance(transform.position, CarController.instance.transform.position) > 120f){
            Destroy(gameObject);
        }
        }
    }

    IEnumerator destroyCoroutine()
    {
        yield return new WaitForSeconds(5f);
        selfDestroy();
    }

    void selfDestroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}