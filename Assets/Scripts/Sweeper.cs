using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sweeper : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public GameObject radar;

    public void Setsweepmode()
    {
        AudioManager.Instance.SweeperSound();

        if (Game.Instance.sweepMode == -1)
        {
            if (Game.Instance._userDatas.gold >= 1)
            {
                Game.Instance._userDatas.gold -= 1;
            }
            else
            {
                resultText.text = "Not enough gold";
                resultText.gameObject.SetActive(true);
                Invoke("removeResult", 1f);
                return;
            }
            resultText.text = "Activated!";
            resultText.gameObject.SetActive(true);
            Invoke("removeResult", 1f);
            Game.Instance.SaveData();
        }

        Game.Instance.sweepMode = -Game.Instance.sweepMode;
        SwitchSweepMode();
        if(Game.Instance.sweepMode == -1 ) 
            radar.SetActive(false);
        else
            radar.SetActive(true);
           
    }

    public void removeResult()
    {
        resultText.gameObject.SetActive(false);
    }

    public void SwitchSweepMode()
    {
        Game.Instance.isImageVisible = !Game.Instance.isImageVisible;
        Game.Instance.sweepScreen.SetActive(Game.Instance.isImageVisible);
    }
}
