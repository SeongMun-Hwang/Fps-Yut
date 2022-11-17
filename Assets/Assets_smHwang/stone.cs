using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour
{
    public GameObject[] player;
    public Yut_Field currentRoute;
    int[] routePosition = new int[2];
    int[] nowPosition = new int[2];
    int[] lastPosition = new int[2];
    int[] steps = new int[2];
    bool isMoving;
    int turn = 0;

    public void throwYut()
    {
        steps[turn] = Random.Range(1, 6);
        switch (steps[turn])
        {
            case 1:
                Debug.Log("도!");
                break;
            case 2:
                Debug.Log("개!");
                break;
            case 3:
                Debug.Log("걸!");
                break;
            case 4:
                Debug.Log("윷!");
                break;
            case 5:
                Debug.Log("모!");
                break;
        }
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        lastPosition[turn] = routePosition[turn];


        while (steps[turn] > 0)
        {
            if (routePosition[turn] == 30 && steps[turn] > 0)
            {
                Debug.Log("Goal");
                Destroy(player[turn]);
                break;
            }
            //지름길->3번째 코너

            nowPosition[turn] = routePosition[turn];
            routePosition[turn]++;
            if (lastPosition[turn] == 5 && nowPosition[turn] == 5)
            {
                routePosition[turn] = 20;
            }
            else if (lastPosition[turn] == 10 && nowPosition[turn] == 10)
            {
                routePosition[turn] = 25;
            }
            else if (lastPosition[turn] == 22 && nowPosition[turn] == 22) //center
            {
                routePosition[turn] = 28;
            }
            else if (lastPosition[turn] == 24 && nowPosition[turn] == 24)
            {
                routePosition[turn] = 15;
            }
            if (nowPosition[turn] == 24 && steps[turn] >= 1)
            {
                routePosition[turn] = 15;

            }
            if ((lastPosition[turn] >= 15 && lastPosition[turn] <= 19) || (lastPosition[turn] >= 28 && lastPosition[turn] <= 29))
            {
                if ((nowPosition[turn] == 19 || nowPosition[turn] == 29) && steps[turn] >= 1)
                {
                    routePosition[turn] = 30;
                }
            }

            Vector3[] nextPos = new Vector3[2];
            nextPos[turn] = currentRoute.childNodeList[routePosition[turn]].position;
            while (MoveToNextNode(nextPos[turn])) { yield return null; }



            yield return new WaitForSeconds(0.1f);
            steps[turn]--;

            if (routePosition[turn] == 30 && steps[turn] > 1)
            {
                Debug.Log("break");
                Debug.Log("Goal");
                Destroy(player[turn]);
                break;
            }
        }


        isMoving = false;
        if (turn == 0) { turn = 1; }
        else if (turn == 1) { turn = 0; }
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (player[turn].transform.position = Vector3.MoveTowards(player[turn].transform.position, goal, 8f * Time.deltaTime));
    }
}
