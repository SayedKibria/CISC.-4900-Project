using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("Player Switching")]
    [SerializeField] private GameObject playerHuman;
    [SerializeField] private PokemonParty party;
    [SerializeField] private HealthBarUI healthBarUI;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 1f;

    private GameObject currentControlled;
    private GameObject currentPokemonInstance;
    public bool canSummonPokemon = false;

    void Start() => currentControlled = playerHuman;

    void Update()
    {
        if (!canSummonPokemon) return;

        for (int i = 0; i < party.team.Count && i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SwitchToPokemon(i);
        }

        if (Input.GetKeyDown(KeyCode.H))
            SwitchToHuman();
    }

    void SwitchToPokemon(int index)
    {
        GameObject pokemonPrefab = party.GetPokemon(index);
        if (pokemonPrefab == null) return;

        if (currentPokemonInstance != null)
            Destroy(currentPokemonInstance);

        Vector3 spawnPos = playerHuman.transform.position + Vector3.up * spawnDistance;
        currentPokemonInstance = Instantiate(pokemonPrefab, spawnPos, Quaternion.identity);

        playerHuman.GetComponent<IControllable>()?.DisableControl();
        currentPokemonInstance.GetComponent<IControllable>()?.EnableControl();

        currentControlled = currentPokemonInstance;

        var health = currentPokemonInstance.GetComponent<Health>();
        if (health != null)
            BattleUIManager.Instance.RegisterCreature(health, false);

        // Initialize stats
        var control = currentPokemonInstance.GetComponent<CreatureControl>();
        if (control != null && control.enabled)
        {
            var data = control.GetComponent<CreatureControl>();
            var knockback = currentPokemonInstance.GetComponent<KnockbackHandler>();
            if (data != null && knockback != null)
                knockback.Initialize(data.GetComponent<CreatureControl>().enabled ? 5f : 0f, 0.2f);
        }
    }

    void SwitchToHuman()
    {
        if (currentPokemonInstance != null)
        {
            playerHuman.transform.position = currentPokemonInstance.transform.position;
            Destroy(currentPokemonInstance);
            currentPokemonInstance = null;
        }

        playerHuman.GetComponent<IControllable>()?.EnableControl();
        healthBarUI.SetTarget(null);
        currentControlled = playerHuman;
    }
}