using ETModel;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    [UIComponent(UIType.UILSLogin)]
    public class VirtualKeyboard : Entity, IAwake
    {
        public GameObject keyboardPanel;
        public Button[] letterButtons;
        public Button[] numberButtons;
        public Button backspaceButton;
        public Button enterButton;
        public Button shiftButton;
        public Button spaceButton;
        public Button closeButton;

        private TMP_InputField currentInputField;
        private bool isShiftPressed = false;

        public void Awake()
        {
            // 查找键盘面板
            this.keyboardPanel = this.GetParent<UIBaseWindow>().GameObject.transform.Find("KeyboardPanel").gameObject;

            // 查找所有字母按钮
            this.letterButtons = new Button[26];
            for (int i = 0; i < 26; i++)
            {
                char letter = (char)('a' + i);
                this.letterButtons[i] = this.keyboardPanel.transform.Find("Letters/" + letter.ToString().ToUpper()).GetComponent<Button>();
                this.letterButtons[i].onClick.AddListener(() => OnLetterButtonClick(letter));
            }

            // 查找所有数字按钮
            this.numberButtons = new Button[10];
            for (int i = 0; i < 10; i++)
            {
                this.numberButtons[i] = this.keyboardPanel.transform.Find("Numbers/" + i.ToString()).GetComponent<Button>();
                this.numberButtons[i].onClick.AddListener(() => OnNumberButtonClick(i));
            }

            // 查找其他按钮
            this.backspaceButton = this.keyboardPanel.transform.Find("Backspace").GetComponent<Button>();
            this.backspaceButton.onClick.AddListener(OnBackspaceButtonClick);

            this.enterButton = this.keyboardPanel.transform.Find("Enter").GetComponent<Button>();
            this.enterButton.onClick.AddListener(OnEnterButtonClick);

            this.shiftButton = this.keyboardPanel.transform.Find("Shift").GetComponent<Button>();
            this.shiftButton.onClick.AddListener(OnShiftButtonClick);

            this.spaceButton = this.keyboardPanel.transform.Find("Space").GetComponent<Button>();
            this.spaceButton.onClick.AddListener(OnSpaceButtonClick);

            this.closeButton = this.keyboardPanel.transform.Find("Close").GetComponent<Button>();
            this.closeButton.onClick.AddListener(OnCloseButtonClick);

            // 默认隐藏键盘
            this.keyboardPanel.SetActive(false);
        }

        public void ShowKeyboard(TMP_InputField inputField)
        {
            this.currentInputField = inputField;
            this.keyboardPanel.SetActive(true);
        }

        public void HideKeyboard()
        {
            this.currentInputField = null;
            this.keyboardPanel.SetActive(false);
        }

        private void OnLetterButtonClick(char letter)
        {
            if (this.currentInputField == null) return;

            string text = this.currentInputField.text;
            char c = this.isShiftPressed ? char.ToUpper(letter) : letter;
            this.currentInputField.text = text + c;
            this.currentInputField.caretPosition = text.Length + 1;

            // 自动切换回小写
            if (this.isShiftPressed)
            {
                this.isShiftPressed = false;
                UpdateShiftButtonState();
            }
        }

        private void OnNumberButtonClick(int number)
        {
            if (this.currentInputField == null) return;

            string text = this.currentInputField.text;
            this.currentInputField.text = text + number.ToString();
            this.currentInputField.caretPosition = text.Length + 1;
        }

        private void OnBackspaceButtonClick()
        {
            if (this.currentInputField == null) return;

            string text = this.currentInputField.text;
            if (text.Length > 0)
            {
                this.currentInputField.text = text.Substring(0, text.Length - 1);
                this.currentInputField.caretPosition = Mathf.Max(0, text.Length - 1);
            }
        }

        private void OnEnterButtonClick()
        {
            if (this.currentInputField == null) return;

            // 触发输入框的提交事件
            this.currentInputField.onSubmit.Invoke(this.currentInputField.text);
            HideKeyboard();
        }

        private void OnShiftButtonClick()
        {
            this.isShiftPressed = !this.isShiftPressed;
            UpdateShiftButtonState();
        }

        private void OnSpaceButtonClick()
        {
            if (this.currentInputField == null) return;

            string text = this.currentInputField.text;
            this.currentInputField.text = text + " ";
            this.currentInputField.caretPosition = text.Length + 1;
        }

        private void OnCloseButtonClick()
        {
            HideKeyboard();
        }

        private void UpdateShiftButtonState()
        {
            // 更新Shift按钮的外观
            Text shiftText = this.shiftButton.GetComponentInChildren<Text>();
            if (shiftText != null)
            {
                shiftText.text = this.isShiftPressed ? "SHIFT" : "shift";
            }
        }
    }
}