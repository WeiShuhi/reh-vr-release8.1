using ET.Client;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILSLoginComponent))]
    [FriendOf(typeof(UILSLoginComponent))]
    public static partial class UILSLoginComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UILSLoginComponent self)
        {
            // 获取UI元素
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.accountInput = rc.Get<GameObject>("Account").GetComponent<TMP_InputField>();
            self.passwordInput = rc.Get<GameObject>("Password").GetComponent<TMP_InputField>();
            self.loginBtn = rc.Get<GameObject>("LoginBtn").GetComponent<XRButtonInteractable>();
            self.accountBtn = rc.Get<GameObject>("Account").GetComponent<XRButtonInteractable>();
            self.passwordBtn = rc.Get<GameObject>("Password").GetComponent<XRButtonInteractable>();

            // 初始化虚拟键盘
            self.virtualKeyboard = self.AddChild<VirtualKeyboard>();
            self.virtualKeyboard.Awake();

            // 设置登录按钮点击事件
            self.loginBtn.onClick.AddListener(() => Login(self));

            // 设置输入框点击事件，显示虚拟键盘
            self.accountBtn.onClick.AddListener(() => ShowVirtualKeyboard(self, self.accountInput));
            self.passwordBtn.onClick.AddListener(() => ShowVirtualKeyboard(self, self.passwordInput));

            // 设置输入框的提交事件，隐藏虚拟键盘
            self.accountInput.onSubmit.AddListener((value) => self.virtualKeyboard.HideKeyboard());
            self.passwordInput.onSubmit.AddListener((value) => self.virtualKeyboard.HideKeyboard());
        }

        private static void ShowVirtualKeyboard(UILSLoginComponent self, TMP_InputField inputField)
        {
            self.virtualKeyboard.ShowKeyboard(inputField);
        }

        private static void Login(UILSLoginComponent self)
        {
            string account = self.accountInput.text;
            string password = self.passwordInput.text;

            // 检查输入是否为空
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                Log.Error("账号或密码不能为空");
                return;
            }

            // 调用登录逻辑
            self.ClientScene().GetComponent<LoginComponent>().Login(account, password).Coroutine();
        }
    }
}