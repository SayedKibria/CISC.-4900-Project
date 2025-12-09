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
        // Move NPC to battle position
        if (npc != null && npcBattlePosition != null)
        {
            npc.transform.position = npcBattlePosition.position;

            // Make NPC face right
            Animator npcAnimator = npc.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetFloat("moveX", 1f); // Right
                npcAnimator.SetFloat("moveY", 0f); // No vertical movement
            }
        }

        if (player != null && playerBattlePosition != null)
            player.transform.position = playerBattlePosition.position;

        // Disable player control
        var playerControl = player.GetComponent<IControllable>();
        if (playerControl != null)
        {
            playerControl.DisableControl();

            // Force face LEFT
            var movement = player.GetComponent<PlayerControl>();
            if (movement != null)
                movement.ForceFaceDirection(new Vector2(-1f, 0f));

            Debug.Log("Player movement disabled and forced to face left.");
        }

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
            switcher.canSummonPokemon = false;
            Debug.Log("Can summon Pokémon enabled!");
            switcher.TriggerAutoSummon(3f); // auto summon after 3 sec
        }
        else
        {
            Debug.LogError("PlayerSwitcher not found on GameSystem!");
        }

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