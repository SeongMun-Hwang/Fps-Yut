using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct user
{
    public GameObject player;
    public int routePosition;
    public int nowPosition;
    public int lastPosition;
    public List<int> FinalPosition;
    public Vector3 nextPos;
    public Vector3 player_start_position;
    public bool goal;
    public bool is_bind;
    public List<int> BindedHorse;
}