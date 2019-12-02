using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public class DashAbility : PlayerAbility
{
    public float DashSpeed;

    protected override void AbilityActivated(Transform target)
    {
        controller.SetVelocityControlOverride(VelocityOverride);
    }

    protected override void AbilityFinished()
    {
        controller.ClearVelocityControlOverride();
    }

    protected override void AbilityReady()
    {
    }

    private Vector3 VelocityOverride(Vector3 forward, Vector3 up, Vector3 currentVel)
    {
        return currentVel.normalized * DashSpeed;
    }

    public override void Initialise()
    {
        base.Initialise();
        AbilityName = "Dash";
        AbilityCooldown = 1.0f;
        AbilityDuration = 0.2f;
        DashSpeed = 50;
    }
}