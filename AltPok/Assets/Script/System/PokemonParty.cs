using UnityEngine;
using System.Collections.Generic;

public class PokemonParty : MonoBehaviour{
    public List<GameObject> team = new List<GameObject>(); // Holds 6 Pokémon prefabs

    public int MaxTeamSize => 6;

    public void AddToTzeam(GameObject newPokemon){
        if (team.Count < MaxTeamSize){
            team.Add(newPokemon);
        }
        else{
            Debug.LogWarning("Team is full! Replace a Pokémon instead.");
        }
    }

    public void ReplacePokemon(int index, GameObject newPokemon){
        if (index >= 0 && index < MaxTeamSize){
            team[index] = newPokemon;
        }
        else{
            Debug.LogWarning("Invalid team index.");
        }
    }

    public GameObject GetPokemon(int index){
        if (index >= 0 && index < team.Count){
            return team[index];
        }
        return null;
    }
}