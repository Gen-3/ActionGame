using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public float damageAmount;//これが相手Managerに呼ばれてダメージになる
    public float defaultAttack;//ビヘイビアのOnStateEnterからPlayerManagerを通してこいつに係数をかけてdamageAmountとする

    private void Start()
    {
        damageAmount = defaultAttack;
    }
}
