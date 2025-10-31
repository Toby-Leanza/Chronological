using UnityEngine;

public class CloneManager : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject clonePrefab;
    public Transform player;
    public int maxClones = 3;

    private int currentClones = 0;

    void Update()
    {
        // Spawn al congelar tiempo (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            TrySpawnClone();
        }
    }

    public void TrySpawnClone()
    {
        if (currentClones >= maxClones)
        {
            Debug.Log("⚠️ Límite de clones alcanzado");
            return;
        }

        if (clonePrefab == null)
        {
            Debug.LogError("❌ No hay prefab de clon");
            return;
        }

        SpawnClone();
    }

    public void SpawnClone()
    {
        GameObject newClone = Instantiate(clonePrefab, player.position, player.rotation);
        currentClones++;

        Debug.Log($"✅ Clon creado ({currentClones}/{maxClones})");
    }

    public void CloneDestroyed()
    {
        currentClones--;
        Debug.Log($"🗑️ Clon destruido ({currentClones}/{maxClones})");
    }

}