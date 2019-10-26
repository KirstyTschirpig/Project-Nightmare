using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility
{
    protected string AbilityName;
    protected float AbilityCooldown;

    /// <summary>
    /// Activates the ability.
    /// </summary>
    /// <param name="target">Target of the ability</param>
    public abstract void Activate(Transform target);

    /// <summary>
    /// Resets the abilities cooldown and makes it immediately reusable.
    /// </summary>
    public abstract void ResetCooldown();

    /// <summary>
    /// Called when the abilities cooldown finishes.
    /// </summary>
    public abstract void OnCooldownComplete();

    /// <summary>
    /// Initialise the ability object.
    /// </summary>
    public abstract void Initialise();
}
