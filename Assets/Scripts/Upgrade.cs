using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textDiamond;
    public TextMeshProUGUI textInfor;
    private int whatToDo = 0;
    public int costGold = 0;
    public int costDiamond = 0;
    public Button dig2;
    public Button heart2;
    public Button ore2;
    public Button dig1;
    public Button heart1;
    public Button ore1;

    public void ConfirmUpgrade()
    {
        
        AdjustCost(whatToDo);
        if (Game.Instance._userDatas.gold >= costGold && Game.Instance._userDatas.diamond >= costDiamond)
        {
            AudioManager.Instance.WinSound();
            DoUpgrade(whatToDo);
            Game.Instance._userDatas.gold -= costGold;
            Game.Instance._userDatas.diamond -= costDiamond;            
            whatToDo = 0;
            Game.Instance.SaveData();
        }     
    }    


    public void AdjustCost(int i)
    {
        if(whatToDo == 0)
        {
            costGold = 0;
            costDiamond = 0;
        }    
        if (whatToDo == 1)
        {
            costGold = 100;
            costDiamond = 0;
        }
        if (whatToDo == 2)
        {
            costGold = 200;
            costDiamond = 5;
        }
        if (whatToDo == 3)
        {
            costGold = 500;
            costDiamond = 10;
        }
        if (whatToDo == 4)
        {
            costGold = 1000;
            costDiamond = 20;
        }
        if (whatToDo == 5)
        {
            costGold = 500;
            costDiamond = 10;
        }
        if (whatToDo == 6)
        {
            costGold = 1000;
            costDiamond = 20;
        }
    }    

    public void DoUpgrade(int i)
    {   
        if(whatToDo == 1)
        {
            costGold = 100;
            costDiamond = 0;
            Game.Instance._userDatas.timeDig -= 0.5f;
            dig2.interactable = true;
            dig1.interactable = false;
            Game.Instance._userDatas.skill1 = false;
            Game.Instance._userDatas.skill2 = true;
        }
        if (whatToDo == 2)
        {
            costGold = 200;
            costDiamond = 5;
            Game.Instance._userDatas.timeDig -= 0.5f;
            dig2.interactable = false;
            Game.Instance._userDatas.skill2 = false;
        }
        if (whatToDo == 3)
        {
            costGold = 500;
            costDiamond = 10;
            Game.Instance._userDatas.maxHeart += 1;
            Game.Instance._userDatas.heart += 1;
            heart2.interactable = true;
            heart1.interactable = false;
            Game.Instance._userDatas.skill3 = false;
            Game.Instance._userDatas.skill4 = true;
        }
        if (whatToDo == 4)
        {
            costGold = 1000;
            costDiamond = 20;
            Game.Instance._userDatas.maxHeart += 1;
            Game.Instance._userDatas.heart += 1;
            heart2.interactable = false;
            Game.Instance._userDatas.skill4 = false;
        }
        if (whatToDo == 5)
        {
            costGold = 500;
            costDiamond = 10;
            Game.Instance._userDatas.probalityGold += 10;
            ore2.interactable = true;
            ore1.interactable = false;
            Game.Instance._userDatas.skill5 = false;
            Game.Instance._userDatas.skill6 = true;
        }
        if (whatToDo == 6)
        {
            costGold = 1000;
            costDiamond = 20;
            Game.Instance._userDatas.probalityGold += 5;
            Game.Instance._userDatas.probalityDiamond += 3;
            ore2.interactable = false;
            Game.Instance._userDatas.skill6 = false;
        }

    }

    public void Choose(int x)
    {
        whatToDo = x;
    }

    public void AdjustInfor(int z)
    {
        
            if (textInfor != null)
        {
            if(z == 0)
            {
                textInfor.text = "Upgrade successful";
            }    
            if (z == 1)
            {
                textInfor.text = "Digging time: 2s -> 1.5s";
            }
            if (z == 2)
            {
                textInfor.text = "Digging time: 1.5s -> 1s";
            }
            if (z == 3)
            {
                textInfor.text = "Max hearts: 3 -> 4";
            }
            if (z == 4)
            {
                textInfor.text = "Max hearts: 4 -> 5";
            }
            if (z == 5)
            {
                textInfor.text = "Chance to collect gold +10%";
            }
            if (z == 6)
            {
                textInfor.text = "Chance to collect gold +5%, diamond +3%";
            }
        }
           
    }

    void Start()
    {
        this.textGold.text = Game.Instance._userDatas.gold.ToString();
        this.textDiamond.text = Game.Instance._userDatas.diamond.ToString();
        if(dig1 != null)
        { 
            dig1.interactable = Game.Instance._userDatas.skill1;
            dig2.interactable = Game.Instance._userDatas.skill2;
            heart1.interactable = Game.Instance._userDatas.skill3;
            heart2.interactable = Game.Instance._userDatas.skill4;
            ore1.interactable = Game.Instance._userDatas.skill5;
            ore2.interactable = Game.Instance._userDatas.skill6;
        }          
    }
    
    void Update()
    {
        textGold.text = Game.Instance._userDatas.gold.ToString();
        textDiamond.text = Game.Instance._userDatas.diamond.ToString();
        AdjustInfor(whatToDo);
    }

}
