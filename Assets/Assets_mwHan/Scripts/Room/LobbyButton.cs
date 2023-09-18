using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyButton : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    private string roomName = null;

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
}
