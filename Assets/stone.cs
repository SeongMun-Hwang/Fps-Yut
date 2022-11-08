using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour
{
    public Yut_Field currentRoute;
    int routePosition;
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
                    Debug.Log("��! ");
                    break;
                case 2:
                    Debug.Log("��! ");
                    break;
                case 3:
                    Debug.Log("��! ");
                    break;
                case 4:
                    Debug.Log("��! ");
                    break;
                case 5:
                    Debug.Log("��! ");
                    break;
            }
            

            if (routePosition + steps < currentRoute.childNodeList.Count)
            {
                StartCoroutine(Move());
            }
            else
            {
                Debug.Log("threwed number is to high");
            }
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
            Vector3 nextPos = currentRoute.childNodeList[routePosition + 1].position;
            while (MoveToNextNode(nextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            steps--;
            routePosition++;
        }
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    } 
}
