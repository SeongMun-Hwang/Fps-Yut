using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stone : MonoBehaviour
{
    public GameObject[] player;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;

    public struct user
    {
        public GameObject player;
        public int routePosition;
        public int nowPosition;
        public int lastPosition;      
        public int steps;
    };

    bool isMoving;
    int turn = 0;
    int sum;
    int chance = 0;
    public Text Yut;
    float time;

    user[] users = new user[2];

    void Start()
    {
        users[0].player = player[0];
        users[1].player = player[1];
        for(int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
    }

    public void Update()
    {
        if (Input.GetKey("space"))
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
            int sound = Random.Range(0, 5);
            yutSound[sound].mute = false;
            yutSound[sound].Play();
            StartCoroutine(Move());
        }   
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
            //포지션이 같으면 fps 전투로 이동
            //if ((users[0].nowPosition == users[1].nowPosition)&& users[0].nowPosition!=0)
            //{
            //    Yut.text = "Encounter!!";
            //    yield return new WaitForSeconds(1f);
            //    SceneManager.LoadScene("Fpsfight");
            //}
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

            Vector3[] nextPos = new Vector3[2];
            nextPos[turn] = currentRoute.childNodeList[users[turn].routePosition].position;
            while (MoveToNextNode(nextPos[turn])) { yield return null; }

            yield return new WaitForSeconds(0.1f);
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
