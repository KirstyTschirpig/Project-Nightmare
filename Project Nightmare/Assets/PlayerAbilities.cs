using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerAbility ability;
    private NightmareCharacterController controller;

    private void Start()
    {
       ability = PlayerAbilityManager.Instance.CreateAbility(PlayerAbilityManager.GetAbilityHandle(typeof(DashAbility)));
       controller = GetComponent<Player>().nightmareCharacter;
       ability.SetCotroller(controller);
       ability.SetPlayer(GetComponent<Player>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ability.Activate(null);
        }
    }
}
