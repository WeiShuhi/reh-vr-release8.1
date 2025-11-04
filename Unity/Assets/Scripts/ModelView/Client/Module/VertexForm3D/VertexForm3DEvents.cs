using ET;

namespace ET.Client
{
    /// <summary>
    /// VertexForm3D场景加载完成事件
    /// </summary>
    public class VertexForm3DSceneLoadedEvent : Entity
    {
        public string SceneName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            SceneName = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D场景卸载完成事件
    /// </summary>
    public class VertexForm3DSceneUnloadedEvent : Entity
    {
        public string SceneName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            SceneName = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D玩家生成事件
    /// </summary>
    public class VertexForm3DPlayerSpawnedEvent : Entity
    {
        public GameObject PlayerGameObject { get; set; }
        public bool IsLocalPlayer { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            PlayerGameObject = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D玩家消失事件
    /// </summary>
    public class VertexForm3DPlayerDespawnedEvent : Entity
    {
        public GameObject PlayerGameObject { get; set; }
        public bool IsLocalPlayer { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            PlayerGameObject = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D登录成功事件
    /// </summary>
    public class VertexForm3DLoginSuccessEvent : Entity
    {
        public string PlayerName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            PlayerName = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D登出事件
    /// </summary>
    public class VertexForm3DLogoutEvent : Entity
    {
        public string PlayerName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            PlayerName = null;
            base.Dispose();
        }
    }
}