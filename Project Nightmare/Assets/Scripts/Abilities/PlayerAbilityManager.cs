using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerAbilityManager : LazySingleton<PlayerAbilityManager>
{
    private Dictionary<int, Type> abilities;

    private void InitialisePlayerAbilities()
    {
        abilities = new Dictionary<int, Type>();
        foreach (Type type in Assembly.GetAssembly(typeof(PlayerAbility)).GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PlayerAbility))))
        {
            abilities.Add(GetAbilityHandle(type), type);
        }
    }

    private PlayerAbility CreateAbility(int handle)
    {
        if (abilities == null)
        {
            Debug.LogError("PlayerAbilityManager is not initialised yet. Please wait.");
            return null;
        }

        if (abilities.Count <= handle)
        {
            Debug.LogError($"Ability Index {handle} it outside of range.");
            return null;
        }

        PlayerAbility ability = (PlayerAbility) Activator.CreateInstance(abilities[handle]);
        ability.Initialise();
        return ability;
    }

    public static int GetAbilityHandle(Type type)
    {
        return GetStableStringHashCode(type.FullName);
    }

    private static int GetStableStringHashCode(string str)
    {
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    protected override void Initialise()
    {
        InitialisePlayerAbilities();
    }
}