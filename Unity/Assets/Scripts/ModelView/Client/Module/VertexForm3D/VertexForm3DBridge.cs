using ET;using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class VertexForm3DBridge : Entity, IAwake, IDestroy
    {
        public bool IsInitialized { get; private set; }
        public ProjectManager ProjectManager => VertexFormCore.ProjectManager.instance;
        public LoginManager LoginManager => VertexFormCore.LoginManager.instance;
        public SpawnManager SpawnManager => VertexFormCore.SpawnManager.Instance;
        public SceneLoader SceneLoader => VertexFormCore.SceneLoader.Instance;
        public VirtualRoomManager VirtualRoomManager => VertexFormCore.VirtualRoomManager.Instance;
        
        public void Initialize()
        {
            if (IsInitialized)
                return;
                
            // 确保VertexForm3D的单例已经初始化
            EnsureSingletonInitialized();
            
            // 注册事件监听
            RegisterEvents();
            
            IsInitialized = true;
            Log.Info("VertexForm3DBridge initialized successfully");
        }
        
        private void EnsureSingletonInitialized()
        {
            // 访问各个单例以确保它们被初始化
            _ = ProjectManager;
            _ = SceneLoader;
        }
        
        private void RegisterEvents()
        {
            // 注册VertexForm3D事件到ET框架
            SceneLoader.sceneLoaded += OnSceneLoaded;
            SceneLoader.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void UnregisterEvents()
        {
            // 取消注册事件
            SceneLoader.sceneLoaded -= OnSceneLoaded;
            SceneLoader.sceneUnloaded -= OnSceneUnloaded;
        }
        
        private void OnSceneLoaded(string sceneName)
        {
            // 将VertexForm3D场景加载事件转换为ET框架事件
            EventSystem.Instance.Publish(this, new VertexForm3DSceneLoadedEvent() { SceneName = sceneName });
        }
        
        private void OnSceneUnloaded(string sceneName)
        {
            // 将VertexForm3D场景卸载事件转换为ET框架事件
            EventSystem.Instance.Publish(this, new VertexForm3DSceneUnloadedEvent() { SceneName = sceneName });
        }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            UnregisterEvents();
            IsInitialized = false;
            
            base.Dispose();
        }
    }
}