using UnityEngine;
using Chronos;

public class TimeManager : MonoBehaviour
{
    public Clock globalClock;

    public float rewindSpeed = -1f;
    public float forwardSpeed = 2f;
    public float normalSpeed = 1f;

    private float lastSpeed;
    private bool isFrozen = false;

    void Start()
    {
        if (globalClock != null)
        {
            lastSpeed = normalSpeed;
            globalClock.localTimeScale = normalSpeed;
        }
    }

    void Update()
    {
        if (globalClock == null) return;

        // Toggle congelar/descongelar con F
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFrozen = !isFrozen;
            lastSpeed = globalClock.localTimeScale;
            globalClock.localTimeScale = isFrozen ? 0f : normalSpeed;
            Debug.Log(isFrozen ? "¡Congelado!" : "¡Descongelado!");
        }

        // Rebobinar con R o avanzar con T
        if (Input.GetKey(KeyCode.R) && isFrozen)
        {
            globalClock.localTimeScale = rewindSpeed;
        }
        else if (Input.GetKey(KeyCode.T) && isFrozen)
        {
            globalClock.localTimeScale = forwardSpeed;
        }
        else
        {
            // Si no se mantiene ninguna tecla, restauramos la velocidad
            globalClock.localTimeScale = isFrozen ? 0f : normalSpeed;
        }
    }
}
