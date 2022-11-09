using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour
{
    public Yut_Field currentRoute;
    int routePosition;
    int lastPosition=0;
    public int steps;
    bool isMoving;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            steps = Random.Range(2, 4);
            switch (steps)
            {
                case 1:
                    Debug.Log("µµ!");
                    break;
                case 2:
                    Debug.Log("°³!");
                    break;
                case 3:
                    Debug.Log("°É!");
                    break;
                case 4:
                    Debug.Log("À·!");
                    break;
                case 5:
                    Debug.Log("¸ð!");
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

        while (steps > 0)
        {
            routePosition++;
            if (lastPosition == 5) //first corner
            {
                routePosition = 21;
                lastPosition = 21;
                steps--;
            }
            else if (lastPosition == 22) //center
            {
                routePosition = 27;
                lastPosition = 27;
                steps--;
            }
            else if (lastPosition == 10) //second corner
            {
                routePosition = 25;
                lastPosition = 25;
                steps--;
            }
            else if (lastPosition == 24) //passed center
            {
                routePosition = 15;
                lastPosition = 15;
                steps--;
            }
            else if (lastPosition == 28 | lastPosition == 19) //long route goal
            {
                routePosition = 0;
                lastPosition = 0;
                steps--;
            }

            routePosition %= currentRoute.childNodeList.Count;

            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            steps--;
        }
        lastPosition = routePosition;
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 8f * Time.deltaTime));
    } 
}
