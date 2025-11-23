using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PokemonParty))]
public class NPCSummoner : MonoBehaviour
{
    [Header("NPC Creature Spawn Settings")]
    public Transform spawnPoint;
    public int defaultCreatureIndex = 0;
    public float summonDelay = 3f; // Delay before summoning

    private PokemonParty npcParty;
    private GameObject currentCreature;

    void Awake()
    {
        npcParty = GetComponent<PokemonParty>();
        if (npcParty == null)
            Debug.LogError($"{gameObject.name} has no PokemonParty assigned!");
    }

    // Public method to trigger delayed summon
    public void TriggerSummon()
    {
        StartCoroutine(SummonAfterDelay());
    }

    private IEnumerator SummonAfterDelay()
    {
        yield return new WaitForSeconds(summonDelay);
        SummonCreature(defaultCreatureIndex);
        Debug.Log($"{gameObject.name} has summoned their creature!");
    }

    public GameObject SummonCreature(int index = -1)
    {
        if (npcParty == null || npcParty.team.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no creatures in its party!");
            return null;
        }

        if (index < 0 || index >= npcParty.team.Count)
            index = defaultCreatureIndex;

        GameObject creaturePrefab = npcParty.GetPokemon(index);
        if (creaturePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}'s party slot {index} is empty!");
            return null;
        }

        if (currentCreature != null)
            Destroy(currentCreature);

        currentCreature = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);

        var controllable = currentCreature.GetComponent<IControllable>();
        if (controllable != null)
            controllable.EnableControl();

        var health = currentCreature.GetComponent<Health>();
        if (health != null && BattleUIManager.Instance != null)
            BattleUIManager.Instance.RegisterCreature(health, true);

        return currentCreature;
    }

    public GameObject GetCurrentCreature() => currentCreature;
}