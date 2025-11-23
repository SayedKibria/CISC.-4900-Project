using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [Header("Battle Positions")]
    [SerializeField] private Transform playerBattlePosition;
    [SerializeField] private Transform npcBattlePosition;

    [Header("NPC & Game System Reference")]
    [SerializeField] private GameObject npc;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameSystem;
    [SerializeField] private Transform battleCameraTarget;

    [Header("Optional: Disable Trigger After Activation")]
    [SerializeField] private bool disableAfterTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger when player enters
        if (!collision.CompareTag("Player")) return;

        SetupBattle();
    }

    private void SetupBattle()
    {
        // Move NPC and Player to battle positions
        if (npc != null && npcBattlePosition != null)
            npc.transform.position = npcBattlePosition.position;

        if (player != null && playerBattlePosition != null)
            player.transform.position = playerBattlePosition.position;

        // Move camera to battle arena & zoom out
        var camFollow = Camera.main.GetComponent<CameraFollow>();
        if (camFollow != null && battleCameraTarget != null)
        {
            camFollow.LockTo(battleCameraTarget.position);
        }


        // Unlock Pokémon summoning in PlayerSwitcher
        var switcher = gameSystem.GetComponent<PlayerSwitcher>();
        if (switcher != null)
        {
            switcher.canSummonPokemon = true;
            Debug.Log("Can summon Pokémon enabled!");
        }
        else
        {
            Debug.LogError("PlayerSwitcher not found on GameSystem!");
        }

        // Inside BattleTrigger -> SetupBattle()
        var npcSummoner = npc.GetComponent<NPCSummoner>();
        if (npcSummoner != null)
        {
            npcSummoner.TriggerSummon(); // Starts the delayed summon
            Debug.Log("NPCSummoner has been triggered to summon!");
        }

        if (disableAfterTrigger)
            gameObject.SetActive(false);

        Debug.Log("Battle setup completed!");
    }
}