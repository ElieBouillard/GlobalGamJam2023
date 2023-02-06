namespace RSLib.Framework.GUI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class EnterSubmitInputField : MonoBehaviour
    {
        [System.Serializable]
        public class SubmitEvent : UnityEvent<string>
        {
        }

        [SerializeField] public SubmitEvent onSubmit = new SubmitEvent();

        private TMPro.TMP_InputField _inputField;
        private bool _wasFocused;

        private void Awake()
        {
            _inputField = GetComponent<TMPro.TMP_InputField>();
        }

        private void Update()
        {
            if (_wasFocused
                && _inputField.text != string.Empty
                && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
            {
                onSubmit.Invoke(_inputField.text);
                _inputField.text = string.Empty;
                _inputField.Select();
                _inputField.ActivateInputField();
            }

            _wasFocused = _inputField.isFocused;
        }
    }
}