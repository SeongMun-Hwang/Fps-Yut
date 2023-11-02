using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class YutText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public static int result = 0;
    public static bool isbackdo = false;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (YutCheck.isAdd.Any(add => !add)) // isAdd 배열에 false가 하나라도 있으면
        {
            text.text = "낙";
        }
        else // 모든 값이 true일 때
        {
            switch (result)
            {
                case 0:
                    text.text = "모";
                    break;
                case 1:
                    text.text = isbackdo ? "백도" : "도";
                    break;
                case 2:
                    text.text = "개";
                    break;
                case 3:
                    text.text = "걸";
                    break;
                case 4:
                    text.text = "윷";
                    break;
                default:
                    // 필요한 경우, 기본 텍스트나 에러 메시지를 여기에 추가
                    break;
            }

            C_ThreedYutThrow throwPacket = new C_ThreedYutThrow();
            throwPacket.Result = result;
            throwPacket.Isbackdo = isbackdo;
            //Managers.Network.Send(throwPacket);
        }
    }
}