using ET;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(VertexForm3DBridge))]
    [FriendOf(typeof(VertexForm3DBridge))]
    public static partial class VertexForm3DBridgeSystem
    {
        [EntitySystem]
        private static void Awake(this VertexForm3DBridge self)
        {
            Log.Info("VertexForm3DBridgeSystem Awake");
            
            // 初始化VertexForm3DBridge
            self.Initialize();
            
            // 注册ET框架事件
            RegisterETEvents(self);
            
            // 启动更新循环
            self.StartCoroutine(UpdateLoop(self).Coroutine());
        }
        
        [EntitySystem]
        private static void Destroy(this VertexForm3DBridge self)
        {
            Log.Info("VertexForm3DBridgeSystem Destroy");
            
            // 取消注册ET框架事件
            UnregisterETEvents(self);
        }
        
        private static void RegisterETEvents(VertexForm3DBridge self)
        {
            // 注册ET框架事件到VertexForm3D
            EventSystem.Instance.Subscribe(self, typeof(LoginSuccessEvent), OnLoginSuccess);
            EventSystem.Instance.Subscribe(self, typeof(LogoutEvent), OnLogout);
            EventSystem.Instance.Subscribe(self, typeof(SceneChangeFinish), OnSceneChangeFinish);
        }
        
        private static void UnregisterETEvents(VertexForm3DBridge self)
        {
            // 取消注册ET框架事件
            EventSystem.Instance.Unsubscribe(self, typeof(LoginSuccessEvent), OnLoginSuccess);
            EventSystem.Instance.Unsubscribe(self, typeof(LogoutEvent), OnLogout);
            EventSystem.Instance.Unsubscribe(self, typeof(SceneChangeFinish), OnSceneChangeFinish);
        }
        
        private static void OnLoginSuccess(Entity self, object args)
        {
            var bridge = self as VertexForm3DBridge;
            var loginEvent = args as LoginSuccessEvent;
            
            if (bridge == null || loginEvent == null)
                return;
                
            // 将ET登录成功事件转发到VertexForm3D
            if (bridge.LoginManager != null)
            {
                bridge.LoginManager.PlayerName_InputName.text = loginEvent.Account;
                bridge.LoginManager.ConnectAnonymously();
            }
        }
        
        private static void OnLogout(Entity self, object args)
        {
            var bridge = self as VertexForm3DBridge;
            
            if (bridge == null)
                return;
                
            // 将ET登出事件转发到VertexForm3D
            if (bridge.VirtualRoomManager != null)
            {
                bridge.VirtualRoomManager.LeaveRoomAndLoadHomeScene();
            }
        }
        
        private static void OnSceneChangeFinish(Entity self, object args)
        {
            var bridge = self as VertexForm3DBridge;
            var sceneEvent = args as SceneChangeFinish;
            
            if (bridge == null || sceneEvent == null)
                return;
                
            // 将ET场景切换事件转发到VertexForm3D
            if (bridge.SceneLoader != null)
            {
                bridge.SceneLoader.LoadScene(sceneEvent.SceneName);
            }
        }
        
        private static async ETTask UpdateLoop(VertexForm3DBridge self)
        {
            while (true)
            {
                await ETTask.DelayFrame(1);
                
                // 处理玩家生成和消失事件
                HandlePlayerEvents(self);
            }
        }
        
        private static void HandlePlayerEvents(VertexForm3DBridge self)
        {
            if (self.SpawnManager == null)
                return;
                
            // 检测新生成的玩家
            foreach (var player in self.SpawnManager.allPlayers)
            {
                if (player == null || player.gameObject == null)
                    continue;
                    
                // 检查玩家是否已经被处理过
                if (!player.gameObject.TryGetComponent(out ETPlayerTag etPlayerTag))
                {
                    // 添加ETPlayerTag标记
                    etPlayerTag = player.gameObject.AddComponent<ETPlayerTag>();
                    etPlayerTag.IsLocalPlayer = player.photonView.IsMine;
                    
                    // 发布玩家生成事件
                    var playerSpawnedEvent = self.AddChild<VertexForm3DPlayerSpawnedEvent>();
                    playerSpawnedEvent.PlayerGameObject = player.gameObject;
                    playerSpawnedEvent.IsLocalPlayer = etPlayerTag.IsLocalPlayer;
                    
                    EventSystem.Instance.Publish(self, playerSpawnedEvent);
                    
                    Log.Info("VertexForm3D player spawned: {0}, IsLocal: {1}", player.gameObject.name, etPlayerTag.IsLocalPlayer);
                }
            }
            
            // 检测消失的玩家
            var allETPlayerTags = GameObject.FindObjectsOfType<ETPlayerTag>();
            foreach (var etPlayerTag in allETPlayerTags)
            {
                if (etPlayerTag == null || etPlayerTag.gameObject == null)
                    continue;
                    
                // 检查玩家是否仍然存在于SpawnManager中
                bool playerExists = false;
                foreach (var player in self.SpawnManager.allPlayers)
                {
                    if (player != null && player.gameObject == etPlayerTag.gameObject)
                    {
                        playerExists = true;
                        break;
                    }
                }
                
                if (!playerExists)
                {
                    // 发布玩家消失事件
                    var playerDespawnedEvent = self.AddChild<VertexForm3DPlayerDespawnedEvent>();
                    playerDespawnedEvent.PlayerGameObject = etPlayerTag.gameObject;
                    playerDespawnedEvent.IsLocalPlayer = etPlayerTag.IsLocalPlayer;
                    
                    EventSystem.Instance.Publish(self, playerDespawnedEvent);
                    
                    Log.Info("VertexForm3D player despawned: {0}, IsLocal: {1}", etPlayerTag.gameObject.name, etPlayerTag.IsLocalPlayer);
                    
                    // 移除ETPlayerTag标记
                    GameObject.Destroy(etPlayerTag);
                }
            }
        }
    }
}

// 用于标记ET玩家的组件
public class ETPlayerTag : MonoBehaviour
{
    public bool IsLocalPlayer { get; set; }
}