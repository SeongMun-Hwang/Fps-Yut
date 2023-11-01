using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SRoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)MsgId.SRoomList, PacketHandler.S_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SStartGame, MakePacket<S_StartGame>);
		_handler.Add((ushort)MsgId.SStartGame, PacketHandler.S_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.SThrowYut, MakePacket<S_ThrowYut>);
		_handler.Add((ushort)MsgId.SThrowYut, PacketHandler.S_ThrowYutHandler);		
		_onRecv.Add((ushort)MsgId.SYutMove, MakePacket<S_YutMove>);
		_handler.Add((ushort)MsgId.SYutMove, PacketHandler.S_YutMoveHandler);		
		_onRecv.Add((ushort)MsgId.SHorseCatch, MakePacket<S_HorseCatch>);
		_handler.Add((ushort)MsgId.SHorseCatch, PacketHandler.S_HorseCatchHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SRotation, MakePacket<S_Rotation>);
		_handler.Add((ushort)MsgId.SRotation, PacketHandler.S_RotationHandler);		
		_onRecv.Add((ushort)MsgId.SDoAttack, MakePacket<S_DoAttack>);
		_handler.Add((ushort)MsgId.SDoAttack, PacketHandler.S_DoAttackHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerAttacked, MakePacket<S_PlayerAttacked>);
		_handler.Add((ushort)MsgId.SPlayerAttacked, PacketHandler.S_PlayerAttackedHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SUpdateRound, MakePacket<S_UpdateRound>);
		_handler.Add((ushort)MsgId.SUpdateRound, PacketHandler.S_UpdateRoundHandler);		
		_onRecv.Add((ushort)MsgId.SSelectWall, MakePacket<S_SelectWall>);
		_handler.Add((ushort)MsgId.SSelectWall, PacketHandler.S_SelectWallHandler);		
		_onRecv.Add((ushort)MsgId.SAttackWall, MakePacket<S_AttackWall>);
		_handler.Add((ushort)MsgId.SAttackWall, PacketHandler.S_AttackWallHandler);		
		_onRecv.Add((ushort)MsgId.SDefMove, MakePacket<S_DefMove>);
		_handler.Add((ushort)MsgId.SDefMove, PacketHandler.S_DefMoveHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerCollision, MakePacket<S_PlayerCollision>);
		_handler.Add((ushort)MsgId.SPlayerCollision, PacketHandler.S_PlayerCollisionHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}