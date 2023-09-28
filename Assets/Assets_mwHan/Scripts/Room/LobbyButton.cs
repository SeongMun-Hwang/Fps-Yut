using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyButton : MonoBehaviour
{
    public GameObject buttonPrefab; // ��ư�� ������ ������

    public Transform buttonParent;   // ��ư�� ������ �θ� ��ü (���� Panel)

    public TMP_InputField roomNameInput;

    private string roomName = null;


    private static List<GameObject> buttons = new List<GameObject>();

    private void Awake()
    {
        roomName = roomNameInput.GetComponent<TMP_InputField>().text;
    }

    private void Update()
    {
        //Ű����
        if (roomName.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            CreateRoom();
        }
    }

    //���콺
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

    // ��ư ���� �Լ�
    public void DrawRoomButtons(S_RoomList roomInfos)
    {
        ClearButtons();

        if (buttonParent == null)
        {
            Debug.Log("�� �� null��?");
            return;
        }

        foreach (RoomInfo roomInfo in roomInfos.RoomInfos)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();

            button.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Roomname;

            button.onClick.AddListener(() => OnButtonClick(roomInfo)); // Ŭ�� �̺�Ʈ ������ ���
            buttons.Add(buttonGO);
        }
    }

    // ������ ��ư���� �����ϴ� �Լ�
    private void ClearButtons()
    {
        foreach (GameObject buttonGO in buttons)
        {
            Destroy(buttonGO);
        }

        buttons.Clear();
    }

    // ��ư�� Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ�
    private void OnButtonClick(RoomInfo roomInfo)
    {
        // Ŭ���� ��ư�� ���� ���� ����
        Debug.Log("Clicked on room: " + roomInfo.Roomname);
        Debug.Log("Room Id : " + roomInfo.RoomId);
    }
}
