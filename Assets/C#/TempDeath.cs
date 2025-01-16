using UnityEngine;

public class TempDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<Player>().Die();
    }
}