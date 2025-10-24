using UnityEngine;

[System.Serializable]
public class KeyFrameData
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool jump;
    public bool interact;
    public Quaternion movementRotation;

    public KeyFrameData(bool up, bool down, bool left, bool right, bool jump, bool interact, Quaternion rotation)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.jump = jump;
        this.interact = interact;
        this.movementRotation = rotation;
    }

    public Vector2 GetMovementVector()
    {
        Vector2 input = Vector2.zero;
        if (up) input.y += 1;
        if (down) input.y -= 1;
        if (right) input.x += 1;
        if (left) input.x -= 1;
        return input.normalized;
    }

    public override string ToString()
    {
        return $"KeyFrame[move:({GetMovementVector()}), jump:{jump}, interact:{interact}]";
    }
}