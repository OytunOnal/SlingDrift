using DG.Tweening;
using Game.LevelSystem.LevelEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private GameObject _levelUp;
        [SerializeField] private Text _counterText;
        [SerializeField] private TMP_Text _perfectMultiplierText;
        [SerializeField] private CanvasGroup _perfectCanvas;
        [SerializeField] private TMP_Text _highScoreText;

        private int _gamePlayCount = 0;
        private int _score = 0;
        private int _highscore = 0;

        public void Initialize()
        {
            _levelUp.SetActive(false);
            _counterText.enabled = false;
            _highscore = PlayerPrefs.GetInt("HighScore",0);
            _highScoreText.text = _highscore.ToString();

            _startButton.onClick.AddListener(() => LevelEventBus.InvokeEvent(LevelEventType.STARTED));
            _startButton.onClick.AddListener(() => LevelEventBus.InvokeEvent(LevelEventType.STARTED , ++_gamePlayCount));

            _restartButton.onClick.AddListener(() => LevelEventBus.InvokeEvent(LevelEventType.INIT));

            LevelEventBus.SubscribeEvent(LevelEventType.INIT, () =>
            {
                _score = 0;
                UpdateCounter(0);
                ButtonVisible(_startButton, true);
                ButtonVisible(_restartButton, false);
            });

            LevelEventBus.SubscribeEvent(LevelEventType.STARTED, ()=>
            {
                _counterText.enabled = true;
                ButtonVisible(_startButton, false);
            });
            LevelEventBus.SubscribeEvent(LevelEventType.FAIL, ()=>
            {
                ButtonVisible(_restartButton, true);
                if (_score > _highscore)
                {
                    _highscore = _score;
                    PlayerPrefs.SetInt("HighScore", _highscore);
                    _highScoreText.text = _highscore.ToString();
                }
            });
            LevelEventBus.SubscribeEvent(LevelEventType.LEVEL_UP,OnLevelUp);
        }

        public void UpdateCounter(int perfectCount)
        {
            if (perfectCount>1) 
            {
                _perfectMultiplierText.text = "PERFECT X " + perfectCount.ToString();
            }
            
            _score += perfectCount;
            _counterText.text = _score.ToString();
        }

        public void Perfect()
        {
            _perfectCanvas.DOFade(1, 0.5f).OnComplete(() =>
            {
                _perfectCanvas.DOFade(0, 0.5f).SetDelay(0.5f);
            });
        }

        public void UpdateHighScore(int count)
        {
            _highScoreText.text = count.ToString();
        }

        private void OnLevelUp()
        {
            _levelUp.SetActive(true);
            Timer.Instance.TimerWait(1f, () =>  _levelUp.SetActive(false));
        }

        private void ButtonVisible(Button button,bool isVisible)
        {
            button.gameObject.SetActive(isVisible);
        }
    }
}
