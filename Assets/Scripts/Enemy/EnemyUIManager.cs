using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyUIManager : MonoBehaviour
{
    public Slider hpSlider;
    public TextMesh damageText;

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void UpdateHP(float value)
    {
        hpSlider.DOValue(value,0.3f);
    }

    public void UpdateDamageText(float damageAmount)
    {
        damageText.text = ((int)damageAmount).ToString();
    }
}
