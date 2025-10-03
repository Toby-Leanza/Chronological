using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    public List<PlayerFrameData> recordedFrames = new List<PlayerFrameData>();
    public bool record = true;

    void Update()
    {
        if (record)
        {
            recordedFrames.Add(new PlayerFrameData(transform.position, transform.rotation));
        }
    }

    public List<PlayerFrameData> GetRecordedFrames()
    {
        return recordedFrames;
    }

    public PlayerFrameData GetLastFrame()
    {
        if (recordedFrames.Count == 0) return new PlayerFrameData(transform.position, transform.rotation);
        return recordedFrames[recordedFrames.Count - 1];
    }
}

[System.Serializable]
public class PlayerFrameData
{
    public Vector3 position;
    public Quaternion rotation;

    public PlayerFrameData(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}
