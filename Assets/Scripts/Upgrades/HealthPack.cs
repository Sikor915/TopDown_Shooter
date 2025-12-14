using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] float healthRestoreAmount = 25f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false) return;
        Destroy(gameObject.GetComponent<Collider2D>());
        Debug.Log("Player picked up Health Pack");
        other.TryGetComponent<PlayerController>(out var player);
        if (player != null)
        {
            player.RestoreHealth(healthRestoreAmount/2f);
            Destroy(gameObject);
        }
    }
}
