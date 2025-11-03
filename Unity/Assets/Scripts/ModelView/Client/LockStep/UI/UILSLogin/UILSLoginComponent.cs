using ET.Client;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UILSLoginComponent: Entity, IAwake
    {
        public TMP_InputField accountInput;
        public TMP_InputField passwordInput;
        public XRButtonInteractable loginBtn;
        public XRButtonInteractable accountBtn;
        public XRButtonInteractable passwordBtn;
        public VirtualKeyboard virtualKeyboard;
    }
}
