﻿using GameFrameX.Hotfix.Server.Heart.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;

namespace GameFrameX.Hotfix.Server.Heart.Handler;

/// <summary>
/// 心跳消息处理器
/// </summary>
[MessageMapping(typeof(ReqHeartBeat))]
internal sealed class ReqHeartBeatHandler : GlobalComponentHandler<HeartBeatComponentAgent>
{
    readonly NotifyHeartBeat resp = new NotifyHeartBeat
    {
        Timestamp = TimeHelper.UnixTimeMilliseconds()
    };

    protected override async Task ActionAsync()
    {
        ReqHeartBeat req = this.Message as ReqHeartBeat;
        // LogHelper.Info("收到心跳请求:" + req.Timestamp);
        Comp.OnUpdateHeartBeatTime(req);
        Channel.UpdateReceiveMessageTime();
        resp.Timestamp = TimeHelper.UnixTimeMilliseconds();
        resp.UniqueId = req.UniqueId;
        await Channel.WriteAsync(resp);
    }
}