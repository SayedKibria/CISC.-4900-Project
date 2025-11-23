using UnityEngine;

public class SimpleGolemAI : MonoBehaviour
{
    private CreatureControl control;
    private Transform target;

    public float meleeRange = 1f;
    public float rangedRange = 4f;

    void Start()
    {
        control = GetComponent<CreatureControl>();
        control.controlType = CreatureControl.ControlType.AI;
    }

    void Update()
    {
        // Try to find player creature until it exists
        if (target == null)
        {
            GameObject creature = GameObject.FindGameObjectWithTag("PCreature");
            if (creature != null)
                target = creature.transform;

            return;
        }

        // Movement toward target
        Vector2 dir = (target.position - transform.position).normalized;
        control.ForceMove(dir);

        float distance = Vector2.Distance(transform.position, target.position);

        // Attack logic based on distance + creatureData
        if (distance <= meleeRange)
        {
            var meleeAttack = GetMeleeAttack();
            if (meleeAttack != null)
                control.ForceAttack(meleeAttack);
        }
        else if (distance <= rangedRange)
        {
            var rangedAttack = GetRangedAttack();
            if (rangedAttack != null)
                control.ForceAttack(rangedAttack);
        }
    }

    private AttackData GetMeleeAttack()
    {
        foreach (var atk in control.data.attacks)
        {
            if (atk.attackData.attackType == AttackType.Melee)
                return atk.attackData;
        }
        return null;
    }

    private AttackData GetRangedAttack()
    {
        foreach (var atk in control.data.attacks)
        {
            if (atk.attackData.attackType == AttackType.Ranged)
                return atk.attackData;
        }
        return null;
    }
}