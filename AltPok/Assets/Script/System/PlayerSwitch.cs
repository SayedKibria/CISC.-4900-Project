using UnityEngine;

public class PlayerSwitcher : MonoBehaviour{
    [Header("Player Switching")]
    [SerializeField] private GameObject playerHuman;
    [SerializeField] private PokemonParty party; // Reference to your party manager
    [SerializeField] private HealthBarUI healthBarUI;


    private GameObject currentControlled;
    private GameObject currentPokemonInstance;

    void Start(){
        currentControlled = playerHuman;
    }

    void Update(){
        for (int i = 0; i < party.team.Count && i < 6; i++){
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)){
                SwitchToPokemon(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.H)){
            SwitchToHuman();
        }
    }

    void SwitchToPokemon(int index){
        GameObject pokemonPrefab = party.GetPokemon(index);

        if (pokemonPrefab == null){
            Debug.LogWarning("No Pokémon assigned in that team slot.");
            return;
        }

        if (currentPokemonInstance != null){
            Destroy(currentPokemonInstance);
        }

        Vector3 spawnPos = playerHuman.transform.position;
        currentPokemonInstance = Instantiate(pokemonPrefab, spawnPos, Quaternion.identity);

        // Disable human control
        var humanControl = playerHuman.GetComponent<IControllable>();
        if (humanControl != null) humanControl.DisableControl();

        // Enable Pokémon control
        var pokeControl = currentPokemonInstance.GetComponent<IControllable>();
        if (pokeControl != null) pokeControl.EnableControl();
        else Debug.LogError("Pokémon prefab is missing IControllable script.");

        currentControlled = currentPokemonInstance;

        var health = currentPokemonInstance.GetComponent<Health>();
        if (health != null){
            BattleUIManager.Instance.RegisterCreature(health, false); // false = player
        }


    }

    void SwitchToHuman(){
        if (currentPokemonInstance != null){
            playerHuman.transform.position = currentPokemonInstance.transform.position;
            Destroy(currentPokemonInstance);
            currentPokemonInstance = null;
        }

        var humanControl = playerHuman.GetComponent<IControllable>();
        if (humanControl != null) humanControl.EnableControl();
        
        healthBarUI.SetTarget(null);

        currentControlled = playerHuman;
    }
}