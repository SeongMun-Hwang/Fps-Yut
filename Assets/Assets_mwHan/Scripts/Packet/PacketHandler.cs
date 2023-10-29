using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public static void S_StartGameHandler(PacketSession session, IMessage packet)
	{
		S_StartGame startGameHandler = packet as S_StartGame;
		YutGameManager yutmanagerscript = Object.FindObjectOfType<YutGameManager>();

	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo player in spawnPacket.Objects)
		{
			Managers.Object.Add(player, myPlayer: false);
			Debug.Log(player.ObjectId);
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

	public static void S_ThrowYutHandler(PacketSession session, IMessage packet)
	{
		S_ThrowYut throwYutHandler = packet as S_ThrowYut;
		stone stoneScript = Object.FindObjectOfType<stone>();
		stoneScript.HandleThrowYut(throwYutHandler.Result);

	}

	public static void S_YutMoveHandler(PacketSession session, IMessage packet)
	{
		S_YutMove yutmovePacket = packet as S_YutMove;
		stone stoneScript = Object.FindObjectOfType<stone>();
		UI uiScript = Object.FindObjectOfType<UI>();
		YutGameManager yutmanagerscript = Object.FindObjectOfType<YutGameManager>();

		uiScript.choose_step = yutmovePacket.UseResult;
		yutmanagerscript.player_number = yutmovePacket.MovedYut;

		stoneScript.handleMovePlayer();
	}

		public static void S_HorseCatchHandler(PacketSession session, IMessage packet)
    {
		S_HorseCatch horsecatchPacket = packet as S_HorseCatch;
		Managers.Object.MyPlayer._playtime = horsecatchPacket.Playtime;
		Debug.Log("horsecatch");
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
		MyPlayerController pc = Object.FindObjectOfType<MyPlayerController>();

		if (pc != null)
        {
			Debug.Log("Die");
			pc.GameEnd(diePacket.ObjectId, diePacket.Timeset);
			return;
        }
	}

	public static void S_SelectWallHandler(PacketSession session, IMessage packet)
    {
		S_SelectWall wallPacket = packet as S_SelectWall;
		move_Pillar mp = Object.FindObjectOfType<move_Pillar>();

		if(mp != null)
        {
			mp.movepillar(wallPacket.Selectwall);
			return;
		}
    }

	public static void S_AttackWallHandler(PacketSession session, IMessage packet)
	{
		move_Pillar mp = Object.FindObjectOfType<move_Pillar>();

		if (mp != null)
		{
			mp.attackWall();
			return;
		}
	}
}