using UnityEngine.SceneManagement;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ground")
        {
            CarController.instance.isDead = true; 
            CarController.instance.PAUSE();
        }
    }
}
