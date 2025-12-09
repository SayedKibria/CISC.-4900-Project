using UnityEngine;
using System.Collections.Generic;

public class npcCreatureAI : MonoBehaviour
{
    private CreatureControl control;
    private Transform target;
    private DashComponent dash;
    private KnockbackHandler knockback;
    private Health health;

    [Header("Distances")]
    public float meleeRange = 1.2f; // still needed for melee attacks

    private float attackCooldownTimer = 0f;

    void Start()
    {
        control = GetComponent<CreatureControl>();
        dash = GetComponent<DashComponent>();
        knockback = GetComponent<KnockbackHandler>();
        health = GetComponent<Health>();
        control.controlType = CreatureControl.ControlType.AI;
    }

    void Update()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("PCreature");
            if (p != null) target = p.transform;
            return;
        }

        if (knockback != null && knockback.IsBeingKnockedBack)
        {
            control.ForceMove(Vector2.zero);
            return;
        }

        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        CreatureControl playerControl = target.GetComponent<CreatureControl>();
        if (playerControl == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;

        // Gather attacks and damage info
        List<AttackData> meleeAttacks = GetAllAttacksOfType(AttackType.Melee);
        List<AttackData> rangedAttacks = GetAllAttacksOfType(AttackType.Ranged);

        float maxMeleeDamage = GetMaxExpectedDamage(meleeAttacks, control.data, playerControl.data);
        float maxRangedDamage = GetMaxExpectedDamage(rangedAttacks, control.data, playerControl.data);

        bool prefersMelee = maxMeleeDamage > maxRangedDamage;
        bool prefersRanged = !prefersMelee;

        float incomingDamage = PredictIncomingDamage();

        // Random Movement with Melee Priority
        Vector2 move = Vector2.zero;

        if (prefersMelee && meleeAttacks.Count > 0 && attackCooldownTimer <= 0)
        {
            // High priority, move toward player if out of melee range
            if (distance > meleeRange * 0.9f)
            {
                move = dir; // always move toward player
                // minor random offset
                move += Random.insideUnitCircle * 0.2f;
            }
            else
            {
                move = Vector2.zero; // in range, stop to attack
            }
        }
        else
        {
            // Fully random movement for ranged or low-damage melee attacks
            float chaosMove = Random.value;
            if (chaosMove < 0.4f)
                move = dir * Random.Range(0.5f, 1f);
            else if (chaosMove < 0.7f)
                move = Random.insideUnitCircle.normalized * Random.Range(0.3f, 0.8f);
            else
                move = -dir * Random.Range(0.3f, 0.7f);
        }

        // Keep roughly near the player
        if (distance > 5f)
            move += dir * 1.0f;

        control.ForceMove(move);

        // Random Dash
        if (!dash.IsDashing() && Random.value < 0.2f)
        {
            Vector2 dashDir;
            float dashChoice = Random.value;
            if (dashChoice < 0.4f)
                dashDir = dir; // toward player
            else if (dashChoice < 0.7f)
                dashDir = -dir; // away
            else
                dashDir = Vector2.Perpendicular(dir) * (Random.value > 0.5f ? 1 : -1); // sideways

            dash.DashInDirection(dashDir);
        }

        // Aattack Logic
        if (attackCooldownTimer <= 0)
        {
            bool attackDone = false;

            // try high damage attack first
            if (prefersMelee && meleeAttacks.Count > 0)
                attackDone = TryAttack(meleeAttacks, dir, distance, meleeRange, incomingDamage);

            if (prefersRanged && !attackDone && rangedAttacks.Count > 0)
            {
                float maxRange = GetMaxRange(rangedAttacks);
                attackDone = TryAttack(rangedAttacks, dir, distance, maxRange, incomingDamage);
            }

            // if high-damage attack on cooldown, try a random other attack
            if (!attackDone)
            {
                List<AttackData> allAttacks = new List<AttackData>();
                allAttacks.AddRange(meleeAttacks);
                allAttacks.AddRange(rangedAttacks);

                if (allAttacks.Count > 0)
                {
                    AttackData randomAttack = allAttacks[Random.Range(0, allAttacks.Count)];
                    float attackRange = randomAttack.attackType == AttackType.Melee ? meleeRange : randomAttack.lifetime * randomAttack.speed;

                    if (distance <= attackRange)
                    {
                        control.ForceAttackWithDirection(randomAttack, dir);
                        attackCooldownTimer = randomAttack.cooldown;
                    }
                }
            }
        }
    }

    // Helpers
    private List<AttackData> GetAllAttacksOfType(AttackType type)
    {
        List<AttackData> result = new List<AttackData>();
        foreach (var atk in control.data.attacks)
            if (atk.attackData.attackType == type)
                result.Add(atk.attackData);
        return result;
    }

    private float GetMaxExpectedDamage(List<AttackData> attacks, CreatureData user, CreatureData target)
    {
        float maxDamage = -1f;
        foreach (var atk in attacks)
        {
            float dmg = ExpectedDamage(atk, user, target);
            if (dmg > maxDamage) maxDamage = dmg;
        }
        return maxDamage;
    }

    private float GetMaxRange(List<AttackData> attacks)
    {
        float maxRange = 0f;
        foreach (var atk in attacks)
        {
            float range = atk.lifetime * atk.speed;
            if (range > maxRange) maxRange = range;
        }
        return maxRange;
    }

    private bool TryAttack(List<AttackData> attacks, Vector2 dir, float distance, float range, float incomingDamage)
    {
        attacks.Sort((a, b) => ExpectedDamage(b, control.data, target.GetComponent<CreatureControl>().data)
            .CompareTo(ExpectedDamage(a, control.data, target.GetComponent<CreatureControl>().data)));

        foreach (var atk in attacks)
        {
            float dmg = ExpectedDamage(atk, control.data, target.GetComponent<CreatureControl>().data);
            if (distance <= range && (Random.value < 0.8f || dmg > incomingDamage))
            {
                control.ForceAttackWithDirection(atk, dir);
                attackCooldownTimer = atk.cooldown;
                return true;
            }
        }
        return false;
    }

    private float ExpectedDamage(AttackData atk, CreatureData user, CreatureData target)
    {
        if (atk == null) return -1f;

        bool isPhysical = atk.attackType == AttackType.Melee;
        int A = isPhysical ? user.attack : user.specialAttack;
        int D = isPhysical ? target.defense : target.specialDefense;

        float STAB = (user.elementType == atk.elementType) ? 1.5f : 1f;
        float typeMult = TypeChart.GetEffectiveness(atk.elementType, target.elementType);

        float avgRand = 0.925f;
        float baseDamage = ((2f * atk.damage * A / D) / 5f) + 2f;
        return baseDamage * STAB * typeMult * avgRand;
    }

    private float PredictIncomingDamage()
    {
        float totalThreat = 0f;
        CreatureControl playerControl = target.GetComponent<CreatureControl>();
        if (playerControl == null || playerControl.data.attacks == null) return 0f;

        float distance = Vector2.Distance(transform.position, target.position);

        foreach (var atk in playerControl.data.attacks)
        {
            AttackData attack = atk.attackData;
            if (attack == null) continue;

            float range = attack.attackType == AttackType.Melee ? meleeRange : attack.lifetime * attack.speed;

            if (distance <= range * 1.2f)
                totalThreat += ExpectedDamage(attack, playerControl.data, control.data);
        }

        return totalThreat;
    }
}