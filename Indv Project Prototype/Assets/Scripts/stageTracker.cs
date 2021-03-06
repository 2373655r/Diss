using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stageTracker : MonoBehaviour
{

    enum Stage {Stages,InputData,HyperParameters,Model};
    Stage stageState = Stage.Stages;

    public GameObject stages;
    public GameObject inputData;
    public GameObject hyperParameters;
    public GameObject model;

    public WaveSpawner waveSpawner;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        SetOneActive(stages);
    }

    // Sets the given canvas active and all others turned off
    // Updates the stageState to match
    public void SetOneActive(GameObject gm)
    {
        Time.timeScale = 1;
        audioManager.StartFadeStarter("levelTheme", 3f, 0.6f);
        if (gm == stages)
        {
            stages.SetActive(true);
            stageState = Stage.Stages;
        } else
        {
            stages.SetActive(false);
        }

        if (gm == inputData)
        {
            audioManager.StartFadeStarter("levelTheme", 3f, 0);
            inputData.SetActive(true);
            stageState = Stage.InputData;
        }
        else
        {
            inputData.SetActive(false);
        }

        if (gm == hyperParameters)
        {
            hyperParameters.SetActive(true);
            stageState = Stage.HyperParameters;
        }
        else
        {
            hyperParameters.SetActive(false);
        }

        if (gm == model)
        {
            model.SetActive(true);
            waveSpawner.StartWave();
            stageState = Stage.Model;
        }
        else
        {
            model.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(stageState == Stage.Stages)
            {
                SceneManager.LoadScene(0);
            } else if(stageState == Stage.Model)
            {
                waveSpawner.EndWave();
                SetOneActive(stages);
            } else
            {
                SetOneActive(stages);
            }
        }
    }
}
