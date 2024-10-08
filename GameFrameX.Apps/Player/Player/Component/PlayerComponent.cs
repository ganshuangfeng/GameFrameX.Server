﻿using System.Threading.Tasks;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Monitor.Player;

namespace GameFrameX.Apps.Player.Player.Component
{
    [ComponentType(ActorType.Player)]
    public sealed class PlayerComponent : StateComponent<PlayerState>
    {
        public async Task<PlayerState> OnPlayerLogin(ReqPlayerLogin reqLogin)
        {
            MetricsPlayerRegister.LoginCounterOptions.Inc();
            return await GameDb.FindAsync<PlayerState>(m => m.Id == reqLogin.Id);
        }
    }
}