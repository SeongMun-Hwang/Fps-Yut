using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class user
{
    public List<horse> horses;
    public int turn;
    public Color DestinationColor;
}
public class horse
{
    public user Owner { get; set; }
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