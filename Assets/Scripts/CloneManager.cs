using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    [Header("Clone Settings")]
    public GameObject clonePrefab;
    public PlayerRecorder playerRecorder;
    public Transform player;

    [Header("Clone Limit")]
    public int maxClones = 3;

    [Header("Debug")]
    public List<CloneController> clones = new List<CloneController>();

    public void SpawnClone()
    {
        if (playerRecorder == null) return;

        // SOLUCIÓN: Verificar límite antes de crear
        if (clones.Count >= maxClones)
        {
            // Opción 1: Destruir el clon más antiguo (FIFO - First In First Out)
            DestroyOldestClone();

            // Opción 2: No crear y mostrar mensaje (comentar línea anterior y descomentar estas)
            // Debug.Log($"Límite de clones alcanzado ({maxClones})");
            // return;
        }

        // Crear el nuevo clon
        PlayerFrameData lastFrame = playerRecorder.GetLastFrame();
        GameObject cloneObj = Instantiate(clonePrefab, lastFrame.position, lastFrame.rotation);
        CloneController cloneCtrl = cloneObj.GetComponent<CloneController>();

        if (cloneCtrl != null)
        {
            cloneCtrl.playerRecorder = playerRecorder;
            cloneCtrl.playerTransform = player;
            clones.Add(cloneCtrl);
        }
    }

    private void DestroyOldestClone()
    {
        if (clones.Count > 0)
        {
            // Limpiar referencias nulas primero
            clones.RemoveAll(c => c == null);

            if (clones.Count > 0)
            {
                CloneController oldestClone = clones[0];
                clones.RemoveAt(0);
                Destroy(oldestClone.gameObject);
            }
        }
    }

    // Método para destruir todos los clones
    public void DestroyAllClones()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            if (clones[i] != null)
                Destroy(clones[i].gameObject);
        }
        clones.Clear();
    }

    // Método para obtener cantidad de clones activos
    public int GetActiveCloneCount()
    {
        clones.RemoveAll(c => c == null);
        return clones.Count;
    }

    // Método para verificar si se puede crear un clon
    public bool CanSpawnClone()
    {
        clones.RemoveAll(c => c == null);
        return clones.Count < maxClones;
    }

    private void Update()
    {
        // Limpiar referencias nulas automáticamente
        clones.RemoveAll(c => c == null);
    }
}