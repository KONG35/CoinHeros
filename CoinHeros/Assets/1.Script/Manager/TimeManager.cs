using UnityEngine;

public class TimeManager
{
    private static TimeManager instance;

    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
                instance = new TimeManager();

            return instance;
        }
    }
    private float saveTimeScale;
    public void Stop()
    {
        Time.timeScale = 0f;
    }
    public void Play()
    {
        Time.timeScale = saveTimeScale;
    }
    public void ChangeTimeScale(float scale)
    {
        saveTimeScale = scale;
        Play();
    }
}
