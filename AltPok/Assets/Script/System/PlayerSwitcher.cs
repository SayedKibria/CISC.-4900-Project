using UnityEngine;
using System.Collections;

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

    private bool[] deadCreatures = new bool[6];
    public bool canSummonPokemon = false;

    // Cooldown System
    [Header("Switch Cooldown")]
    public float switchCooldown = 1.5f;
    private bool isSwitching = false; // lock while switching

    void Start()
    {
        currentControlled = playerHuman;
    }

    void Update()
    {
        if (!canSummonPokemon || isSwitching)
            return;

        for (int i = 0; i < party.team.Count && i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // ignore dead creatures without cooldown
                if (deadCreatures[i])
                {
                    Debug.Log($"Pokemon {i} is dead. Cannot switch to it.");
                    continue; // skip Without starting cooldown
                }

                // Only valid switches trigger cooldown
                StartCoroutine(SwitchCooldownRoutine(i));
                return;
            }
        }
    }
    
    // Auto Summon System
    public void TriggerAutoSummon(float delay = 1.5f)
    {
        StartCoroutine(AutoSummonRoutine(delay));
    }

    private System.Collections.IEnumerator AutoSummonRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Summon Pokémon 0 Only if none is out
        if (currentPokemonInstance == null)
            SwitchToPokemon(0);

        // unlock manual switching
        canSummonPokemon = true;
    }

    // Handles cooldown lock
    private System.Collections.IEnumerator SwitchCooldownRoutine(int index)
    {
        isSwitching = true;

        SwitchToPokemon(index);

        // Wait for cooldown duration
        yield return new WaitForSeconds(switchCooldown);

        isSwitching = false;
    }

    private void SwitchToPokemon(int index)
    {
        // Prevent summoning dead Pokémon
        if (deadCreatures[index])
        {
            Debug.Log($"Pokemon {index} has fainted and cannot be summoned.");
            return;
        }

        GameObject prefab = party.GetPokemon(index);
        if (prefab == null) return;

        // Destroy previously summoned Pokémon
        if (currentPokemonInstance != null)
            Destroy(currentPokemonInstance);

        // Spawn new Pokémon next to the player
        Vector3 spawnPos = playerHuman.transform.position + Vector3.left * spawnDistance;
        currentPokemonInstance = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Transfer control
        playerHuman.GetComponent<IControllable>()?.DisableControl();
        currentPokemonInstance.GetComponent<IControllable>()?.EnableControl();

        currentControlled = currentPokemonInstance;

        // Handle health load/restore
        var health = currentPokemonInstance.GetComponent<Health>();
        if (health != null)
        {
            if (party.creatureCurrentHP[index] == -1)
            {
                party.creatureMaxHP[index] = health.GetMaxHealth();
                party.creatureCurrentHP[index] = party.creatureMaxHP[index];
            }

            // Restore saved HP
            health.Initialize(party.creatureMaxHP[index]);
            health.SetCurrentHealth(party.creatureCurrentHP[index]);

            // Save HP changes
            health.OnHealthChanged.AddListener(value =>
            {
                party.creatureCurrentHP[index] =
                    Mathf.RoundToInt(value * party.creatureMaxHP[index]);
            });

            // Handle death
            health.OnDeath.AddListener(() => HandlePokemonDeath(index));

            // Register UI
            BattleUIManager.Instance?.RegisterCreature(health, false);
        }
    }

    private void HandlePokemonDeath(int index)
    {
        deadCreatures[index] = true;

        if (currentPokemonInstance != null)
            Destroy(currentPokemonInstance);

        // Auto summon next after delay
        StartCoroutine(SummonDelayedNext());
    }
    private IEnumerator SummonDelayedNext()
    {
        // Wait 0.5 seconds before summoning the next creature
        yield return new WaitForSeconds(0.5f);

        // Find next available Pokémon
        for (int i = 0; i < party.team.Count; i++)
        {
            if (!deadCreatures[i])
            {
                Debug.Log($"Player auto-summoning next Pokémon index {i}");
                SwitchToPokemon(i);
                yield break;
            }
        }

        Debug.Log("Player has no remaining Pokémon!");
    }
}