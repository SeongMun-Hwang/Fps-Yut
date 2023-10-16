using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

        SceneManager.LoadScene("YutPlay");

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

        for (int i = 0; i < roomInfos.RoomInfos.Count; i++)
        {
            Debug.Log("id : " + roomInfos.RoomInfos[i].RoomId);
            Debug.Log("name : " + roomInfos.RoomInfos[i].Roomname);
        }


        for (int i = 0; i < roomInfos.RoomInfos.Count; i++)
        {
            RoomInfo room = roomInfos.RoomInfos[i];
            GameObject buttonGO = Instantiate(buttonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();

            button.GetComponentInChildren<TextMeshProUGUI>().text = room.Roomname;

            // Ŭ�� �̺�Ʈ ������ ���
            button.onClick.AddListener(() =>
            {
                EnterRoomButton(room);
            });

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
    private void EnterRoomButton(RoomInfo roomInfo)
    {
        // Ŭ���� ��ư�� ���� ���� ����
        Debug.Log("Clicked on room: " + roomInfo.Roomname);
        Debug.Log("Room Id : " + roomInfo.RoomId);

        C_EnterRoom enterroomPacket = new C_EnterRoom();
        enterroomPacket.RoomId = roomInfo.RoomId;
        Managers.Network.Send(enterroomPacket);

        SceneManager.LoadScene("YutPlay");
    }
}
