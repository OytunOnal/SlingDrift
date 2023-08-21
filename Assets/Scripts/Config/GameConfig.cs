using DG.Tweening;
using Game.Managers;
using UnityEngine;

namespace Config
{
    public  class GameConfig : MonoBehaviour
    {
        #region Singleton Pattern

        private static GameConfig _instance;

        public static GameConfig Instance
        {
            get { return _instance ?? (_instance = (GameConfig)FindObjectOfType(typeof(GameConfig))); }
        }

        #endregion

        #region LevelConfig

        [SerializeField] public  int LEVEL_LENGTH = 6;
        [SerializeField] public  int U_CORNER_PROBABILITY = 5;
        [SerializeField] public  float CAR_PASS_TRESHOLD = 10f;

        [SerializeField] public float SLING_START_DISTANCE = 30f;
        [SerializeField] public float SLING_ROTATE_DISTANCE = 15f;
        [SerializeField] public int SLING_SPEED = 150;
        [SerializeField] public int SLING_ROTATE_SPEED = 150;
        [SerializeField] public float U_CORNER_INTERACTIBLE = 27f;
        [SerializeField] public float CORNER_INTERACTIBLE = 25f;

        [SerializeField] public float PERFECT_DISTANCE = 6f;
        [SerializeField] public float AFTER_DRIFT_DURATION = 6f;
        #endregion


        [SerializeField] public  float CAR_SPEED = 30f;

        public AnimationCurve easeCurve;
    }
}
