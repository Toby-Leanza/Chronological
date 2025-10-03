using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    public GameObject clonePrefab;
    public PlayerRecorder playerRecorder;
    public Transform player;

    public List<CloneController> clones = new List<CloneController>();

    public void SpawnClone()
    {
        if (playerRecorder == null) return;

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
}
