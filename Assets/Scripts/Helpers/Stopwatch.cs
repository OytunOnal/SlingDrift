
using UnityEngine;

public class Stopwatch
{
    private float startTime;
    private float stopTime;
    private bool isRunning = false;

    public void StartStopwatch()
    {
        if (!isRunning)
        {
            startTime = Time.time;
            isRunning = true;
        }
    }

    public float StopStopwatch()
    {
        if (isRunning)
        {
            stopTime = Time.time;
            isRunning = false;
            return stopTime - startTime;
        }

        return 0f; // Stopwatch wasn't running, return 0 as the elapsed time
    }

    public void ResetStopwatch()
    {
        startTime = 0f;
        stopTime = 0f;
        isRunning = false;
    }
}
