using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    int modifier;
    public int diceNum;
    public int maxDamageVal;

    encumbranceStates playerState = encumbranceStates.normal;

    GameObject ownerObject;

    public LayerMask groundLayer;
    Attack _attack;
    HitUI damagePrefab;
    // Start is called before the first frame update
    void Start()
    {
        _attack = new Attack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        AIThinker AI = other.GetComponent<AIThinker>();
        PlayerController player = other.GetComponent<PlayerController>();

        if(AI != null && ownerObject.GetComponent<PlayerController>())
        {
            bool hasHit;

            HitUI hitText = Instantiate(damagePrefab);

            if(playerState == encumbranceStates.normal)
            {
                //For now, factor in a normal roll for the normal state
                hasHit = _attack.AttackRoll(modifier, AI.stats.armourClass);
            }
            else
            {
                hasHit = _attack.AttackAdvDisadv(modifier, AI.stats.armourClass, false);
            }

            if (hasHit)
            {
                //
                int damage = _attack.DamageRoll(diceNum, maxDamageVal, modifier);
                Debug.Log(damage);
                AI.healthSystem.Damage(damage);
                hitText.SetUITextAndPos($"Hit! \n {damage} damage!", other.transform.position);
            }
            else
            {
                hitText.SetUITextAndPos("Missed!", other.transform.position);
            }

            Destroy(gameObject);
        } else if(player != null && ownerObject.GetComponent<AIThinker>())
        {
            //Damage the player
            bool hasHit = false;

            HitUI hitText = Instantiate(damagePrefab);

            hasHit = _attack.AttackRoll(modifier, player.GetPlayerStats().armourClass);

            if (hasHit)
            {
                int damage = _attack.DamageRoll(diceNum, maxDamageVal, modifier);
                Debug.Log(damage);
                player._healthSystem.Damage(damage);
                hitText.SetUITextAndPos($"Hit! \n {damage} damage!", other.transform.position);
            }
            else
            {
                hitText.SetUITextAndPos("Missed!", other.transform.position);
            }

            Destroy(gameObject);
        } else if(other.gameObject.layer == (1 << 7))
        {
            int groundLayer = 7;
            RaycastHit groundHit;

            int bitmask = (1 << groundLayer);

            bool hit = Physics.Raycast(transform.position, Vector3.down, out groundHit, .5f, bitmask);

            /*
            if (hit)
            {
                Destroy(gameObject);
            }*/

            
            
        }
    }

    public void SetModifierAndRollValues(int modifierVal, int diceNum, int maxDamageVal, HitUI damageUI, GameObject ownerObject)
    {
        damagePrefab = damageUI;
        modifier = modifierVal;
        this.diceNum = diceNum;
        this.maxDamageVal = maxDamageVal;
        this.ownerObject = ownerObject;
    }

    public void SetEncumbranceState(encumbranceStates state)
    {
        playerState = state;
    }

    


}
