using UnityEngine;

[System.Serializable]
public struct PosFrameData
{
    public Vector3 position;
    public Quaternion rotation;

    public PosFrameData(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }

    // Método para crear nuevo frame con posición modificada
    public PosFrameData WithPosition(Vector3 newPosition)
    {
        return new PosFrameData(newPosition, rotation);
    }

    // Método para crear nuevo frame con rotación modificada
    public PosFrameData WithRotation(Quaternion newRotation)
    {
        return new PosFrameData(position, newRotation);
    }

    public override string ToString()
    {
        return $"PosFrame[pos:{position}, rot:{rotation.eulerAngles.y:F1}°]";
    }
}