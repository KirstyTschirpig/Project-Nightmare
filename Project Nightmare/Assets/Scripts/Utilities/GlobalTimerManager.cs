using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalTimerManager : LazySingleton<GlobalTimerManager>
{
    public delegate void OnTimerComplete();

    private class Timer
    {
        public float remaining;
        public float length;
        public bool isPaused;
        public bool isRepeating;

        public OnTimerComplete onTimerComplete;
    }

    public class TimerHandle
    {
        public uint Key;
        public bool IsValid;

        public TimerHandle(uint key)
        {
            Key = key;
            IsValid = true;
        }

        public static implicit operator uint(TimerHandle handle) => handle.Key;
    }

    private Dictionary<uint, Timer> timers;
    private List<uint> completedTimers;

    private List<TimerHandle> timerHandles;

    private uint timerKey = 0;

    protected override void Initialise()
    {
        timers = new Dictionary<uint, Timer>();
        completedTimers = new List<uint>();
        timerHandles = new List<TimerHandle>();
    }

    void Update()
    {
        completedTimers.Clear();

        float deltaTime = Time.deltaTime;
        foreach (var kvp in timers)
        {
            Timer t = kvp.Value;
            if (t.isPaused) continue;

            t.remaining -= deltaTime;
            if (!(t.remaining <= 0)) continue;

            if (t.isRepeating) t.remaining = t.length;
            completedTimers.Add(kvp.Key);
        }

        foreach (var key in completedTimers)
        {
            Timer timer = timers[key];

            if (!timer.isRepeating)
            {
                //Invalidate any handles.
                timers.Remove(key);
                ClearTimerHandles(key);
            }

            timer.onTimerComplete?.Invoke();
        }
    }

    public void ResetTimer(uint handle)
    {
        if (!timers.ContainsKey(handle))
        {
            Debug.LogError($"Warning: Timer with handle {handle} has already ended.");
            return;
        }

        timers[handle].remaining = timers[handle].length;
    }

    public TimerHandle CreateTimer(float length, OnTimerComplete onTimerComplete, bool repeating = false)
    {
        uint key;
        //Unchecked since eventual overflow is deliberate.
        unchecked
        {
            key = timerKey++;
        }

        Timer timer = new Timer
        {
            isPaused = false,
            isRepeating = repeating,
            length = length,
            remaining = length,
            onTimerComplete = onTimerComplete
        };

        timers.Add(key, timer);

        TimerHandle handle = new TimerHandle(key);
        timerHandles.Add(handle);
        return handle;
    }

    public float GetRemainingTime(uint handle)
    {
        if (!timers.ContainsKey(handle))
        {
            Debug.LogError($"Warning: Timer with handle {handle} has already ended.");
            return -1;
        }

        return timers[handle].remaining;
    }

    public bool IsTimerPaused(uint handle)
    {
        if (!timers.ContainsKey(handle))
        {
            Debug.LogError($"Warning: Timer with handle {handle} has already ended.");
            return false;
        }

        return timers[handle].isPaused;
    }

    public void PauseTimer(uint handle, bool pause = true)
    {
        if (!timers.ContainsKey(handle))
        {
            Debug.LogError($"Warning: Timer with handle {handle} has already ended.");
        }

        timers[handle].isPaused = pause;
    }

    public void DestroyTimer(uint handle)
    {
        if (!timers.ContainsKey(handle))
        {
            Debug.LogError($"Warning: Timer with handle {handle} has already ended and was not marked as repeating.");
        }

        timers.Remove(handle);
        ClearTimerHandles(handle);
    }

    private void ClearTimerHandles(uint key)
    {
        foreach (TimerHandle handle in timerHandles.Where(t => t.Key == key)) handle.IsValid = false;
        timerHandles.RemoveAll(t => t.Key == key);
    }
}