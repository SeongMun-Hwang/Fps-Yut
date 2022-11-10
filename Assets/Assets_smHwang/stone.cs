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
            steps = Random.Range(3, 5);
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
                    steps++;
                    Debug.Log("모!");
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
        if (lastPosition == 5)
        {
            routePosition = 20;
            steps--;
        }
        else if (lastPosition == 10)
        {
            routePosition = 25;
            steps--;
        }
        else if (lastPosition == 22)
        {
            routePosition = 27;
            steps--;
        }
        else if (lastPosition == 23)
        {
            routePosition = 15;
            steps -= 2;
        }
        else if (lastPosition == 24)
        {
            routePosition = 15;
            steps--;
        }
        while (steps > 0)
        {
            routePosition++;
            routePosition %= currentRoute.childNodeList.Count;
           
            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            steps--;
            
            if (routePosition == 0 && lastPosition != 0)
            {
                Debug.Log("break");
                break;
            }
        }

        nowPosition = routePosition;
        Debug.Log("nowposition: "+nowPosition);
        Debug.Log("routeposition: "+routePosition);
        Debug.Log("lastposition: "+lastPosition);

       
        if (routePosition == 0 && lastPosition != 0)
        {
            Debug.Log("Goal");
            Destroy(gameObject);
        }
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 8f * Time.deltaTime));
    } 
}
