using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;
    public float moveSpeed;
    private int frameIndex;

    void Start()
    {
        if (playerMovement != null)
        {
            moveSpeed = playerMovement.speed;
        }

        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            frameIndex = playerRecorder.recordedFrames.Count - 1;
            transform.position = playerRecorder.recordedFrames[frameIndex].position;
            transform.rotation = playerRecorder.recordedFrames[frameIndex].rotation;
        }
    }

    void Update()
    {
        if (playerRecorder == null || playerRecorder.recordedFrames.Count == 0) return;

        if (TimeControls.Instance.isFrozen)
        {
            if (TimeControls.Instance.isRewinding)
                frameIndex = Mathf.Max(0, frameIndex - 1);
            else if (TimeControls.Instance.isForwarding)
                frameIndex = Mathf.Min(playerRecorder.recordedFrames.Count - 1, frameIndex + 1);

            transform.position = playerRecorder.recordedFrames[frameIndex].position;
            transform.rotation = playerRecorder.recordedFrames[frameIndex].rotation;
        }
        else
        {
            // El clon solo se mueve hacia atrás en el tiempo cuando está descongelado
            // Se detiene cuando llega al frame 0
            if (frameIndex > 0)
            {
                frameIndex++;
                transform.position = playerRecorder.recordedFrames[frameIndex].position;
                transform.rotation = playerRecorder.recordedFrames[frameIndex].rotation;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !TimeControls.Instance.isFrozen)
            Destroy(gameObject);
    }
}