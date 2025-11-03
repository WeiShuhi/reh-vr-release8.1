using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(VirtualKeyboard))]
    [FriendOf(typeof(VirtualKeyboard))]
    public static partial class VirtualKeyboardSystem
    {
        [EntitySystem]
        private static void Awake(this VirtualKeyboard self)
        {
            // 获取虚拟键盘的所有按键
            ReferenceCollector rc = self.KeyboardRoot.GetComponent<ReferenceCollector>();
            
            // 字母按键
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string buttonName = "Button" + c;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    string character = c.ToString();
                    button.onClick.AddListener(() => self.AddCharacter(character));
                }
            }
            
            // 数字按键
            for (int i = 0; i <= 9; i++)
            {
                string buttonName = "Button" + i;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    string character = i.ToString();
                    button.onClick.AddListener(() => self.AddCharacter(character));
                }
            }
            
            // 符号按键
            string[] symbols = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "+", "=", "[", "]", "{", "}", ";", ":", "'", "\"", ",", ".", "<", ">", "/", "?", "~" };
            foreach (string symbol in symbols)
            {
                string buttonName = "Button" + symbol;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    button.onClick.AddListener(() => self.AddCharacter(symbol));
                }
            }
            
            // 特殊按键
            GameObject backspaceButton = rc.Get<GameObject>("ButtonBackspace");
            if (backspaceButton != null)
            {
                Button button = backspaceButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Backspace());
            }
            
            GameObject enterButton = rc.Get<GameObject>("ButtonEnter");
            if (enterButton != null)
            {
                Button button = enterButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Enter());
            }
            
            GameObject spaceButton = rc.Get<GameObject>("ButtonSpace");
            if (spaceButton != null)
            {
                Button button = spaceButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Space());
            }
            
            GameObject shiftButton = rc.Get<GameObject>("ButtonShift");
            if (shiftButton != null)
            {
                Button button = shiftButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Shift());
            }
            
            GameObject capsLockButton = rc.Get<GameObject>("ButtonCapsLock");
            if (capsLockButton != null)
            {
                Button button = capsLockButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.CapsLock());
            }
            
            GameObject symbolButton = rc.Get<GameObject>("ButtonSymbol");
            if (symbolButton != null)
            {
                Button button = symbolButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Symbol());
            }
            
            GameObject clearButton = rc.Get<GameObject>("ButtonClear");
            if (clearButton != null)
            {
                Button button = clearButton.GetComponent<Button>();
                button.onClick.AddListener(() => self.Clear());
            }
        }
        
        [EntitySystem]
        private static void Destroy(this VirtualKeyboard self)
        {
            // 移除所有事件监听
            ReferenceCollector rc = self.KeyboardRoot.GetComponent<ReferenceCollector>();
            
            // 字母按键
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string buttonName = "Button" + c;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                }
            }
            
            // 数字按键
            for (int i = 0; i <= 9; i++)
            {
                string buttonName = "Button" + i;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                }
            }
            
            // 特殊按键
            GameObject backspaceButton = rc.Get<GameObject>("ButtonBackspace");
            if (backspaceButton != null)
            {
                Button button = backspaceButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject enterButton = rc.Get<GameObject>("ButtonEnter");
            if (enterButton != null)
            {
                Button button = enterButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject spaceButton = rc.Get<GameObject>("ButtonSpace");
            if (spaceButton != null)
            {
                Button button = spaceButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject shiftButton = rc.Get<GameObject>("ButtonShift");
            if (shiftButton != null)
            {
                Button button = shiftButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject capsLockButton = rc.Get<GameObject>("ButtonCapsLock");
            if (capsLockButton != null)
            {
                Button button = capsLockButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject symbolButton = rc.Get<GameObject>("ButtonSymbol");
            if (symbolButton != null)
            {
                Button button = symbolButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
            
            GameObject clearButton = rc.Get<GameObject>("ButtonClear");
            if (clearButton != null)
            {
                Button button = clearButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
            }
        }
        
        private static void UpdateKeyboardLayout(this VirtualKeyboard self)
        {
            ReferenceCollector rc = self.KeyboardRoot.GetComponent<ReferenceCollector>();
            
            // 更新字母按键的显示
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string buttonName = "Button" + c;
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    TextMeshProUGUI text = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        if (self.IsCapsLockActive || self.IsShiftActive)
                        {
                            text.text = c.ToString();
                        }
                        else
                        {
                            text.text = c.ToString().ToLower();
                        }
                    }
                }
            }
            
            // 更新符号按键的显示
            string[] symbols = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "+", "=", "[", "]", "{", "}", ";", ":", "'", "\"", ",", ".", "<", ">", "/", "?", "~" };
            string[] lowercaseSymbols = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "_", "+", "=", "[", "]", "{", "}", ";", ":", "'", "\"", ",", ".", "<", ">", "/", "?", "~" };
            
            for (int i = 0; i < symbols.Length; i++)
            {
                string buttonName = "Button" + symbols[i];
                GameObject buttonGO = rc.Get<GameObject>(buttonName);
                if (buttonGO != null)
                {
                    TextMeshProUGUI text = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        if (self.IsSymbolModeActive)
                        {
                            text.text = symbols[i];
                        }
                        else
                        {
                            text.text = lowercaseSymbols[i];
                        }
                    }
                }
            }
            
            // 更新Shift按钮的显示
            GameObject shiftButton = rc.Get<GameObject>("ButtonShift");
            if (shiftButton != null)
            {
                TextMeshProUGUI text = shiftButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    if (self.IsShiftActive)
                    {
                        text.text = "SHIFT (ON)";
                    }
                    else
                    {
                        text.text = "SHIFT";
                    }
                }
            }
            
            // 更新Caps Lock按钮的显示
            GameObject capsLockButton = rc.Get<GameObject>("ButtonCapsLock");
            if (capsLockButton != null)
            {
                TextMeshProUGUI text = capsLockButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    if (self.IsCapsLockActive)
                    {
                        text.text = "CAPS (ON)";
                    }
                    else
                    {
                        text.text = "CAPS";
                    }
                }
            }
            
            // 更新Symbol按钮的显示
            GameObject symbolButton = rc.Get<GameObject>("ButtonSymbol");
            if (symbolButton != null)
            {
                TextMeshProUGUI text = symbolButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    if (self.IsSymbolModeActive)
                    {
                        text.text = "SYMBOL (ON)";
                    }
                    else
                    {
                        text.text = "SYMBOL";
                    }
                }
            }
        }
    }
}