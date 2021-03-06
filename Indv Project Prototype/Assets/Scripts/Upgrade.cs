using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    public static Upgrade instance;
    public Button[] btns;
    public Button sellBtn;

    Money money;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one upgrade shop in scene");
            return;
        }
        instance = this;
    }

    public Canvas canvas;
    private Turret callingTurret;
    private BoxCollider2D col;
    private Tower callingTower;
    private Capacitor callingCapacitor;
    private Node callingNode;

    public void Start()
    {
        col = GetComponent<BoxCollider2D>();
        money = FindObjectOfType<Money>();
    }

    public void ShowUpgrades(Vector3 pos, GameObject tower, Node node)
    {
        transform.position = pos + new Vector3(0, 0, -1);
        callingTower = tower.GetComponent<Tower>();
        callingNode = node;

        int counter = 0;
        foreach (Upgrades upgrade in callingTower.getUpgrades())
        {
            if (counter < btns.Length)
            {
                if(upgrade.available == false)
                {
                    btns[counter].interactable = false;
                } else
                {
                    btns[counter].interactable = true;
                }
                btns[counter].GetComponentInChildren<Text>().text = upgrade.name + " " + upgrade.cost + "$";
                counter++;
            }
        }

        sellBtn.GetComponentInChildren<Text>().text = "Sell " + callingTower.resellValue + "$";


        col.enabled = true;
        canvas.enabled = true;
    }

    public void HideUpgrades()
    {
        col.enabled = false;
        canvas.enabled = false;
    }

    public void PickSelection(int i)
    {
        Upgrades[] upgradesarr = callingTower.getUpgrades();
        if (i >= 0 && i < upgradesarr.Length)
        {
            bool succeed = false;
            if (i == 0)
            {
                //first upgrade
                succeed = money.Purchase(upgradesarr[0].cost);
            }
            else if (i == 1)
            {
                //second upgrade
                succeed = money.Purchase(upgradesarr[1].cost);
            }
            if (succeed)
            {
                callingTower.Upgrade(i);
                HideUpgrades();
            }
        }
    }

    public void Sell()
    {
        if(callingTower != null)
        {
            if(callingTower.type == Tower.Type.Turret)
            {
                money.AddMoney(50);
            } else if (callingTower.type == Tower.Type.Capacitor)
            {
                money.AddMoney(100);
            } else if (callingTower.type == Tower.Type.Fuse)
            {
                money.AddMoney(500);
            }
        }
        Destroy(callingTower.gameObject);
        HideUpgrades();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        HideUpgrades();

    }
}
