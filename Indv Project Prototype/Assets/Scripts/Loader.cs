using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TMPro;

public class Loader : MonoBehaviour
{
    public TextAsset textFile;
    Sentences sentences_json;
    CustomSentence[] sentences;

    public TextAsset knownSentencesFile;

    public bool tutorial = false;

    public TextMeshProUGUI text_helper;

    int currentIndex = 0;
    public TextMeshProUGUI text;
    string[] correctWords;
    int bomb_loc = -1;
    //may have to use string instead of ints if tmp doesn't get the same values
    public float no_bomb_chance = 0; // 0 = 100% for bomb 1 = 0%

    public PopUp popup;

    public NLPOrgs NLPOrgs;

    public Points points;

    public int KnownSentenceChance = 1;

    public int tutorialIndex = 0;

    DiffcultyMod diff_mod;

    public bool Sendable = false;

    string[] s;
    string[] knownSentences;

    public Scrollbar scrollbar;

    public void Set_Bomb_Chance(float i)
    {
        if(i >= 0 && i <= 1)
        {
            no_bomb_chance = i;

        } else
        {
            Debug.Log("Bomb chance not valid value");
            no_bomb_chance = 1;
        }
        
    }



    void Start()
    {
        diff_mod = GetComponent<DiffcultyMod>();
        s = textFile.text.Split('\n');
        if(knownSentencesFile != null)
        {
            knownSentences = knownSentencesFile.text.Split('\n');
        }
        currentIndex = 0;
        tutorialIndex = 0;
    }

    public void Load_Next_Sentence()
    {
        Sendable = false;

        if (tutorial == true)
        {
            points.knownSentence = true;
            Load_Tutorial_Sentence();
        }
        else if (Random.Range(0, KnownSentenceChance) == 0)
        {
            points.knownSentence = true;
            LoadKnownSentence();
        }
        else
        {
            points.knownSentence = false;
            Load_Unknown_Sentence();
        }
    }

    public void Load_Prev_Sentence()
    {
        Sendable = false;

        if (tutorial == true)
        {
            points.knownSentence = true;
            tutorialIndex -= 1;
            Load_Tutorial_Sentence();
        }
        else
        {
            points.knownSentence = true;
            LoadKnownSentence(true);
        }
    }

    public void Load_Unknown_Sentence()
    {

        if (s == null)
        {
            return;
        }
        if (s.Length == 0)
        {
            return;
        }
        diff_mod.Load_Sentence();
        currentIndex = Random.Range(0, s.Length);
        if (currentIndex < s.Length)
        {
            string temp_text = s[currentIndex];
            if (Random.value > no_bomb_chance)
            {
                // %Chance for a bomb
                temp_text = PutBomb(temp_text);
                points.Set_Bomb_Loc(bomb_loc);
            }
            NLPOrgs.ParseSentence(temp_text);
            text.text = temp_text;
            points.set_Correct_Loc(NLPOrgs.GetLoc());
            NLPOrgs.ClearLoc();
            currentIndex += 1;

            //Flag this sentence as intresting
            Sendable = true;

            //start timer
            diff_mod.StartTimer();

            if(scrollbar != null)
            {
                scrollbar.value = 1;
            }
        }
        else
        {
            text.text = "Out of sentences";
            GameManager.GM.GetComponent<Points>().set_Correct_Loc(null);
        }
    }

    public void Load_Tutorial_Sentence()
    {
        if (knownSentences == null)
        {
            return;
        }
        if (knownSentences.Length == 0)
        {
            return;
        }
        if (tutorialIndex >= knownSentences.Length)
        {
            if(popup != null)
            {
                popup.SetActive(true);
            }
            //Save beating the tutorial:
            GameManager.GM.SetLevel(1, true);
            //Tutorial is beat for both systems so user doesnt have to do it twice
            GameManager.GM.SetLevel(1, false);

            return;
        }
        if (tutorialIndex < 0 || tutorialIndex >= knownSentences.Length)
        {
            return;
        }
        string temp_text = knownSentences[tutorialIndex];
        string loc = "";
        if (temp_text.Length > 0)
        {
            int i = temp_text.IndexOf(" ");

            //Split the locations from the setence
            loc = temp_text.Substring(0, i);
            temp_text = temp_text.Substring(i + 1);
            string[] temp = loc.Split(',');

            //Turn the locations into an array
            int[] locations = new int[temp.Length];
            for (int j = 0; j < temp.Length; j++)
            {
                int val = 0;
                if (int.TryParse(temp[j], out val))
                {
                    locations[j] = val;
                }
                else
                {
                    locations[j] = -2;
                }
            }
            text.text = temp_text;
            points.set_Correct_Loc(locations);

            //No bombs in tutorial
            points.Set_Bomb_Loc(-1);

            tutorialIndex++;

            //start timer
            diff_mod.StartTimer();

            if (scrollbar != null)
            {
                scrollbar.value = 1;
            }

        }
        else
        {
            Load_Next_Sentence();
            return;
        }
    }

    public void LoadKnownSentence(bool prev=false)
    {
        if (knownSentences == null)
        {
            return;
        }
        if (knownSentences.Length == 0)
        {
            return;
        }
        if (prev == false)
        {
            currentIndex = Random.Range(0, knownSentences.Length);
            //currentIndex += 1;
        }
        string temp_text = knownSentences[currentIndex];
        string loc = "";
        if(temp_text.Length > 0)
        {
            int i = temp_text.IndexOf(" ");

            //Split the locations from the setence
            loc = temp_text.Substring(0, i);
            temp_text = temp_text.Substring(i+1);
            string[] temp = loc.Split(',');

            //Turn the locations into an array
            int[] locations = new int[temp.Length];
            for(int j = 0; j<temp.Length; j++)
            {
                int val = 0;
                if(int.TryParse(temp[j], out val))
                {
                    locations[j] = val;
                }
                else
                {
                    locations[j] = -2;
                }
            }
            text.text = temp_text;
            points.set_Correct_Loc(locations);

            //No bombs in known sentences
            points.Set_Bomb_Loc(-1);

            //start timer
            diff_mod.StartTimer();

            if (scrollbar != null)
            {
                scrollbar.value = 1;
            }
        } else
        {
            Load_Next_Sentence();
            return;
        }
    }

    /* May need this later
    public string CutTextToFit(string s)
    {
        string return_string;
        return_string = s.Substring(0, max_text_length);
        string[] split = return_string.Split(seperators);
        List<string> split_list = new List<string>(split);
        for (int i = 0; i < sentences[currentIndex].pos_correct.Length; i++)
        {
            if (sentences[currentIndex].pos_correct[i] >= split.Length)
            {
                sentences[currentIndex].pos_correct[i] += 1;
            }
        }
    }
    */

    public string PutBomb(string s)
    {
        string[] split = s.Split(' ');
        List<string> split_list = new List<string>(split);
        bomb_loc = Random.Range(0, split.Length);
        split_list.Insert(bomb_loc, "BOMB");

        string bombed_Text = string.Join(" ", split_list.ToArray());

        //Find where bomb really is according to TMP
        text_helper.text = bombed_Text;
        text_helper.ForceMeshUpdate(true);

        bomb_loc = -1;

        int counter = 0;

        foreach (TMP_WordInfo wInfo in text_helper.textInfo.wordInfo)
        {
            if(wInfo.characterCount > 0)
            {
                if(wInfo.GetWord() == "BOMB")
                {
                    bomb_loc = counter;
                    break;
                }
                
            }
            counter++;
        }

        return bombed_Text;
    }
}
