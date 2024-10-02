using UnityEngine;
using EnumStruct;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ItemStruct;
using Unity.VisualScripting;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager um_instance;

    public UIState uiState;

    public GameObject uiTitle;
    public GameObject uiLobby;
    public GameObject uiRoom;
    public GameObject uiPlay;
    public GameObject roomList;
    public TextMeshProUGUI nickNameText;
    public TextMeshProUGUI roomNameText;

    public bool isPlayScene = false;
    public GameObject checkUnitPosition;
    public Button readyButton;
    public Button resetButton;

    public GameObject turnInfo;
    public GameObject watingTurn;
    public GameObject blueTurn;
    public GameObject redTurn;

    public TextMeshProUGUI knightDeathText;
    public TextMeshProUGUI royaltyDeathText;

    // 아이템
    public GameObject bottomLayer;
    public Button itemButton01;
    public Button itemButton02;
    public Button itemButton03;
    public Button itemButton04;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemNum;
    public TextMeshProUGUI itemInfo;
    public Button itemUse;

    public GameObject checkConfirm;
    public Button confirmButton;
    public Button cancelButton;

    public ItemNameNum curClickedItem;
    public GameObject selectUnit00;
    public GameObject selectUnit01;

    // 게임 엔딩
    public GameObject gameEnd;
    public TextMeshProUGUI endingText;

    void Start()
    {
        //uiState = UIState.Title;
        if (um_instance == null)
        {
            um_instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (um_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckReadyButton();
        CheckDeathUnit();
        ChangeTurnImage();
    }

    public void ChangeUiState(UIState state)
    {
        switch (state)
        {
            case UIState.Title:
                uiTitle.SetActive(true);
                uiLobby.SetActive(false);
                uiRoom.SetActive(false);
                uiPlay.SetActive(false);
                break; 
            case UIState.Lobby:
                uiTitle.SetActive(false);
                uiLobby.SetActive(true);
                uiRoom.SetActive(false);
                uiPlay.SetActive(false);
                break; 
            case UIState.Room:
                uiTitle.SetActive(false);
                uiLobby.SetActive(false);
                uiRoom.SetActive(true);
                uiPlay.SetActive(false);
                break; 
            case UIState.Play:
                uiTitle.SetActive(false);
                uiLobby.SetActive(false);
                uiRoom.SetActive(false);
                uiPlay.SetActive(true);
                break;
        }

        this.uiState = state;
    }

    public void CreatRoomButton()
    {
        //Debug.Log("생성 버튼 닉네임 : " + (nickNameText.text == "") + " / 방 이름 : " + (roomNameText.text == ""));
        //Debug.Log("닉네임 텍스트: '" + nickNameText.text.Trim().Length + "'");

        string nickName = nickNameText.text.Trim().Length >= 2 ? nickNameText.text.Trim() : "";
        string roomName = roomNameText.text.Trim().Length >= 2 ? roomNameText.text.Trim() : "";

        //Debug.Log("닉네임 : " + nickName + " / 방 이름 : " + roomName);
        //Debug.Log("일치 ? 닉네임 : " + (nickName == "") + " / 방 이름 : " + (roomName == ""));

        PhotonManager.pm_instance.RoomSetting(nickName, roomName);
        PhotonManager.pm_instance.CreateRoom();
    }    

    public void JoinRoomButton()
    {
        string nickName = nickNameText.text.Trim().Length >= 2 ? nickNameText.text : "";
        string roomName = "";
        //PhotonManager.pm_instance.RoomSetting(nickName, roomName);
        PhotonManager.pm_instance.JoinRoom();
    }

    public void GameStartButton()
    {
        PhotonManager.pm_instance.GameStart();
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            isPlayScene = true;

            checkUnitPosition = GameObject.Find("CheckUnitPosition");
            readyButton = GameObject.Find("GetReady").GetComponent<Button>();
            resetButton = GameObject.Find("GetWaiting").GetComponent<Button>();
            knightDeathText = GameObject.Find("KnightNum").GetComponent<TextMeshProUGUI>();
            royaltyDeathText = GameObject.Find("RoyaltyNum").GetComponent<TextMeshProUGUI>();

            // 턴 초기화
            turnInfo = GameObject.Find("TurnInfo");
            watingTurn = GameObject.Find("WaitingTurn");
            blueTurn = GameObject.Find("BlueTurn");
            redTurn = GameObject.Find("RedTurn");

            watingTurn.SetActive(true);
            blueTurn.SetActive(false);
            redTurn.SetActive(false);

            // 아이템 관련
            bottomLayer = GameObject.Find("Bottom");
            itemButton01 = GameObject.Find("ItemButton01").GetComponent<Button>();
            itemButton02 = GameObject.Find("ItemButton02").GetComponent<Button>();
            itemButton03 = GameObject.Find("ItemButton03").GetComponent<Button>();
            itemButton04 = GameObject.Find("ItemButton04").GetComponent<Button>();
            itemName = GameObject.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemNum = GameObject.Find("ItemNum").GetComponent<TextMeshProUGUI>();
            itemInfo = GameObject.Find("ItemInfo").GetComponent<TextMeshProUGUI>();
            itemUse = GameObject.Find("ItemUse").GetComponent<Button>();

            checkConfirm = GameObject.Find("CheckConfirm");
            selectUnit00 = GameObject.Find("SelectUnit00");
            selectUnit01 = GameObject.Find("SelectUnit01");
            confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
            cancelButton = GameObject.Find("CancelButton").GetComponent<Button>();

            itemButton01.onClick.AddListener(() => ClickedItemButton(1));
            itemButton02.onClick.AddListener(() => ClickedItemButton(2));
            itemButton03.onClick.AddListener(() => ClickedItemButton(3));
            itemButton04.onClick.AddListener(() => ClickedItemButton(4));

            curClickedItem = ItemNameNum.ChangePosition;
            ClickedItemButton(1);

            bottomLayer.SetActive(false);
            checkConfirm.SetActive(false);

            gameEnd = GameObject.Find("GameEnd");
            endingText = GameObject.Find("EndingText").GetComponent<TextMeshProUGUI>();

            gameEnd.SetActive(false);

            CreatGoalPosition();

            if (readyButton != null)
            {
                readyButton.onClick.AddListener(GetReady);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(GetWaiting);
            }
        }
    }

    public void CheckReadyButton()
    {
        if(!isPlayScene || PlayManager.pm_instance.CheckCurPhase() != PlayPhase.UnitPlacement)
        {
            return;
        }

        //Button readyButton = GameObject.Find("GetReady").GetComponent<Button>();
        Image buttonImage = readyButton.GetComponent<Image>();

        if (UnitManager.um_instance.CheckAllUnitPosition())
        {
            buttonImage.color = new Color(1f, 1f, 1f);
        }
        else
        {
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f);
        }

    }

    public void GetReady()
    {
        if (!UnitManager.um_instance.CheckAllUnitPosition())
        {
            return;
        }

        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.blueReady = true;            
        } else if(PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.redReady = true;
        }

        PhotonManager.pm_instance.GetReady();
    }

    public void GetWaiting()
    {
        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.blueReady = false;
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.redReady = false;
        }

        UnitManager.um_instance.ResetUnitPositionAndCreate();
        PhotonManager.pm_instance.GetWaiting();
    }

    public void HiddingReadyButton()
    {
        checkUnitPosition.SetActive(false);
        watingTurn.SetActive(false);
    }

    public void UncoverItemUI()
    {
        bottomLayer.SetActive(true);
    }

    public void CheckDeathUnit()
    {
        if(knightDeathText == null || royaltyDeathText == null)
        {
            return;
        }

        knightDeathText.text = UnitManager.um_instance.CheckDeathUnitCount(UnitType.Knight).ToString();
        royaltyDeathText.text = UnitManager.um_instance.CheckDeathUnitCount(UnitType.Royalty).ToString();
    }

    public void CreatGoalPosition()
    {
        GameObject leftArrow = Resources.Load<GameObject>("Prefab/UI/Out_Left");
        GameObject rightArrow = Resources.Load<GameObject>("Prefab/UI/Out_Right");        

        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            Vector3 topPosition = new Vector3(-3f, 0.15f, 4.3f);
            Vector3 bottomPosition = new Vector3(3f, 0.15f, -4.65f);

            Quaternion rotation = Quaternion.Euler(90, 0, 0);
            GameObject leftUnit = Instantiate(leftArrow, topPosition, rotation);
            GameObject rightUnit = Instantiate(rightArrow, bottomPosition, rotation);
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            Vector3 topPosition = new Vector3(3f, 0.15f, -4.3f);
            Vector3 bottomPosition = new Vector3(-3f, 0.15f, 4.65f);

            Quaternion rotation = Quaternion.Euler(90, 180, 0);
            GameObject leftUnit = Instantiate(leftArrow, topPosition, rotation);
            GameObject rightUnit = Instantiate(rightArrow, bottomPosition, rotation);
        }
    }

    public void ChangeTurnImage()
    {
        if (!isPlayScene || PlayManager.pm_instance.CheckCurPhase() == PlayPhase.UnitPlacement)
        {
            return;
        }

        PlayerRole type = PlayManager.pm_instance.CheckCurRole();

        if (type == PlayerRole.Master && PlayManager.pm_instance.isMyturn)
        {
            blueTurn.SetActive(true);
            redTurn.SetActive(false);
        } else if (type == PlayerRole.Master && !PlayManager.pm_instance.isMyturn)
        {
            blueTurn.SetActive(false);
            redTurn.SetActive(true);
        }

        if (type == PlayerRole.Participant && PlayManager.pm_instance.isMyturn)
        {
            blueTurn.SetActive(false);
            redTurn.SetActive(true);
        }
        else if (type == PlayerRole.Participant && !PlayManager.pm_instance.isMyturn)
        {
            blueTurn.SetActive(true);
            redTurn.SetActive(false);
        }
    }

    public void ClickedItemButton(int num)
    {
        ItemNameNum temp = (ItemNameNum)num;

        itemUse.onClick.RemoveAllListeners();

        switch (temp)
        {
            case ItemNameNum.ChangePosition:
                itemName.text = "위치 변경";
                itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.ChangePosition).ToString();
                itemInfo.text = "아군 유닛 두 개를 선택하여 위치를 변경합니다.";
                itemUse.onClick.AddListener(() => ClickedItemUseButton((int)ItemNameNum.ChangePosition));
                curClickedItem = ItemNameNum.ChangePosition;
                break; 
            case ItemNameNum.MoveEnemy:
                itemName.text = "상대 유닛 이동";
                itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.MoveEnemy).ToString();
                itemInfo.text = "상대 유닛을 강제로 움직이게 합니다.";
                itemUse.onClick.AddListener(() => ClickedItemUseButton((int)ItemNameNum.MoveEnemy));
                curClickedItem = ItemNameNum.MoveEnemy;
                break; 
            case ItemNameNum.DoubleMove:
                itemName.text = "2칸 이동";
                itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.DoubleMove).ToString();
                itemInfo.text = "두 칸을 이동할 수 있습니다.";
                itemUse.onClick.AddListener(() => ClickedItemUseButton((int)ItemNameNum.DoubleMove));
                curClickedItem = ItemNameNum.DoubleMove;
                break;
            case ItemNameNum.Uncover:
                itemName.text = "정체 확인";
                itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.Uncover).ToString();
                itemInfo.text = "상대 유닛의 정체를 확인할 수 있습니다..";
                itemUse.onClick.AddListener(() => ClickedItemUseButton((int)ItemNameNum.Uncover));
                curClickedItem = ItemNameNum.Uncover;
                break;
            case ItemNameNum.Bomb:
                itemName.text = "폭탄";
                itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.Bomb).ToString();
                itemInfo.text = "폭탄을 밟은 플레이어는 턴이 한 번 스킵됩니다. /n(상대방 유저는 보이지 않습니다.)";
                itemUse.onClick.AddListener(() => ClickedItemUseButton((int)ItemNameNum.Bomb));
                curClickedItem = ItemNameNum.Bomb;
                break; 
        }
    }

    public void ChangeItemNum(ItemNameNum num)
    {
        switch (num)
        {
            case ItemNameNum.ChangePosition:
                if(curClickedItem == ItemNameNum.ChangePosition)
                {
                    itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.ChangePosition).ToString();
                }
                break;
            case ItemNameNum.MoveEnemy:
                if (curClickedItem == ItemNameNum.MoveEnemy)
                {
                    itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.MoveEnemy).ToString();
                }
                break;
            case ItemNameNum.DoubleMove:
                if (curClickedItem == ItemNameNum.DoubleMove)
                {
                    itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.DoubleMove).ToString();
                }
                break;
            case ItemNameNum.Uncover:
                if (curClickedItem == ItemNameNum.Uncover)
                {
                    itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.Uncover).ToString();
                }
                break;
            case ItemNameNum.Bomb:
                if (curClickedItem == ItemNameNum.Bomb)
                {
                    itemNum.text = PlayManager.pm_instance.GetItemNum(ItemNameNum.Bomb).ToString();
                }
                break;
        }
    }

    public void ClickedItemUseButton(int num)
    {
        // 자신턴에만 사용 가능, 아이템은 한턴에 한번만
        if (!PlayManager.pm_instance.isMyturn || PlayManager.pm_instance.isUseItem)
        {
            return;
        }

        ItemNameNum temp = (ItemNameNum)num;

        if (PlayManager.pm_instance.GetItemNum(temp) <= 0)
        {
            return;
        }

        switch (temp)
        {
            case ItemNameNum.ChangePosition:
                PlayManager.pm_instance.isUseItem = true;
                PlayManager.pm_instance.isUseChangePosition = true;
                checkConfirm.SetActive(true);
                selectUnit00.SetActive(true);
                selectUnit01.SetActive(true);
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() => ButtonItemUseConfirm());
                cancelButton.onClick.AddListener(() => ButtonItemUseCancel());
                break;
            case ItemNameNum.MoveEnemy:
                PlayManager.pm_instance.isUseItem = true;
                PlayManager.pm_instance.item02--;                
                PhotonManager.pm_instance.ChangeIsUseMoveEnemy();
                break;
            case ItemNameNum.DoubleMove:
                PlayManager.pm_instance.isUseItem = true;
                PlayManager.pm_instance.item03--;
                PhotonManager.pm_instance.ChangeIsUseDoubleMove();
                break;
            case ItemNameNum.Uncover:
                checkConfirm.SetActive(true);
                selectUnit00.SetActive(true);
                selectUnit01.SetActive(false);
                PlayManager.pm_instance.isUseUncover = true;
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() => ButtonItemUseUncover());
                cancelButton.onClick.AddListener(() => ButtonItemUseCancel());
                break;
            case ItemNameNum.Bomb:
                break;
        }

        ChangeItemNum(temp);
    }

    public void ButtonItemUseConfirm()
    {
        PlayManager.pm_instance.ChangeUnitPosition();
        PlayManager.pm_instance.item01--;
        ChangeItemNum(ItemNameNum.ChangePosition);
    }

    public void ButtonItemUseCancel()
    {
        PlayManager.pm_instance.isUseItem = false;
        checkConfirm.SetActive(false);
        PlayManager.pm_instance.ClearChangeUnit();
    }

    public void ButtonItemUseUncover()
    {
        PlayManager.pm_instance.UncoverUnitHide();
        PlayManager.pm_instance.item04--;
        ChangeItemNum(ItemNameNum.Uncover);
    }

    public void GameEndingText(string text)
    {
        gameEnd.SetActive(true);
        endingText.text = text;

        StartCoroutine(WaitAndLoadScene(5f));
    }

    private IEnumerator WaitAndLoadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonManager.pm_instance.LeaveRoomForAll();
        //SceneManager.LoadScene("MainScene");
    }
}
