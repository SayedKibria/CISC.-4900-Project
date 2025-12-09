using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PokemonParty))]
public class NPCSummoner : MonoBehaviour
{
    [Header("NPC Creature Spawn Settings")]
    [SerializeField] private float spawnDistance = 1f; // distance in front of NPC
    public int defaultCreatureIndex = 0;
    public float summonDelay = 3f;

    private PokemonParty npcParty;
    private GameObject currentCreature;
    private bool[] deadCreatures = new bool[6];

    void Awake()
    {
        npcParty = GetComponent<PokemonParty>();
    }

    public void TriggerSummon()
    {
        StartCoroutine(SummonAfterDelay());
    }

    private IEnumerator SummonAfterDelay()
    {
        yield return new WaitForSeconds(summonDelay);
        SummonCreature(defaultCreatureIndex);
    }

    public GameObject SummonCreature(int index = -1)
    {
        if (npcParty == null || npcParty.team.Count == 0) return null;

        if (index < 0 || index >= npcParty.team.Count)
            index = defaultCreatureIndex;

        if (deadCreatures[index])
        {
            Debug.LogWarning($"NPC Pokémon {index} is dead and cannot be resummoned.");
            return null;
        }

        GameObject prefab = npcParty.GetPokemon(index);
        if (prefab == null) return null;

        if (currentCreature != null)
            Destroy(currentCreature);

        // Spawn in front of NPC based on its facing direction
        Vector3 spawnOffset = Vector3.right * spawnDistance; // change direction
        Vector3 spawnPos = transform.position + spawnOffset;

        currentCreature = Instantiate(prefab, spawnPos, Quaternion.identity);

        var controllable = currentCreature.GetComponent<IControllable>();
        controllable?.EnableControl();

        var health = currentCreature.GetComponent<Health>();
        if (health != null)
        {
            if (npcParty.creatureCurrentHP[index] == -1)
            {
                npcParty.creatureMaxHP[index] = health.GetMaxHealth();
                npcParty.creatureCurrentHP[index] = npcParty.creatureMaxHP[index];
            }

            health.Initialize(npcParty.creatureMaxHP[index]);
            health.SetCurrentHealth(npcParty.creatureCurrentHP[index]);

            health.OnHealthChanged.AddListener(value =>
            {
                npcParty.creatureCurrentHP[index] =
                    Mathf.RoundToInt(value * npcParty.creatureMaxHP[index]);
            });

            health.OnDeath.AddListener(() => HandleDeath(index));

            BattleUIManager.Instance?.RegisterCreature(health, true);
        }

        return currentCreature;
    }

    private void HandleDeath(int index)
    {
        deadCreatures[index] = true;
        currentCreature = null;

        Debug.Log($"NPC Pokémon {index} fainted.");

        // Wait 0.5s, then summon the next one
        StartCoroutine(SummonDelayedNext());
    }

    private void SummonNextAvailable()
    {
        for (int i = 0; i < npcParty.team.Count; i++)
        {
            if (!deadCreatures[i])
            {
                Debug.Log($"NPC summoning next Pokémon index {i}");
                SummonCreature(i);
                return;
            }
        }

        Debug.Log("NPC has no remaining Pokémon!");
    }

    private IEnumerator SummonDelayedNext()
    {
        // Delay before summoning replacement
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < npcParty.team.Count; i++)
        {
            if (!deadCreatures[i])
            {
                Debug.Log($"NPC summoning next Pokémon index {i}");
                SummonCreature(i);
                yield break;
            }
        }

        Debug.Log("NPC has no remaining Pokémon!");
    }

    public GameObject GetCurrentCreature() => currentCreature;
}