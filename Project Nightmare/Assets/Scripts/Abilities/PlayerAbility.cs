using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public abstract class PlayerAbility
{
    protected string AbilityName;
    protected float AbilityCooldown;
    protected float AbilityDuration;
    public bool IsReady { get; protected set; }

    protected GlobalTimerManager.TimerHandle timerHandle;
    protected GlobalTimerManager.TimerHandle durationHandle;

    protected NightmareCharacterController controller;
    protected Player player;

    //Abstract functions
    protected abstract void AbilityActivated(Transform target);
    protected abstract void AbilityFinished();
    protected abstract void AbilityReady();


    public void SetCotroller(NightmareCharacterController controller)
    {
        this.controller = controller;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    /// <summary>
    /// Activates the ability.
    /// </summary>
    /// <param name="target">Target of the ability</param>
    public void Activate(Transform target = null)
    {
        if (!IsReady) return;

        durationHandle = GlobalTimerManager.Instance.CreateTimer(AbilityDuration, OnAbilityFinished);
        IsReady = false;

        AbilityActivated(target);
    }

    private void OnAbilityFinished()
    {
        timerHandle = GlobalTimerManager.Instance.CreateTimer(AbilityCooldown, OnCooldownComplete);
        AbilityFinished();
    }

    /// <summary>
    /// Resets the abilities cooldown and makes it immediately reusable.
    /// </summary>
    public void ResetCooldown()
    {
        if (timerHandle != null && timerHandle.IsValid) GlobalTimerManager.Instance.ResetTimer(timerHandle);
        IsReady = true;
        AbilityReady();
    }

    /// <summary>
    /// Called when the abilities cooldown finishes.
    /// </summary>
    private void OnCooldownComplete()
    {
        IsReady = true;
        AbilityReady();
    }

    /// <summary>
    /// Initialise the ability object.
    /// </summary>
    public virtual void Initialise()
    {
        IsReady = true;
    }
}