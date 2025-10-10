using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    [Header("Clone Settings")]
    public GameObject clonePrefab;
    public PlayerRecorder playerRecorder;
    public Transform player;
    public PlayerMovement playerMovement;

    [Header("Clone Limit")]
    public int maxClones = 3;

    [Header("Debug")]
    public List<CloneController> clones = new List<CloneController>();

    void Start()
    {
        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void SpawnClone()
    {
        if (playerRecorder == null || player == null) return;

        if (clones.Count >= maxClones)
        {
            DestroyOldestClone();
        }

        GameObject cloneObj = Instantiate(clonePrefab, player.position, player.rotation);
        CloneController cloneCtrl = cloneObj.GetComponent<CloneController>();

        if (cloneCtrl != null)
        {
            cloneCtrl.playerRecorder = playerRecorder;
            cloneCtrl.playerMovement = playerMovement;
            cloneCtrl.playerTransform = player;
            cloneCtrl.SetInitialState();
            clones.Add(cloneCtrl);
        }
    }

    private void DestroyOldestClone()
    {
        if (clones.Count > 0)
        {
            CloneController oldest = clones[0];
            clones.RemoveAt(0);
            Destroy(oldest.gameObject);
        }
    }

    public void DestroyAllClones()
    {
        foreach (var clone in clones)
        {
            if (clone != null)
                Destroy(clone.gameObject);
        }
        clones.Clear();
    }

    void Update()
    {
        clones.RemoveAll(c => c == null);
    }
}
