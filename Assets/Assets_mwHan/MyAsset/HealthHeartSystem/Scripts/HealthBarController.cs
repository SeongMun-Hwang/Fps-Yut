using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private GameObject[] heartContainers;
    private Image[] heartFills;

    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    public GameObject player;

    private void Start()
    {
        // Should I use lists? Maybe :)
        heartContainers = new GameObject[(int)player.GetComponent<MyPlayerController>().MaxHp];
        heartFills = new Image[(int)player.GetComponent<MyPlayerController>().MaxHp];

        player.GetComponent<MyPlayerController>().onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    public void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < player.GetComponent<MyPlayerController>().MaxHp)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < player.GetComponent<MyPlayerController>().Hp)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }

        if (player.GetComponent<MyPlayerController>().Hp % 1 != 0)
        {
            int lastPos = Mathf.FloorToInt(player.GetComponent<MyPlayerController>().Hp);
            heartFills[lastPos].fillAmount = player.GetComponent<MyPlayerController>().Hp % 1;
        }
    }

    void InstantiateHeartContainers()
    {
        for (int i = 0; i < player.GetComponent<MyPlayerController>().MaxHp; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
}
