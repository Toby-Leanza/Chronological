using System.Collections.Generic;
using UnityEngine;

public class KeyRecorder : MonoBehaviour
{
    public List<KeyFrameData> recordedKeyFrames = new List<KeyFrameData>();
    public bool record = true;

    public void ClearRecording()
    {
        recordedKeyFrames.Clear();
    }

    public KeyFrameData GetKeyFrameAt(int index)
    {
        if (index >= 0 && index < recordedKeyFrames.Count)
            return recordedKeyFrames[index];
        return null;
    }

    public int GetFrameCount()
    {
        return recordedKeyFrames.Count;
    }
}