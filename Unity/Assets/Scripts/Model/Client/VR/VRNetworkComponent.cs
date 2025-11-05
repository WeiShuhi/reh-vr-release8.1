using System;using ET;using MemoryPack;

namespace ET.Client.VR
{
    [EntitySystemOf(typeof(VRNetworkComponent))]
    [FriendOf(typeof(VRNetworkComponent))]
    public static partial class VRNetworkComponentSystem
    {
        [EntitySystem]
        private static void Awake(this VRNetworkComponent self)
        {
            self.IsConnected = false;
            self.VRDataRate = 60; // 60 FPS
            self.LastVRDataSentTime = 0;
        }

        [EntitySystem]
        private static void Start(this VRNetworkComponent self)
        {
            // 订阅网络连接事件
            EventSystem.Instance.Subscribe(self, typeof(NetConnected), self.OnNetConnected);
            EventSystem.Instance.Subscribe(self, typeof(NetDisconnected), self.OnNetDisconnected);
            
            // 启动VR数据发送协程
            self.SendVRDataLoopAsync().Coroutine();
        }

        [EntitySystem]
        private static void Destroy(this VRNetworkComponent self)
        {
            // 取消订阅网络连接事件
            EventSystem.Instance.Unsubscribe(self, typeof(NetConnected), self.OnNetConnected);
            EventSystem.Instance.Unsubscribe(self, typeof(NetDisconnected), self.OnNetDisconnected);
        }

        private static void OnNetConnected(this VRNetworkComponent self, object args)
        {
            self.IsConnected = true;
            Log.Info("VR network connected");
        }

        private static void OnNetDisconnected(this VRNetworkComponent self, object args)
        {
            self.IsConnected = false;
            Log.Info("VR network disconnected");
        }

        private static async ETTask SendVRDataLoopAsync(this VRNetworkComponent self)
        {
            while (true)
            {
                await ETTask.Delay(1000 / self.VRDataRate);
                
                if (!self.IsConnected) continue;
                
                // 获取最新的VR数据
                VRCoreComponent vrCore = self.Root().GetComponent<VRCoreComponent>();
                if (vrCore == null || !vrCore.IsVRModeActive) continue;
                
                // 发送VR姿态数据
                await self.SendVRPoseDataAsync(vrCore.VRPose);
            }
        }

        private static async ETTask SendVRPoseDataAsync(this VRNetworkComponent self, VRPose vrPose)
        {
            try
            {
                // 序列化VR姿态数据
                VRPoseMessage vrPoseMessage = VRPoseMessage.Create();
                vrPoseMessage.HeadPosition = vrPose.HeadPosition;
                vrPoseMessage.HeadRotation = vrPose.HeadRotation;
                vrPoseMessage.LeftControllerPosition = vrPose.LeftControllerPosition;
                vrPoseMessage.LeftControllerRotation = vrPose.LeftControllerRotation;
                vrPoseMessage.RightControllerPosition = vrPose.RightControllerPosition;
                vrPoseMessage.RightControllerRotation = vrPose.RightControllerRotation;
                vrPoseMessage.Timestamp = TimeInfo.Instance.ClientNow();
                
                // 发送消息
                await self.Root().GetComponent<ClientSenderComponent>().Send(vrPoseMessage);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send VR pose data: {e}");
            }
        }

        private static async ETTask SendVRInputDataAsync(this VRNetworkComponent self, VRInputData vrInputData)
        {
            try
            {
                // 序列化VR输入数据
                VRInputMessage vrInputMessage = VRInputMessage.Create();
                vrInputMessage.LeftTriggerPressed = vrInputData.LeftTriggerPressed;
                vrInputMessage.LeftGripPressed = vrInputData.LeftGripPressed;
                vrInputMessage.LeftPrimaryButtonPressed = vrInputData.LeftPrimaryButtonPressed;
                vrInputMessage.LeftSecondaryButtonPressed = vrInputData.LeftSecondaryButtonPressed;
                vrInputMessage.LeftPrimaryAxis = vrInputData.LeftPrimaryAxis;
                vrInputMessage.LeftSecondaryAxis = vrInputData.LeftSecondaryAxis;
                vrInputMessage.RightTriggerPressed = vrInputData.RightTriggerPressed;
                vrInputMessage.RightGripPressed = vrInputData.RightGripPressed;
                vrInputMessage.RightPrimaryButtonPressed = vrInputData.RightPrimaryButtonPressed;
                vrInputMessage.RightSecondaryButtonPressed = vrInputData.RightSecondaryButtonPressed;
                vrInputMessage.RightPrimaryAxis = vrInputData.RightPrimaryAxis;
                vrInputMessage.RightSecondaryAxis = vrInputData.RightSecondaryAxis;
                vrInputMessage.Timestamp = TimeInfo.Instance.ClientNow();
                
                // 发送消息
                await self.Root().GetComponent<ClientSenderComponent>().Send(vrInputMessage);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send VR input data: {e}");
            }
        }
    }

    public class VRNetworkComponent : Entity, IAwake, IStart, IDestroy
    {
        public bool IsConnected { get; set; }
        public int VRDataRate { get; set; }
        public long LastVRDataSentTime { get; set; }
    }

    [MemoryPackable]
    public partial class VRPoseMessage : MessageObject, IMessage
    {
        public static int opcode = 10001;
        public UnityEngine.Vector3 HeadPosition;
        public UnityEngine.Quaternion HeadRotation;
        public UnityEngine.Vector3 LeftControllerPosition;
        public UnityEngine.Quaternion LeftControllerRotation;
        public UnityEngine.Vector3 RightControllerPosition;
        public UnityEngine.Quaternion RightControllerRotation;
        public long Timestamp;
    }

    [MemoryPackable]
    public partial class VRInputMessage : MessageObject, IMessage
    {
        public static int opcode = 10002;
        public bool LeftTriggerPressed;
        public bool LeftGripPressed;
        public bool LeftPrimaryButtonPressed;
        public bool LeftSecondaryButtonPressed;
        public UnityEngine.Vector2 LeftPrimaryAxis;
        public UnityEngine.Vector2 LeftSecondaryAxis;
        public bool RightTriggerPressed;
        public bool RightGripPressed;
        public bool RightPrimaryButtonPressed;
        public bool RightSecondaryButtonPressed;
        public UnityEngine.Vector2 RightPrimaryAxis;
        public UnityEngine.Vector2 RightSecondaryAxis;
        public long Timestamp;
    }

    public struct VRInputData
    {
        public bool LeftTriggerPressed;
        public bool LeftGripPressed;
        public bool LeftPrimaryButtonPressed;
        public bool LeftSecondaryButtonPressed;
        public UnityEngine.Vector2 LeftPrimaryAxis;
        public UnityEngine.Vector2 LeftSecondaryAxis;
        public bool RightTriggerPressed;
        public bool RightGripPressed;
        public bool RightPrimaryButtonPressed;
        public bool RightSecondaryButtonPressed;
        public UnityEngine.Vector2 RightPrimaryAxis;
        public UnityEngine.Vector2 RightSecondaryAxis;
    }
}