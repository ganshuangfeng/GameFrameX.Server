﻿using System.Threading.Tasks;
using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Monitor.Account;
using GameFrameX.Monitor.Player;
using Random = GameFrameX.Utility.Random;

namespace GameFrameX.Apps.Account.Login.Component
{
    [ComponentType(ActorType.Account)]
    public sealed class LoginComponent : StateComponent<LoginState>
    {
        public async Task<LoginState> OnLogin(ReqLogin reqLogin)
        {
            MetricsAccountRegister.LoginCounterOptions.Inc();
            return await GameDb.FindAsync<LoginState>(m => m.UserName == reqLogin.UserName && m.Password == reqLogin.Password);
        }

        public async Task<LoginState> Register(long accountId, ReqLogin reqLogin)
        {
            MetricsAccountRegister.RegisterCounterOptions.Inc();
            LoginState loginState = new LoginState() { Id = accountId, UserName = reqLogin.UserName, Password = reqLogin.Password };
            await GameDb.SaveOneAsync<LoginState>(loginState);
            return loginState;
        }

        public async Task<List<PlayerState>> GetPlayerList(ReqPlayerList reqPlayerList)
        {
            MetricsPlayerRegister.GetPlayerListCounterOptions.Inc();
            return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
        }

        public async Task<PlayerState> OnPlayerCreate(ReqPlayerCreate reqPlayerCreate)
        {
            PlayerState playerState = new PlayerState
            {
                Id = ActorIdGenerator.GetActorId(ActorType.Player),
                AccountId = reqPlayerCreate.Id,
                Name = reqPlayerCreate.Name,
                Level = (uint)Random.GetRandom(1, 50),
                State = 0,
                Avatar = (uint)Random.GetRandom(1, 50),
            };
            MetricsPlayerRegister.CreateCounterOptions.Inc();
            await GameDb.SaveOneAsync<PlayerState>(playerState);
            return playerState;
        }


    }
}