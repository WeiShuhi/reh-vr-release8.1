using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ET.Client
{
	[EntitySystemOf(typeof(UILoginComponent))]
	[FriendOf(typeof(UILoginComponent))]
	[FriendOf(typeof(VirtualKeyboard))]
	public static partial class UILoginComponentSystem
	{
		[EntitySystem]
		private static void Awake(this UILoginComponent self)
		{
			ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			self.loginBtn = rc.Get<GameObject>("LoginBtn");

			self.loginBtn.GetComponent<Button>().onClick.AddListener(() => { self.OnLogin(); });
			self.account = rc.Get<GameObject>("Account");
			self.password = rc.Get<GameObject>("Password");
			
			// 获取虚拟键盘组件
			GameObject keyboardGO = rc.Get<GameObject>("VirtualKeyboard");
			VirtualKeyboard virtualKeyboard = keyboardGO.AddComponent<VirtualKeyboard>();
			
			// 获取TMP_InputField组件
			TMP_InputField accountInput = self.account.GetComponent<TMP_InputField>();
			TMP_InputField passwordInput = self.password.GetComponent<TMP_InputField>();
			
			// 为输入框添加点击事件，激活虚拟键盘
			accountInput.onSelect.AddListener((text) => virtualKeyboard.SetInputField(accountInput));
			passwordInput.onSelect.AddListener((text) => virtualKeyboard.SetInputField(passwordInput));
			
			// 为虚拟键盘添加事件处理
			virtualKeyboard.OnEnterPressed += () => self.OnLogin();
			
			// 添加XR支持的交互组件
			AddXRInteractionComponents(self.account);
			AddXRInteractionComponents(self.password);
			AddXRInteractionComponents(self.loginBtn);
		}
		
		private static void AddXRInteractionComponents(GameObject gameObject)
		{
			// 添加XR交互组件
			if (!gameObject.TryGetComponent(out XRGraphicRaycaster xrGraphicRaycaster))
			{
				xrGraphicRaycaster = gameObject.AddComponent<XRGraphicRaycaster>();
			}
			
			// 如果是按钮，添加XR交互组件
			Button button = gameObject.GetComponent<Button>();
			if (button != null && !button.TryGetComponent(out XRButtonInteractable xrButtonInteractable))
			{
				xrButtonInteractable = button.gameObject.AddComponent<XRButtonInteractable>();
				xrButtonInteractable.button = button;
			}
		}

		public static void OnLogin(this UILoginComponent self)
		{
			LoginHelper.Login(
				self.Root(),
				self.account.GetComponent<TMP_InputField>().text,
				self.password.GetComponent<TMP_InputField>().text).Coroutine();
		}
	}
}
