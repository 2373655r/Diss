using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallUnit : MonoBehaviour
{
    public int health = 100;
    int curHealth = 100;

    public bool friendly = false;

    public Image healthBar;

    Money money;

    AudioManager audioManager;

    public GameObject deathParticles;

    void Start()
    {
        curHealth = health;
        money = FindObjectOfType<Money>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void takeDamage(int damage)
    {
        curHealth -= damage;

        //Healthbar
        float ratio = (float)curHealth / (float)health;
        if (ratio >= 0 && ratio <= 1)
        {
            healthBar.fillAmount = ratio;
        }

        if (curHealth <= 0)
        {
            if(friendly == false)
            {
                money.AddMoney(50);
                if(audioManager != null)
                {
                    audioManager.Play("enemyDeath");
                }
            }
            else
            {
                if (audioManager != null)
                {
                    audioManager.Play("friendDeath");
                }
            }

            if(deathParticles != null)
            {
                GameObject go = Instantiate(deathParticles,transform.position,transform.rotation);
                Destroy(go, 2f);
            }
            Destroy(gameObject);
        }
    }
}
