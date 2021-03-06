using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiffcultyMod : MonoBehaviour
{
    Loader loader;
    public TextMeshProUGUI text;

    public float no_bomb_chance = 1; // 1=never 0=always
    public bool reversed = false;
    public float timer_max = 15; //if timer <  0 then no timer
    float current_time;
    bool timer_on = false;

    public TimerBar timebar;

    AudioManager am;

    Points points;

    TMPDetector tmpd;

    // Start is called before the first frame update
    void Start()
    {
        loader = GetComponent<Loader>();
        loader.Set_Bomb_Chance(no_bomb_chance);
        am = FindObjectOfType<AudioManager>();
        points = FindObjectOfType<Points>();
        tmpd = FindObjectOfType<TMPDetector>();
    }

    public void Load_Sentence()
    {
        loader.Set_Bomb_Chance(no_bomb_chance);
        if (reversed == true)
        {
            text.rectTransform.eulerAngles = new Vector3(180, 0, 180);
        }
        else
        {
            text.rectTransform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void Update()
    {
        if(timebar == null)
        {
            return;
        }
        if(timer_on)
        {
            if(current_time <= 0)
            {
                //Play times up sound
                if (am != null)
                {
                    am.Play("time");
                }

                //Lose points
                if(points != null)
                {
                    points.LoseTenPerPoints();
                }

                Debug.Log("Out of time");
                timer_on = false;

                tmpd.LoadNextSentenceNoChecks();
                return;
            } 
            timebar.SetHealth(current_time);
            current_time -= Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        if(timebar == null)
        {
            return;
        }
        if(timer_max < 0)
        {
            return;
        }
        timebar.SetMaxHealth(timer_max);
        current_time = timer_max;
        timer_on = true;
    }
}
