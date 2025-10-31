using UnityEngine;

public class GolpesCajas : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip golpesClip;

    private float[] golpesInicio = { 0.2f, 1.0f, 1.9f, 2.9f, 4.0f, 5.0f, 5.7f, 7.4f, 8.3f, 9.0f, 10.1f, 10.7f, 11.4f, 12.3f };
    private float duracionGolpe = 0.5f;

    void Start()
    {
        audioSource.clip = golpesClip;
        audioSource.playOnAwake = false;
    }

    public void ReproducirGolpe()
    {
        int i = Random.Range(0, golpesInicio.Length);
        float startTime = golpesInicio[i];
        
        audioSource.time = startTime;
        audioSource.Play();

        Invoke(nameof(DetenerGolpe), duracionGolpe);
    }

    void DetenerGolpe()
    {
        audioSource.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 0.3f)
        {
            ReproducirGolpe();
        }
    }

}