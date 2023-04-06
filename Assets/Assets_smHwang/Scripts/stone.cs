using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class stone : MonoBehaviour
{
    public GameObject[] player;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;
    public TextMeshProUGUI Yut;
    public GameObject[] red_team;
    public GameObject[] blue_team;

    public struct user
    {
        public GameObject[] player;
        public int routePosition;
        public int nowPosition;
        public int lastPosition;      
        public int steps;
        public Vector3 nextPos;
    };

    bool isMoving;
    int turn = 0;
    int sum = 0;
    int chance = 0;
    float time;
    bool isFight = false;

    user[] users = new user[2];
    //users[0].player = new GameObject[4];
    void Start()
    {
        if (isFight == false)
        {
            users[0].player = red_team[0];
            users[1].player = blue_team[1];
            //for (int i = 0; i < 4; i++)
            //{
            //    users[0].player[i] = red_team[i];
            //    users[1].player[i] = blue_team[i];
            //}
        }
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
    }

    public void throwYut()
    {
        int[] yut = new int[4];
        for (int i = 0; i < 4; i++)
        {
            yut[i] = Random.Range(0, 2);
            sum += yut[i];
        }
        switch (sum)
        {
            case 0:
                users[turn].steps = 5;
                chance = 1;
                Yut.text = "모!";
                break;
            case 1:
                if (yut[3] == 1)
                {
                    users[turn].steps = -1;
                    Yut.text = "back-do!";
                    break;
                }
                users[turn].steps = 1;
                Yut.text = "도!";
                break;
            case 2:
                users[turn].steps = 2;
                Yut.text = "개!";
                break;
            case 3:
                users[turn].steps = 3;
                Yut.text = "걸!";
                break;
            case 4:
                users[turn].steps = 4;
                chance = 1;
                Yut.text = "윷!";
                break;
        }
        yutSound[sum].mute = false;
        yutSound[sum].Play();
        StartCoroutine(Move());
    }

    public void throw_do()
    {
        users[turn].steps = 1;
        StartCoroutine(Move());
    }
    public void throw_back_do()
    {
        users[turn].steps = -1;
        StartCoroutine(Move());
    }
    public void throw_gae()
    {
        users[turn].steps = 2;
        StartCoroutine(Move());
    }
    public void throw_girl()
    {
        users[turn].steps = 3;
        StartCoroutine(Move());
    }
    public void throw_yut()
    {
        users[turn].steps = 4;
        StartCoroutine(Move());
    }
    public void throw_mo()
    {
        users[turn].steps = 5;
        StartCoroutine(Move());
    }


    IEnumerator Move()
    {
        yield return new WaitForSeconds(1f);
        Yut.text = "";
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        users[turn].lastPosition = users[turn].routePosition;


        while (users[turn].steps > 0)
        {            
            if (users[turn].routePosition == 30 && users[turn].steps > 0)
            {
                Debug.Log("Goal");
                Destroy(player[turn]);
                break;
            }

            users[turn].nowPosition = users[turn].routePosition;
            users[turn].routePosition++;

            if (users[turn].lastPosition == 5 && users[turn].nowPosition == 5)
            {
                users[turn].routePosition = 20;
            }
            else if (users[turn].lastPosition == 10 && users[turn].nowPosition == 10)
            {
                users[turn].routePosition = 25;
            }
            else if (users[turn].lastPosition == 22 && users[turn].nowPosition == 22) //center
            {
                users[turn].routePosition = 28;
            }
            else if (users[turn].lastPosition == 24 && users[turn].nowPosition == 24)
            {
                users[turn].routePosition = 15;
            }
            if (users[turn].nowPosition == 24 && users[turn].steps >= 1)
            {
                users[turn].routePosition = 15;

            }
            if ((users[turn].lastPosition >= 15 && users[turn].lastPosition <= 19) || (users[turn].lastPosition >= 28 && users[turn].lastPosition <= 29))
            {
                if ((users[turn].nowPosition == 19 || users[turn].nowPosition == 29) && users[turn].steps >= 1)
                {
                    users[turn].routePosition = 30;
                }
            }

            users[turn].nextPos = currentRoute.childNodeList[users[turn].routePosition].position;
            while (MoveToNextNode(users[turn].nextPos)) { yield return null; }

            //start fps fight if position same
            if ((users[0].nowPosition == users[1].nowPosition) && users[1].routePosition != 0)
            {
                Yut.text = "Encounter!!";
                isFight = true;
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("Fpsfight");
            }
            yield return new WaitForSeconds(0.1f);
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
           
            users[turn].steps--;
        }

        isMoving = false;
        sum = 0;
        //윷, 모 나왔을 시 턴 유지, 아니면 변경
        if (chance == 1)
        {
            chance--;
        }
        else {
            if (turn == 0) { turn = 1; }
            else if (turn == 1) { turn = 0; }
            Yut.text = "player " + (turn+1) + " turn!";
        }        
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (player[turn].transform.position = Vector3.MoveTowards(player[turn].transform.position, goal, 8f * Time.deltaTime));
    }
}
