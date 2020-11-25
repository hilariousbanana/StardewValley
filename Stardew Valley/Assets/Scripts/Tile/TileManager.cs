﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public enum TSTATE
    {
        PLANTABLE,      //밭에 씨앗을 심을 수 있는 상태
        ACTIVATED,      //플레이어 반경 내에 타일이 있는 상태
        STAGE1,
        STAGE2,
        STAGE3,
        STAGE4,
        STAGE5          //Crop. 추수가 가능한 상태
    }
    public TSTATE tileState;

    public Image noticePanel;
    public SpriteRenderer spriteRenderer;

    private GameObject player;
    public GameObject canvasPanel;
    public GameObject plantPanel;
    public GameObject activatedTile;
    public GameObject reapPanel;

    Transform targetPos;

    private float distance;
    private int seedType;
    public int tileNumber;
    private int slotNumber =0;

    private int plantedDay = 0;
    private int plantedHour = 0;
    private int plantedType = 0;
    private int tempState = 0;

    private bool isPlanted = false;
    private bool isStarted = false;
    private bool isReaped = false;
    private bool isReset = false;
    private bool dayAfter = false;
    private bool dayAfter_2 = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<MovingObject>().gameObject;
        tileState = TSTATE.PLANTABLE;
        targetPos = GetComponent<Transform>();
    }

    void Update()
    {
        slotNumber = DataController.instance.data.chosenSlot;

        if (DataController.instance.data.sleepException == true)
        {
            UpdateVariable();
            DataController.instance.data.sleepException = false;
        }

        if (!isPlanted) //심은게 없는 경우
        {
            if (GetDistance() <=34.0f) //수치 수정해야함.
            {
                ChangeState(TSTATE.ACTIVATED);
            }
            else
            {
                ChangeState(TSTATE.PLANTABLE);
            }
        }
        else //있는 경우
        {
            if(!isStarted)
            {
                isStarted = true;
                plantedDay = DataController.instance.data.day;
                plantedHour = DataController.instance.data.hour;

                if (plantedHour <= 20 &&
                    seedType != 10002) //하루 지나면 자라있게
                {
                    plantedType = 0;
                }
                else //이틀 후에 자라도록
                {
                    plantedType = 1;
                }
                CropTimer(plantedDay, plantedHour);
            }
        }
        UpdateVariable();
        GetDistance();
    }

    void ChangeState(TSTATE _state)
    {
        tileState = _state;

        switch(_state)
        {
            case TSTATE.PLANTABLE:
                activatedTile.SetActive(false);
                break;
            case TSTATE.ACTIVATED:
                activatedTile.SetActive(true);
                break;
            case TSTATE.STAGE1:
                activatedTile.SetActive(false);
                break;
            case TSTATE.STAGE2:
                ChangeImage();
                //StartCoroutine(CheckTimer(plantedType));
                break;
            case TSTATE.STAGE3:
                ChangeImage();
                //StartCoroutine(CheckTimer(plantedType));
                break;
            case TSTATE.STAGE4:
                ChangeImage();
                if(seedType == 10005)
                {
                    tileState = TSTATE.STAGE5;
                }
                //StartCoroutine(CheckTimer(plantedType));
                break;
            case TSTATE.STAGE5:
                {
                    if(seedType != 10005)
                    {
                        ChangeImage();
                    }
                    if (isReaped)
                        ChangeState(TSTATE.PLANTABLE);
                }
                break;
        }
    }

    float GetDistance()
    {
        distance = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y),
        new Vector2(player.transform.position.x, player.transform.position.y));
        //Vector2 temp = this.transform.position - player.transform.position;
        //distance = temp.sqrMagnitude;
        return distance;
    }

    void CropTimer(int _day, int _hour)
    {
        switch(seedType)
        {
            case 10001:
                {
                    
                }
                break;
            case 10002:
                break;
            case 10003:
                break;
            case 10004:
                break;
            case 10005:
                break;
        }
    }

    public void OnMouseDown()
    {
        Debug.Log(GetDistance());
        if(DataController.instance.data.tileActivated == false && !isStarted && DataController.instance.data.noticeActivated == false && !DataController.instance.data.optionActivated)
        {
            canvasPanel.SetActive(true);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos.position);
            plantPanel.transform.position = new Vector3(screenPos.x + 40, screenPos.y -40, 0);
            DataController.instance.data.tileActivated = true;
            plantPanel.SetActive(true);
        }
        if (tileState == TSTATE.STAGE5 && DataController.instance.data.noticeActivated == false) //추수
        {
            canvasPanel.SetActive(true);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos.position);
            reapPanel.transform.position = new Vector3(screenPos.x + 40, screenPos.y - 40, 0);
            DataController.instance.data.tileActivated = true;
            reapPanel.SetActive(true);

        }
    }

    void ResetTile()
    {
        isReset = true;
        isReaped = false;
        tileState = TSTATE.PLANTABLE;
        spriteRenderer.sprite = null;
        seedType = 0;
    }

    IEnumerator CheckTimer(int _plantedType)
    {
        switch(_plantedType)
        {
            case 0:
                {
                    yield return new WaitUntil(() => dayAfter == true);
                    Debug.Log("wait until now");
                    ChangeState(tileState++);
                }
                break;
            case 1:
                {
                    yield return new WaitUntil(() => dayAfter_2 == true);
                    ChangeState(tileState++);
                }
                break;
        }
    }

    void UpdateVariable()
    {
        if (isStarted)
        {
            switch (plantedType)
            {
                case 0:
                    {
                        if (DataController.instance.data.day - plantedDay == 1 || DataController.instance.data.day - plantedDay == -20)
                        {
                            if(tileState != TSTATE.STAGE5)
                            {
                                Debug.Log("the day after");
                                dayAfter = true;
                                tempState = (int)tileState;
                                tempState++;
                                ChangeState((TSTATE)tempState);
                                Debug.Log("tile State now is" + tileState);
                                plantedDay = DataController.instance.data.day;
                                tileState = (TSTATE)tempState;
                                dayAfter = false;
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        if (DataController.instance.data.day - plantedDay == 2 || DataController.instance.data.day - plantedDay == -19)
                        {
                            if(tileState != TSTATE.STAGE5)
                            {
                                dayAfter_2 = true;
                                tempState = (int)tileState;
                                tempState++;
                                ChangeState((TSTATE)tempState);
                                plantedDay = DataController.instance.data.day;
                                tileState = (TSTATE)tempState;
                                dayAfter_2 = false;
                            }
                        }
                    }
                    break;
            }
        } 
    }

    void ChangeImage()
    {
        spriteRenderer.sprite = Resources.Load($"ItemIcon/{seedType}_{tileState}", typeof(Sprite)) as Sprite;
    }

    public void CancelButton()
    {
        Debug.Log("Pressed CancelButton. Tile Number is" + tileNumber);
        DataController.instance.data.tileActivated = false;
        plantPanel.SetActive(false);
        reapPanel.SetActive(false);
        canvasPanel.SetActive(false);
    }

    public void PlantButton()
    {
        Debug.Log("Pressed PlantButton. Tile Number is" + tileNumber);
        if (tileState == TSTATE.ACTIVATED && Inventory.instance.isEmpty == false) //심기
        {
            if (Inventory.instance.inventoryItemList[slotNumber].itemType == Item.ItemType.Seed)
            {
                DataController.instance.data.ChangeHP(-3);
                seedType = Inventory.instance.inventoryItemList[slotNumber].itemID;
                ChangeState(TSTATE.STAGE1);
                Debug.Log("is Clicked. And SeedType is" + seedType);
                spriteRenderer.sprite = Resources.Load($"ItemIcon/{seedType}_{tileState}", typeof(Sprite)) as Sprite;
                Inventory.instance.UseAnItem(slotNumber);
                isPlanted = true;
            }
            else
            {
                noticePanel.sprite = Resources.Load("Sprites/" + "SeedOnly_Notice", typeof(Sprite)) as Sprite;
                noticePanel.gameObject.SetActive(true);
                DataController.instance.data.noticeActivated = true;
            }
        }
        else if (tileState == TSTATE.PLANTABLE && Inventory.instance.isEmpty == false) //너무 멀리 있음
        {
            noticePanel.sprite = Resources.Load("Sprites/" + "FarTile_Notice", typeof(Sprite)) as Sprite;
            noticePanel.gameObject.SetActive(true);
            DataController.instance.data.noticeActivated = true;
        }
        DataController.instance.data.tileActivated = false;
        plantPanel.SetActive(false);
        canvasPanel.SetActive(false);
    }

    public void ReapButton()
    {
        AudioManager.instance.Play("Reap");
        isReaped = true;
        isPlanted = false;
        isStarted = false;
        reapPanel.SetActive(false);
        DataController.instance.data.ChangeHP(-3);
        Inventory.instance.GetAnItem(seedType + 10000);
        ResetTile();
    }
}