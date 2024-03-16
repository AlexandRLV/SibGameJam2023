using UnityEngine;

namespace GameCore.Enemies
{
    public class MarkController : MonoBehaviour
    {
        [SerializeField] private GameObject _questionMark;
        [SerializeField] private GameObject _exclamationMark;
        
        private void Start()
        {
            _questionMark.SetActive(false);
            _exclamationMark.SetActive(false);
        }

        public void SetProvokedMarkState(bool state)
        {
            if (_questionMark.activeSelf != state)
                _questionMark.SetActive(state);
        }
        public void SetAlarmMarkState(bool state)
        {
            if (_exclamationMark.activeSelf != state)
                _exclamationMark.SetActive(state);
        }

        public void SetQuestionMark()
        {
            _questionMark.SetActive(true);
            _exclamationMark.SetActive(false);
        }

        public void SetExclamationMark()
        {
            _questionMark.SetActive(false);
            _exclamationMark.SetActive(true);
        }

        public void ResetMarks()
        {
            _questionMark.SetActive(false);
            _exclamationMark.SetActive(false);
        }
    }
}
