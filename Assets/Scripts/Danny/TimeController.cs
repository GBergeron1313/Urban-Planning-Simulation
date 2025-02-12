using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    public float currentTime = 0; // In hours (e.g., 8.5 = 8:30 AM)
    public float timeScale = 1.0f; // Speed of time (1.0 = real-time)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime * timeScale / 3600; // Convert seconds to hours
        if (currentTime >= 24) currentTime -= 24; // Reset after 24 hours
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}