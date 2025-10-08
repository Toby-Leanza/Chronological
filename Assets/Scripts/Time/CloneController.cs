using UnityEngine;

public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;
    public float moveSpeed;
    public float destroyDistance = 5f;

    private int frameIndex;

    void Start()
    {
        if (playerMovement != null)
            moveSpeed = playerMovement.speed;

        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            frameIndex = playerRecorder.recordedFrames.Count - 1;
            ApplyFrame(frameIndex);
        }
    }

    void Update()
    {
        if (playerRecorder == null || playerRecorder.recordedFrames.Count == 0) return;

        if (TimeControls.Instance.isFrozen)
        {
            if (TimeControls.Instance.isRewinding)
                frameIndex--;
            else if (TimeControls.Instance.isForwarding)
                frameIndex++;

            frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
            ApplyFrame(frameIndex);
        }
        else
        {
            if (frameIndex < playerRecorder.recordedFrames.Count - 1)
            {
                frameIndex++;
                ApplyFrame(frameIndex);
            }

            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= destroyDistance)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private void ApplyFrame(int index)
    {
        transform.position = playerRecorder.recordedFrames[index].position;
        transform.rotation = playerRecorder.recordedFrames[index].rotation;
    }
}
