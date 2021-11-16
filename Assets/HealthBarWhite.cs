using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarWhite : MonoBehaviour
{
    private Image healthBar;
    public float currentHealth;
    public float MaxHealth = 100f;
    PlayerCombat PlayerCombat;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        PlayerCombat = GameObject.Find("0 Team").GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = PlayerCombat.health;
        healthBar.fillAmount = currentHealth / MaxHealth;
    }
}
