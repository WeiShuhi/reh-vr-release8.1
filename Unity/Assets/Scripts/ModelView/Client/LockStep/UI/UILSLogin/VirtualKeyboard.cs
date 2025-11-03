using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class VirtualKeyboard : Entity, IAwake
    {
        public GameObject KeyboardRoot;
        public TMP_InputField CurrentInputField;
        public UnityAction<string> OnTextChanged;
        public UnityAction OnEnterPressed;
        public UnityAction OnBackspacePressed;
        public UnityAction OnSpacePressed;
        public UnityAction OnShiftPressed;
        public UnityAction OnCapsLockPressed;
        public UnityAction OnSymbolPressed;
        
        private bool isShiftActive = false;
        private bool isCapsLockActive = false;
        private bool isSymbolModeActive = false;
        
        public void SetInputField(TMP_InputField inputField)
        {
            CurrentInputField = inputField;
            if (inputField != null)
            {
                OnTextChanged = (text) => inputField.text = text;
            }
        }
        
        public void AddCharacter(string character)
        {
            if (CurrentInputField == null) return;
            
            string newText = CurrentInputField.text + character;
            CurrentInputField.text = newText;
            CurrentInputField.caretPosition = newText.Length;
            
            OnTextChanged?.Invoke(newText);
        }
        
        public void Backspace()
        {
            if (CurrentInputField == null || CurrentInputField.text.Length == 0) return;
            
            string newText = CurrentInputField.text.Substring(0, CurrentInputField.text.Length - 1);
            CurrentInputField.text = newText;
            CurrentInputField.caretPosition = newText.Length;
            
            OnBackspacePressed?.Invoke();
        }
        
        public void Enter()
        {
            OnEnterPressed?.Invoke();
        }
        
        public void Space()
        {
            AddCharacter(" ");
            OnSpacePressed?.Invoke();
        }
        
        public void Shift()
        {
            isShiftActive = !isShiftActive;
            UpdateKeyboardLayout();
            OnShiftPressed?.Invoke();
        }
        
        public void CapsLock()
        {
            isCapsLockActive = !isCapsLockActive;
            UpdateKeyboardLayout();
            OnCapsLockPressed?.Invoke();
        }
        
        public void Symbol()
        {
            isSymbolModeActive = !isSymbolModeActive;
            UpdateKeyboardLayout();
            OnSymbolPressed?.Invoke();
        }
        
        public void Clear()
        {
            if (CurrentInputField == null) return;
            
            CurrentInputField.text = "";
            CurrentInputField.caretPosition = 0;
            
            OnTextChanged?.Invoke("");
        }
        
        private void UpdateKeyboardLayout()
        {
            // 根据当前模式更新键盘按键显示
            // 这里需要根据实际的键盘UI结构来实现
            // 可以通过设置不同的TextMeshPro组件文本来切换大小写和符号
        }
        
        public bool IsShiftActive => isShiftActive;
        public bool IsCapsLockActive => isCapsLockActive;
        public bool IsSymbolModeActive => isSymbolModeActive;
    }
}