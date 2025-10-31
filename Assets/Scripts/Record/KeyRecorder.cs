using System.Collections.Generic;
using UnityEngine;

public class KeyRecorder : MonoBehaviour
{
    public List<KeyFrameData> recordedKeyFrames = new List<KeyFrameData>();
    public bool record = true;

    void FixedUpdate()
    {
        if (!record || TimeControls.Instance.isFrozen) return;

        // ✅ KEYRECORDER SE ENCARGA DE GRABAR TODOS LOS INPUTS
        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool jump = Input.GetKey(KeyCode.Space);
        bool interact = Input.GetKey(KeyCode.E);

        // Obtener rotación de cámara (si está disponible)
        Quaternion cameraRotation = Quaternion.identity;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraRotation = mainCamera.transform.rotation;
        }

        // Crear y grabar frame
        KeyFrameData currentFrame = new KeyFrameData(up, down, left, right, jump, interact, cameraRotation);
        recordedKeyFrames.Add(currentFrame);

        // Debug opcional
        if (recordedKeyFrames.Count % 60 == 0) // Cada ~1 segundo a 60 FPS
        {
            Debug.Log($"📊 KeyRecorder: {recordedKeyFrames.Count} frames grabados");
        }
    }

    public void SetRecording(bool shouldRecord)
    {
        record = shouldRecord;
        Debug.Log(record ? "🔴 KeyRecorder grabando..." : "⏸️ KeyRecorder pausado");
    }
}