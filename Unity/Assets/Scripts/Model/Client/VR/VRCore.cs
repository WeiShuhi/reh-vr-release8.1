using System;using ET;using UnityEngine.XR;using UnityEngine.XR.Interaction.Toolkit;

namespace ET.Client.VR
{
    [EntitySystemOf(typeof(VRCoreComponent))]
    [FriendOf(typeof(VRCoreComponent))]
    public static partial class VRCoreComponentSystem
    {
        [EntitySystem]
        private static void Awake(this VRCoreComponent self)
        {
            self.IsInitialized = false;
            self.IsVRModeActive = false;
        }

        [EntitySystem]
        private static void Start(this VRCoreComponent self)
        {
            self.InitializeVRAsync().Coroutine();
        }

        [EntitySystem]
        private static void Update(this VRCoreComponent self)
        {
            if (!self.IsVRModeActive) return;
            
            // 更新VR输入
            self.UpdateVRInput();
            
            // 更新VR姿态
            self.UpdateVRPose();
        }

        private static async ETTask InitializeVRAsync(this VRCoreComponent self)
        {
            try
            {
                // 检查XR设备是否可用
                if (!XRDevice.isPresent)
                {
                    Log.Warning("XR device not present");
                    return;
                }

                // 初始化XR输入系统
                await InputDevices.InitAsync();
                
                // 初始化XR交互工具包
                XRInteractionManager interactionManager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();
                if (interactionManager != null)
                {
                    self.XRInteractionManager = interactionManager;
                }

                // 获取主相机
                self.MainCamera = UnityEngine.Camera.main;
                
                // 初始化VR输入
                self.InitializeVRInput();
                
                // 初始化VR姿态跟踪
                self.InitializeVRPose();
                
                self.IsInitialized = true;
                self.IsVRModeActive = true;
                
                Log.Info("VR system initialized successfully");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to initialize VR system: {e}");
                self.IsInitialized = false;
                self.IsVRModeActive = false;
            }
        }

        private static void InitializeVRInput(this VRCoreComponent self)
        {
            // 初始化控制器输入
            self.LeftController = new VRControllerInput();
            self.RightController = new VRControllerInput();
            
            // 初始化头部输入
            self.HeadInput = new VRHeadInput();
        }

        private static void UpdateVRInput(this VRCoreComponent self)
        {
            // 更新控制器输入
            self.LeftController.Update(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller);
            self.RightController.Update(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller);
            
            // 更新头部输入
            self.HeadInput.Update(InputDeviceCharacteristics.HeadMounted);
        }

        private static void InitializeVRPose(this VRCoreComponent self)
        {
            // 初始化姿态跟踪
            self.VRPose = new VRPose();
        }

        private static void UpdateVRPose(this VRCoreComponent self)
        {
            // 更新头部姿态
            if (self.HeadInput.Device.isValid)
            {
                self.HeadInput.Device.TryGetFeatureValue(CommonUsages.devicePosition, out self.VRPose.HeadPosition);
                self.HeadInput.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out self.VRPose.HeadRotation);
            }
            
            // 更新左控制器姿态
            if (self.LeftController.Device.isValid)
            {
                self.LeftController.Device.TryGetFeatureValue(CommonUsages.devicePosition, out self.VRPose.LeftControllerPosition);
                self.LeftController.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out self.VRPose.LeftControllerRotation);
            }
            
            // 更新右控制器姿态
            if (self.RightController.Device.isValid)
            {
                self.RightController.Device.TryGetFeatureValue(CommonUsages.devicePosition, out self.VRPose.RightControllerPosition);
                self.RightController.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out self.VRPose.RightControllerRotation);
            }
        }
    }

    public class VRCoreComponent : Entity, IAwake, IStart, IUpdate
    {
        public bool IsInitialized { get; set; }
        public bool IsVRModeActive { get; set; }
        public UnityEngine.Camera MainCamera { get; set; }
        public XRInteractionManager XRInteractionManager { get; set; }
        public VRControllerInput LeftController { get; set; }
        public VRControllerInput RightController { get; set; }
        public VRHeadInput HeadInput { get; set; }
        public VRPose VRPose { get; set; }
    }

    public struct VRControllerInput
    {
        public InputDevice Device { get; private set; }
        public bool TriggerPressed { get; private set; }
        public bool GripPressed { get; private set; }
        public bool PrimaryButtonPressed { get; private set; }
        public bool SecondaryButtonPressed { get; private set; }
        public UnityEngine.Vector2 PrimaryAxis { get; private set; }
        public UnityEngine.Vector2 SecondaryAxis { get; private set; }

        public void Update(InputDeviceCharacteristics characteristics)
        {
            if (!this.Device.isValid)
            {
                this.Device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            }

            if (this.Device.isValid)
            {
                this.Device.TryGetFeatureValue(CommonUsages.triggerButton, out this.TriggerPressed);
                this.Device.TryGetFeatureValue(CommonUsages.gripButton, out this.GripPressed);
                this.Device.TryGetFeatureValue(CommonUsages.primaryButton, out this.PrimaryButtonPressed);
                this.Device.TryGetFeatureValue(CommonUsages.secondaryButton, out this.SecondaryButtonPressed);
                this.Device.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.PrimaryAxis);
                this.Device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out this.SecondaryAxis);
            }
        }
    }

    public struct VRHeadInput
    {
        public InputDevice Device { get; private set; }
        public UnityEngine.Vector3 Position { get; private set; }
        public UnityEngine.Quaternion Rotation { get; private set; }

        public void Update(InputDeviceCharacteristics characteristics)
        {
            if (!this.Device.isValid)
            {
                this.Device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            }

            if (this.Device.isValid)
            {
                this.Device.TryGetFeatureValue(CommonUsages.devicePosition, out this.Position);
                this.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out this.Rotation);
            }
        }
    }

    public struct VRPose
    {
        public UnityEngine.Vector3 HeadPosition;
        public UnityEngine.Quaternion HeadRotation;
        public UnityEngine.Vector3 LeftControllerPosition;
        public UnityEngine.Quaternion LeftControllerRotation;
        public UnityEngine.Vector3 RightControllerPosition;
        public UnityEngine.Quaternion RightControllerRotation;
    }
}