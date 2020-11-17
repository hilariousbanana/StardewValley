﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoSingleton<GameManager>
{
    enum GAMESTATE
    { 
        DAY,           
        NIGHT,              //22시 이전에 잠 든 경우(다음날 HP == MaxHP)
        LATENIGHT,       //22 ~ 24시 사이에 잠 든 경우 (다음날 HP == curHP * 1.2)
        OVERNIGHT,      //24시가 된 경우
        GAMEOVER        //Hp == 0
    }

    GAMESTATE state;
    public Image noticePanel;
    private DateTime loginTime;

    // Start is called before the first frame update
    void Start()
    {
        loginTime = System.DateTime.Now;
        state = GAMESTATE.DAY;
        BGMManager.instance.Play(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Time Flow
        TimeSpan timeChecker = System.DateTime.Now - loginTime;
        if (timeChecker.TotalSeconds >= 6)
        {
            DataController.instance.data.ChangeMinute(10);
            //Database.instance.ChangeMinute(10);
            loginTime = System.DateTime.Now;
            DataController.instance.data.LinkDataToText();
            //Database.instance.LinkDataToText();
        }

        //Sleep and Date adjust
        if(DataController.instance.data.isSleeping == true)
        {
            //Database.instance.isSleeping = false;
            DataController.instance.data.isSleeping = false;
            //int tempTime = Database.instance.sleepHour;
            int tempTime = DataController.instance.data.sleepHour;
            SleepTimeCheck(tempTime);
        }
        else if (DataController.instance.data.hour == 24)
        {
            ChangeState(GAMESTATE.OVERNIGHT);
        }

        //CheckGameOver
        if (DataController.instance.data.curHp <= 0)
        {
             ChangeState(GAMESTATE.GAMEOVER);
        }
    }

    void ChangeState(GAMESTATE _state)
    {
        state = _state;
        switch(_state)
        {
            case GAMESTATE.NIGHT:
                {
                    //Database.instance.curHp = Database.MaxHp;
                    //Database.instance.RenewalDay();
                    DataController.instance.data.curHp = Database.MaxHp;
                    DataController.instance.data.RenewalDay();
                    ChangeState(GAMESTATE.DAY);
                }
                break;

            case GAMESTATE.LATENIGHT:
                {
                    if (DataController.instance.data.curHp < 20)
                    {
                        DataController.instance.data.curHp = 70;
                    }
                    else if(DataController.instance.data.curHp < 50)
                    {
                        DataController.instance.data.curHp = 60;
                    }
                    ChangeState(GAMESTATE.DAY);
                }
                break;

            case GAMESTATE.OVERNIGHT:
                {
                    noticePanel.sprite = Resources.Load("Sprites/" + "DayEnded_Notice", typeof(Sprite)) as Sprite;
                    noticePanel.gameObject.SetActive(true);
                    DataController.instance.data.RenewalDay();
                    ChangeState(GAMESTATE.DAY);
                }
                break;

            case GAMESTATE.GAMEOVER:
                {
                    noticePanel.sprite = Resources.Load("Sprites/" + "GameOver_Notice", typeof(Sprite)) as Sprite;
                    noticePanel.gameObject.SetActive(true);
                    DataController.instance.data.ResetDay();
                    ChangeState(GAMESTATE.DAY);
                }
                break;
        }
    }

    void SleepTimeCheck(int _time)
    {
        if (_time < 22)
        {
            ChangeState(GAMESTATE.NIGHT);
        }
        else
        {
            ChangeState(GAMESTATE.LATENIGHT);
        }
    }
}
