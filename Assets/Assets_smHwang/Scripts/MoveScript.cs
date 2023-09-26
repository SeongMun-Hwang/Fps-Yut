using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private const int FIRST_CORNER = 5;
    private const int SECOND_CORNER = 10;
    private const int THIRD_CORNER = 15;
    private const int CENTER = 22;
    private const int GOAL = 30;
    private const int FIRST_FORK = 20;
    private const int SECOND_FORK = 25;
    private const int CENTER_STRAIGHT = 24;
    private const int CENTER_FORK = 28;

    private user[][] users;
    private int turn;
    private int player_number;

    public void GetData(int t, int p_num, user[][] u)
    {
        turn = t;
        player_number = p_num;
        users = u;
    }
    void Start()
    {
        turn = YutGameManager.Instance.GetTurn();
        player_number = YutGameManager.Instance.GetPlayerNumber();
    }
    //백도 이동 예외 처리
    public void BackdoRoute()
    {
        if (users[turn][player_number].nowPosition == FIRST_FORK)
        {
            users[turn][player_number].routePosition = FIRST_CORNER;
        }
        else if (users[turn][player_number].nowPosition == SECOND_FORK)
        {
            users[turn][player_number].routePosition = SECOND_CORNER;
        }
        else if (users[turn][player_number].nowPosition == THIRD_CORNER && users[turn][player_number].lastPosition == CENTER_STRAIGHT)
        {
            users[turn][player_number].routePosition = CENTER_STRAIGHT;
        }
        else if (users[turn][player_number].nowPosition == 1)
        {
            users[turn][player_number].routePosition = GOAL;
        }
        else
        {
            users[turn][player_number].routePosition = users[turn][player_number].nowPosition - 1;
        }
        users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
    }
    //정상 이동 예외처리
    public int NormalRoute(int LeftStep)
    {
        //if(users[turn][player_number].lastPosition== users[turn][player_number].nowPosition)
        //{
        //    switch (users[turn][player_number].nowPosition)
        //    {
        //        case FIRST_CORNER:
        //            users[turn][player_number].routePosition = FIRST_FORK;
        //            break;
        //        case SECOND_CORNER:
        //            users[turn][player_number].routePosition = SECOND_FORK;
        //            break;
        //        case CENTER:
        //            users[turn][player_number].routePosition = CENTER_FORK;
        //            break;
        //    }
        //}
        if (users[turn][player_number].nowPosition == FIRST_CORNER && users[turn][player_number].lastPosition == FIRST_CORNER)
        {
            return FIRST_FORK;
        }
        else if (users[turn][player_number].nowPosition == SECOND_CORNER && users[turn][player_number].lastPosition == SECOND_CORNER)
        {
            return SECOND_FORK;
        }
        else if (users[turn][player_number].nowPosition == CENTER && users[turn][player_number].lastPosition==CENTER) //center
        {
            return CENTER_FORK;
        }
        else if (users[turn][player_number].nowPosition==24)
        {
            return THIRD_CORNER;
        }
        else if (users[turn][player_number].nowPosition == 29)
        {
            return GOAL;
        }
        else if (users[turn][player_number].nowPosition == 19)
        {
            return GOAL;
        }
        else
        {
            return -1;
        }
    }
    //플레이어 이동함수
    public bool MoveToNextNode(Vector3 goal)
    {
        if (users[turn][player_number].player != null)
        {
            // 현재 위치에서 x, z 값을 가져오고 y 값은 변경하지 않습니다.
            Vector3 startPos = users[turn][player_number].player.transform.position;
            Vector3 newGoal = new Vector3(goal.x, startPos.y, goal.z);

            bool isMovingPlayer = (newGoal - startPos).sqrMagnitude > 0.001f * 0.001f;
            if (isMovingPlayer)
            {
                users[turn][player_number].player.transform.position = Vector3.MoveTowards(startPos, newGoal, 8f * Time.deltaTime);
            }
            //업은 말이면 함께 이동
            if (users[turn][player_number].is_bind)
            {
                foreach (int bindedIndex in users[turn][player_number].BindedHorse)
                {
                    if (users[turn][bindedIndex].player != null)
                    {
                        Vector3 bindedStartPos = users[turn][bindedIndex].player.transform.position;
                        Vector3 bindedGoal = new Vector3(goal.x, bindedStartPos.y, goal.z);
                        users[turn][bindedIndex].player.transform.position = Vector3.MoveTowards(bindedStartPos, bindedGoal, 8f * Time.deltaTime);
                    }
                }
            }
            return isMovingPlayer;
        }
        return false;
    }
}
