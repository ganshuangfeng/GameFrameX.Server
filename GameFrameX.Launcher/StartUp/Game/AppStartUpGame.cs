﻿namespace GameFrameX.Launcher.StartUp.Game
{
    /// <summary>
    /// 游戏服务器
    /// </summary>
    [StartUpTag(ServerType.Game)]
    internal sealed class AppStartUpGame : AppStartUpBase
    {
        public override async Task EnterAsync()
        {
            try
            {
                LogHelper.Info($"开始启动服务器{Setting.ServerType}");
                var hotfixPath = Directory.GetCurrentDirectory() + "/hotfix";
                if (!Directory.Exists(hotfixPath))
                {
                    Directory.CreateDirectory(hotfixPath);
                }

                ConfigComponent.Instance.LoadConfig();
                
                LogHelper.Info($"actor limit logic start...");
                ActorLimit.Init(ActorLimit.RuleType.None);
                LogHelper.Info($"actor limit logic end...");

                LogHelper.Info($"launch db service start...");
                MongoDbService mongoDbService = new MongoDbService();
                GameDb.Init(mongoDbService);
                GameDb.Open(Setting.DataBaseUrl, Setting.DataBaseName);
                LogHelper.Info($"launch db service end...");

                LogHelper.Info($"register comps start...");
                await ComponentRegister.Init(typeof(AppsHandler).Assembly);
                LogHelper.Info($"register comps end...");

                LogHelper.Info($"load hotfix module start");
                await HotfixManager.LoadHotfixModule(Setting);
                LogHelper.Info($"load hotfix module end");

                LogHelper.Info("进入游戏主循环...");
                LogHelper.Info("***进入游戏主循环***");
                GlobalSettings.LaunchTime = DateTime.Now;
                GlobalSettings.IsAppRunning = true;
                await AppExitToken;
            }
            catch (Exception e)
            {
                LogHelper.Info($"服务器执行异常，e:{e}");
                LogHelper.Fatal(e);
            }

            LogHelper.Info($"退出服务器开始");
            await HotfixManager.Stop();
            LogHelper.Info($"退出服务器成功");
        }

        protected override void Init()
        {
            if (Setting == null)
            {
                Setting = new AppSetting
                {
                    ServerId = 1501,
                    ServerType = ServerType.Game,
                    TcpPort = 21000,
                    HttpPort = 20001,
                    WsPort = 21100,
                    //
                    HttpCode = "inner_httpcode",
                    DataBaseUrl = "mongodb+srv://gameframex:f9v42aU9DVeFNfAF@gameframex.8taphic.mongodb.net/?retryWrites=true&w=majority",
                    DataBaseName = "gameframex"
                };
            }

            base.Init();
        }
    }
}