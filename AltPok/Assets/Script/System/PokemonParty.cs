using UnityEngine;
using System.Collections.Generic;

public class PokemonParty : MonoBehaviour
{
    public List<GameObject> team = new List<GameObject>();

    // Store HP for each Pokémon
    public int[] creatureCurrentHP = new int[6];
    public int[] creatureMaxHP = new int[6];

    public int MaxTeamSize => 6;

    void Awake()
    {
        // -1 means HP not initialized yet
        for (int i = 0; i < 6; i++)
        {
            creatureCurrentHP[i] = -1;
            creatureMaxHP[i] = -1;
        }
    }

    public GameObject GetPokemon(int index)
    {
        if (index >= 0 && index < team.Count)
            return team[index];
        return null;
    }
}