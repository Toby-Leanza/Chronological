using System.Collections.Generic;
using UnityEngine;

public class PosRecorder : MonoBehaviour
{
    public List<PosFrameData> recordedPosFrames = new List<PosFrameData>();
    public bool record = true;

    private Transform targetTransform;

    void Start()
    {
        targetTransform = transform; // Por defecto graba a sí mismo
        Debug.Log($"PosRecorder iniciado en: {gameObject.name}, Record: {record}");
    }

    void FixedUpdate()
    {
        if (!record || targetTransform == null || TimeControls.Instance != null && TimeControls.Instance.isFrozen)
        {
            Debug.Log($"PosRecorder no grabando - Record: {record}, Target: {targetTransform != null}");
            return;
        }

        // Grabar posición y rotación
        PosFrameData currentPosFrame = new PosFrameData(
            targetTransform.position,
            targetTransform.rotation
        );

        recordedPosFrames.Add(currentPosFrame);
        Debug.Log($"PosFrame grabado #{recordedPosFrames.Count} - Pos: {targetTransform.position}");
    }
}