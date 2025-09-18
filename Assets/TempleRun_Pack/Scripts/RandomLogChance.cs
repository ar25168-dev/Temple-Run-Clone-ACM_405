using UnityEngine;

public class RandomSpawnChildren : MonoBehaviour
{
    [Range(0f, 100f)]
    [SerializeField] private float spawnChance = 5f; // Default 5%

    private void OnEnable()
    {
        // Loop through all direct children of this parent
        foreach (Transform child in transform)
        {
            float roll = Random.Range(0f, 100f);

            // Enable or disable each child based on its own roll
            child.gameObject.SetActive(roll <= spawnChance);
        }
    }
}
