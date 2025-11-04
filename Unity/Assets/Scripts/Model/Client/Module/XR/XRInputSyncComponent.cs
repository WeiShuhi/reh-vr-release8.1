using System; using System.Collections.Generic; using ET;

namespace ET.Client {
    [ComponentOf(typeof(Scene))]
    public class XRInputSyncComponent : Entity, IAwake, IUpdate {
        // 网络会话
        private SessionComponent sessionComponent;
        
        // 输入事件同步队列
        private Queue<XRInputEvent> syncEventQueue;
        
        // 输入事件同步间隔（毫秒）
        private const long SyncInterval = 50;
        
        // 上次同步时间
        private long lastSyncTime;
        
        // XR输入事件结构
        public struct XRInputEvent {
            public string EventType;
            public string DeviceName;
            public string InputName;
            public float FloatValue;
            public bool BoolValue;
            public Vector3 VectorValue;
            public Quaternion QuaternionValue;
            public long Timestamp;
        }
        
        public void Awake() {
            sessionComponent = this.Scene().GetComponent<SessionComponent>();
            syncEventQueue = new Queue<XRInputEvent>();
            lastSyncTime = TimeInfo.Instance.ServerNow();
            
            // 订阅XR输入事件
            EventSystem.Instance.Subscribe<XREventControllerButton>(this.Scene(), OnControllerButtonEvent);
            EventSystem.Instance.Subscribe<XREventControllerAxis>(this.Scene(), OnControllerAxisEvent);
            EventSystem.Instance.Subscribe<XREventHandGesture>(this.Scene(), OnHandGestureEvent);
            EventSystem.Instance.Subscribe<XREventHeadPose>(this.Scene(), OnHeadPoseEvent);
            
            Log.Info("XRInputSyncComponent初始化完成");
        }
        
        public void Update() {
            // 定时同步输入事件
            long currentTime = TimeInfo.Instance.ServerNow();
            if (currentTime - lastSyncTime >= SyncInterval) {
                SyncInputEvents();
                lastSyncTime = currentTime;
            }
        }
        
        private void OnControllerButtonEvent(Scene scene, XREventControllerButton e) {
            XRInputEvent inputEvent = new XRInputEvent {
                EventType = "ControllerButton",
                DeviceName = e.DeviceName,
                InputName = e.InputName,
                BoolValue = e.Value,
                Timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到同步队列
            syncEventQueue.Enqueue(inputEvent);
        }
        
        private void OnControllerAxisEvent(Scene scene, XREventControllerAxis e) {
            XRInputEvent inputEvent = new XRInputEvent {
                EventType = "ControllerAxis",
                DeviceName = e.DeviceName,
                InputName = e.InputName,
                FloatValue = e.Value,
                Timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到同步队列
            syncEventQueue.Enqueue(inputEvent);
        }
        
        private void OnHandGestureEvent(Scene scene, XREventHandGesture e) {
            XRInputEvent inputEvent = new XRInputEvent {
                EventType = "HandGesture",
                DeviceName = "Hand",
                InputName = e.GestureName,
                FloatValue = e.Confidence,
                Timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到同步队列
            syncEventQueue.Enqueue(inputEvent);
        }
        
        private void OnHeadPoseEvent(Scene scene, XREventHeadPose e) {
            XRInputEvent inputEvent = new XRInputEvent {
                EventType = "HeadPose",
                DeviceName = "Head",
                InputName = "Pose",
                VectorValue = e.Position,
                QuaternionValue = e.Rotation,
                Timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到同步队列
            syncEventQueue.Enqueue(inputEvent);
        }
        
        private void SyncInputEvents() {
            // 如果没有会话或队列为空，则不进行同步
            if (sessionComponent?.Session == null || syncEventQueue.Count == 0) {
                return;
            }
            
            // 创建输入同步消息
            C2Server_XRInputSync message = C2Server_XRInputSync.Create();
            message.InputEvents = new List<C2Server_XRInputSync.XRInputEvent>();
            
            // 将队列中的事件转换为消息格式
            while (syncEventQueue.Count > 0) {
                XRInputEvent inputEvent = syncEventQueue.Dequeue();
                
                C2Server_XRInputSync.XRInputEvent msgEvent = new C2Server_XRInputSync.XRInputEvent {
                    EventType = inputEvent.EventType,
                    DeviceName = inputEvent.DeviceName,
                    InputName = inputEvent.InputName,
                    FloatValue = inputEvent.FloatValue,
                    BoolValue = inputEvent.BoolValue,
                    VectorValue = inputEvent.VectorValue,
                    QuaternionValue = inputEvent.QuaternionValue,
                    Timestamp = inputEvent.Timestamp
                };
                
                message.InputEvents.Add(msgEvent);
            }
            
            // 发送消息到服务器
            sessionComponent.Session.Send(message);
        }
    }
    
    // 客户端到服务器的XR输入同步消息
    [Message]
    public class C2Server_XRInputSync : MessageObject, IRequest {
        public static C2Server_XRInputSync Create() {
            return ObjectPool.Instance.Fetch(typeof(C2Server_XRInputSync)) as C2Server_XRInputSync;
        }
        
        public override void Dispose() {
            this.RpcId = default;
            this.InputEvents?.Clear();
            ObjectPool.Instance.Recycle(this);
        }
        
        public int RpcId { get; set; }
        public List<XRInputEvent> InputEvents = new List<XRInputEvent>();
        
        public struct XRInputEvent {
            public string EventType;
            public string DeviceName;
            public string InputName;
            public float FloatValue;
            public bool BoolValue;
            public Vector3 VectorValue;
            public Quaternion QuaternionValue;
            public long Timestamp;
        }
    }
}