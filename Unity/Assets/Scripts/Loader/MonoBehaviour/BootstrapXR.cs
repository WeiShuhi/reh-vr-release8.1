using System; using System.Threading.Tasks; using UnityEngine; using UnityEngine.XR; using UnityEngine.XR.Interaction.Toolkit; using ET;

public class BootstrapXR : MonoBehaviour {
    [SerializeField] private XRInteractionManager xrInteractionManager;
    [SerializeField] private XRRig xrRig;
    [SerializeField] private XRController leftController;
    [SerializeField] private XRController rightController;
    [SerializeField] private InputActionManager inputActionManager;
    [SerializeField] private GameObject xrCameraRig;
    
    private async ETTask StartAsync() {
        // 初始化XR系统
        await InitializeXRSystem();
        
        // 初始化XR摄像机
        InitializeXRCamera();
        
        // 初始化XR输入模块
        InitializeXRInput();
        
        // 接入ET世界逻辑
        await InitializeETWorld();
    }
    
    private async ETTask InitializeXRSystem() {
        // 检查XR设备是否可用
        if (!XRDevice.isPresent) {
            Log.Warning("XR设备不可用，将使用模拟模式");
            return;
        }
        
        // 初始化XR系统
        XRSettings.enabled = true;
        
        // 等待XR系统初始化完成
        await ETTask.Delay(1000);
        
        Log.Info("XR系统初始化完成");
    }
    
    private void InitializeXRCamera() {
        // 确保XR摄像机存在
        if (xrCameraRig == null) {
            xrCameraRig = GameObject.Find("XRCameraRig");
        }
        
        if (xrCameraRig != null) {
            // 设置XR摄像机为活跃状态
            xrCameraRig.SetActive(true);
            
            // 获取XRRig组件
            xrRig = xrCameraRig.GetComponent<XRRig>();
            if (xrRig != null) {
                Log.Info("XR摄像机初始化完成");
            }
        } else {
            Log.Warning("未找到XR摄像机，将使用默认摄像机");
        }
    }
    
    private void InitializeXRInput() {
        // 初始化XR输入管理器
        if (xrInteractionManager == null) {
            xrInteractionManager = FindObjectOfType<XRInteractionManager>();
        }
        
        if (xrInteractionManager != null) {
            xrInteractionManager.enabled = true;
            Log.Info("XR输入管理器初始化完成");
        } else {
            Log.Warning("未找到XR输入管理器");
        }
        
        // 初始化输入动作管理器
        if (inputActionManager == null) {
            inputActionManager = FindObjectOfType<InputActionManager>();
        }
        
        if (inputActionManager != null) {
            inputActionManager.EnableAllActions();
            Log.Info("XR输入动作管理器初始化完成");
        } else {
            Log.Warning("未找到XR输入动作管理器");
        }
        
        // 初始化控制器
        if (leftController == null) {
            leftController = FindObjectOfType<XRController>();
        }
        
        if (rightController == null) {
            rightController = FindObjectOfType<XRController>();
        }
        
        Log.Info("XR输入模块初始化完成");
    }
    
    private async ETTask InitializeETWorld() {
        // 初始化ET世界
        await ET.World.Instance.Initialize();
        
        // 创建ET场景
        ET.Scene scene = ET.Scene.Create(ET.World.Instance, ET.SceneType.Current);
        
        // 初始化XR组件
        scene.AddComponent<ET.Client.XRInputAdapterComponent>();
        scene.AddComponent<ET.Client.XRInputSyncComponent>();
        scene.AddComponent<ET.Client.XRLoggerComponent>();
        
        Log.Info("ET世界逻辑接入完成");
    }
    
    private void Start() {
        this.StartAsync().Coroutine();
    }
    
    private void Update() {
        // 处理XR输入更新
        UpdateXRInput();
    }
    
    private void UpdateXRInput() {
        // 这里将处理XR输入更新
    }
}