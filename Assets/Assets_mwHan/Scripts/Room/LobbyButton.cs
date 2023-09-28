using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyButton : MonoBehaviour
{
    public GameObject buttonPrefab; // 버튼을 생성할 프리팹

    public Transform buttonParent;   // 버튼을 생성할 부모 객체 (보통 Panel)

    public TMP_InputField roomNameInput;

    private string roomName = null;


    private static List<GameObject> buttons = new List<GameObject>();

    private void Awake()
    {
        roomName = roomNameInput.GetComponent<TMP_InputField>().text;
    }

    private void Update()
    {
        //키보드
        if (roomName.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            CreateRoom();
        }
    }

    //마우스
    public void CreateRoom()
    {
        roomName = roomNameInput.text;
        if (roomName.Length <= 0)
            roomName = "default room name";

        C_MakeRoom makeRoomPacket = new C_MakeRoom();
        makeRoomPacket.RoomName = roomName;
        Managers.Network.Send(makeRoomPacket);

        Debug.Log(roomName);
    }

    public void GetRoomList()
    {
        C_RoomList roomlistPacket = new C_RoomList();
        Managers.Network.Send(roomlistPacket);
    }

    // 버튼 생성 함수
    public void DrawRoomButtons(S_RoomList roomInfos)
    {
        ClearButtons();

        if (buttonParent == null)
        {
            Debug.Log("너 왜 null임?");
            return;
        }

        foreach (RoomInfo roomInfo in roomInfos.RoomInfos)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();

            button.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Roomname;

            button.onClick.AddListener(() => OnButtonClick(roomInfo)); // 클릭 이벤트 리스너 등록
            buttons.Add(buttonGO);
        }
    }

    // 생성된 버튼들을 삭제하는 함수
    private void ClearButtons()
    {
        foreach (GameObject buttonGO in buttons)
        {
            Destroy(buttonGO);
        }

        buttons.Clear();
    }

    // 버튼이 클릭되었을 때 호출되는 함수
    private void OnButtonClick(RoomInfo roomInfo)
    {
        // 클릭된 버튼에 대한 동작 구현
        Debug.Log("Clicked on room: " + roomInfo.Roomname);
        Debug.Log("Room Id : " + roomInfo.RoomId);
    }
}
