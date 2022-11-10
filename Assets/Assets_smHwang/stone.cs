using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour
{
    public Yut_Field currentRoute;
    int routePosition;
    int nowPosition = 0;
    int lastPosition = 0;
    public int steps;
    bool isMoving;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            steps = Random.Range(1, 6);
            switch (steps)
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
    }

    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        lastPosition = routePosition;
        

        while (steps > 0)
        {
            if (routePosition == 30 && steps > 0)
            {
                Debug.Log("Goal");
                Destroy(gameObject);
                break;
            }
            //지름길->3번째 코너

            nowPosition = routePosition;
            routePosition++;
            //routePosition %= currentRoute.childNodeList.Count;

            //Debug.Log("nowposition: " + nowPosition);
            //Debug.Log("routeposition: " + routePosition);
            //Debug.Log("lastposition: " + lastPosition);

            if (lastPosition == 5 && nowPosition==5)
            {
                routePosition = 20;
            }
            else if (lastPosition == 10 && nowPosition == 10)
            {
                routePosition = 25;
            }
            else if (lastPosition == 22 && nowPosition == 22) //center
            {
                routePosition = 28;
            }
            else if (lastPosition == 24 && nowPosition == 24)
            {
                routePosition = 15;
            }
            if (nowPosition == 24 && steps >= 1)
            {
                routePosition = 15;

            }
            if ((lastPosition >= 15 && lastPosition <= 19) || (lastPosition >= 28 && lastPosition <= 29))
            {
                if ((nowPosition==19||nowPosition==29)&&steps>=1)
                {
                    routePosition = 30;
                }
            }
                

            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos)) { yield return null; }


            
            yield return new WaitForSeconds(0.1f);
            steps--;

            if (routePosition == 30 && steps>1)
            {
                Debug.Log("break");
                Debug.Log("Goal");
                Destroy(gameObject);
                break;
            }
        }

        
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 8f * Time.deltaTime));
    } 
}
