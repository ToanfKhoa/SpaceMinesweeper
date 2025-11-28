using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDatas 
{
    public int level;
    public int gold;
    public int diamond;
    public int maxHeart;
    public int heart;
    public int timeHeart;
    public float timeDig;
    public int probalityGold;
    public int probalityDiamond;

    public bool skill1;
    public bool skill2;
    public bool skill3;
    public bool skill4;
    public bool skill5;
    public bool skill6;
    public UserDatas()
    {
        Init();
    }

    public void Init()
    {
        level = 1;
        gold = 0;
        diamond = 0;
        maxHeart = 3;
        heart = 3;
        timeDig = 1.5f;
        probalityGold = 15;
        probalityDiamond = 4;
        skill1 = false;
        skill2 = false;
        skill3 = false;
        skill4 = false;
        skill5 = false;
        skill6 = false;
    }
}
