using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_RoomListHandler(PacketSession session, IMessage packet)
	{
		S_RoomList roomListPacket = packet as S_RoomList;

		// Scene에서 LobbyButton GameObject를 찾아서 사용
		LobbyButton lobbyButton = Object.FindObjectOfType<LobbyButton>();

		if (lobbyButton != null)
		{
			lobbyButton.DrawRoomButtons(roomListPacket);
		}
		else
		{
			Debug.LogError("LobbyButton not found in the scene.");
		}
	}

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.RemoveMyPlayer();
	}

	public static void S_ThrowYutHandler(PacketSession session, IMessage packet)
	{
		S_ThrowYut throwYutHandler = packet as S_ThrowYut;
	}

	public static void S_StartGameHandler(PacketSession session, IMessage packet)
	{
		S_StartGame startGameHandler = packet as S_StartGame;
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo player in spawnPacket.Objects)
		{
			Managers.Object.Add(player, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
		{
			return;
		}

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null)
		{
			return;
		}

		pc.PosInfo = movePacket.PosInfo;
	}

	public static void S_RotationHandler(PacketSession session, IMessage packet)
	{
		S_Rotation rotationPacket = packet as S_Rotation;

		GameObject go = Managers.Object.FindById(rotationPacket.ObjectId);
		if (go == null)
		{
			Debug.Log("FindById 실패");
			return;
		}

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null)
		{
			return;
		}

		pc.RotInfo = rotationPacket.RotInfo;
	}

	public static void S_DoAttackHandler(PacketSession session, IMessage packet)
    {
		S_DoAttack atkPacket = packet as S_DoAttack;

		GameObject go = Managers.Object.FindById(atkPacket.ObjectId);
		if (go == null)
		{
			Debug.Log("FindById 실패");
			return;
		}

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
		{
			pc.doAttack = true;
			return;
		}
	}

	public static void S_PlayerAttackedHandler(PacketSession session, IMessage packet)
	{
		S_PlayerAttacked attackedPacket = packet as S_PlayerAttacked;

		GameObject go = Managers.Object.FindById(attackedPacket.ObjectId);
		if (go == null)
		{
			Debug.Log("FindById 실패");
			return;
		}

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
		{
			pc.PlayerAttacked(attackedPacket.AttackedDirection, attackedPacket.AttackForce);
			return;
		}
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
    {
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
		{
			Debug.Log("FindById 실패");
			return;
		}

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
        {
			Debug.Log("Die");
			return;
        }
	}
}