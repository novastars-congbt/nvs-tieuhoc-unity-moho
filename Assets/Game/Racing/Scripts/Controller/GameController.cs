using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

namespace Novastars.MiniGame.DuaXe
{
    public class GameController : SingletonBehaviour<GameController>
    {
        [Header("Các loại xe")]
        [SerializeField][LabelText("Xe người chơi")] public Image _mainCar;
        [SerializeField][LabelText("Xe cùng làn")] public Image _roadCar;
        [SerializeField][LabelText("Xe chặn đường")] public Image _obstacleCar;
        [SerializeField][LabelText("Đích đến")] private Image _lineDes;

        [Header("Vị trí xe")]
        [SerializeField][LabelText("Đường đi")] private Transform _road;
        [SerializeField][LabelText("Xe đang chạy cùng làn")] private Transform _currentCarBlockTransform;
        [SerializeField][LabelText("Vị trí xe cùng làn")] private Transform _roadCarListParrent;
        [SerializeField][LabelText("Vị trí xe chặn đường")] private Transform _obstacleCarListParrent;

        [Header("Cài đặt hình nền động")]
        [ReadOnly][SerializeField][LabelText("Bật hình nền động")] private bool _isParallaxOn = false;
        [Space]
        [SerializeField][LabelText("Ảnh nền động")] private Image _cityParallax;
        [SerializeField][LabelText("Ảnh nền động màn chơi")] private Image _roadParallax;
        [SerializeField][LabelText("Ảnh nền động rìa màn chơi")] private Image _sideWalkParallax;
        [Space]
        [SerializeField][LabelText("Tốc độ nền màn chơi")] public float _foregroundSpeed;
        [SerializeField][LabelText("Tốc độ nền")] public float _backgroundSpeed;
        [Space]
        [Header("Cài đặt đích đến")]
        [SerializeField][LabelText("Điểm chuẩn bị")] private Transform _beforeStartPoint;
        [SerializeField][LabelText("Điểm bắt đầu")] private Transform _startPoint;
        [SerializeField][LabelText("Điểm cuối")] private Transform _endPoint;
        [SerializeField][LabelText("Điểm đến")] private Transform _destination;
        [SerializeField][LabelText("Điểm kết thúc")] private Transform _afterEndPoint;
        [Space]
        [Header("Cài đặt các làn đường")]
        [SerializeField][LabelText("Làn đường trên")] private Transform _road1Pos;
        [SerializeField][LabelText("Làn đường giữa")] private Transform _road2Pos;
        [SerializeField][LabelText("Làn đường dưới")] private Transform _road3Pos;

        private readonly float m_parallaxSpeedModify = 10f;
        private float duration;
        private Vector2 _carStartPos;
        private Vector2 _lineStartPos;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();

            SetupParallax();
        }
        private void Start()
        {
            duration = 1.5f * (0.6f / _foregroundSpeed);
            _carStartPos = _mainCar.transform.position;
            _lineStartPos = _lineDes.transform.position;
        }

        private void Update()
        {
            if (_isParallaxOn) StartParallax();
            if (_foregroundSpeed >= 1) _foregroundSpeed =_foregroundSpeed/10;
        }
        #endregion

        #region Public Method
        public void SetupDefault()
        {
            DOTween.CompleteAll();
            foreach (Transform child in _road.GetComponentsInChildren<Transform>()){
                if (child != _road && child.name.Contains("(Clone)")){
                    Destroy(child.gameObject);
                }
            }
            _isParallaxOn = false;
            _destination.gameObject.SetActive(false);
            _mainCar.transform.SetParent(_road.transform, true);
            _mainCar.transform.position = _carStartPos;
            _lineDes.transform.position = _lineStartPos;
        }
        public float CarSize(Image car){
            var size = 1f;
            switch(car.sprite.name){
                case "Racing_Car01":
                    size = 0.8f;
                    break;
                case "Racing_Car02":
                    size = 1.2f;
                    break;
                case "Racing_Car03":
                    size = 1;
                    break;
                case "Racing_Car04":
                    size = 2;
                    break;
                case "Racing_Car05":
                    size = 0.85f;
                    break;
                default:
                    break;
            }
            return size;
        }
        public void StartRace()
        {
            if (!_isParallaxOn) FeedbackManager.Instance.PlayStartCarSFX();
            var StartVFX = FeedbackManager.Instance.PlayStartCarVFX();
            StartVFX.transform.SetParent(_mainCar.transform, true);
            if (StartVFX) StartVFX.transform.localPosition = new Vector2(-160, 30);

            _mainCar.transform.DOMove(_endPoint.position, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                _isParallaxOn = true;
                
                _mainCar.transform.DOMove(_startPoint.position, 3).SetEase(Ease.Linear);
                _lineDes.transform.DOMoveX(_beforeStartPoint.position.x, duration).SetEase(Ease.Linear);
                
                SetupRoadCar();
            });
        }
        public void SetupCar()
        {
            _mainCar.sprite = GameManager.Instance.CurrentCarSprite;
            _mainCar.GetComponent<RectTransform>().sizeDelta = GameManager.Instance.CurrentCarSprite.rect.size;
            _mainCar.GetComponent<RectTransform>().localScale = Vector3.one * CarSize(_mainCar);
            CarController.Instance.WheelCar(_mainCar);
        }

        public void RunToRoad(int roadNumber, bool answer)
        {
            switch (roadNumber)
            {
                case 1:
                    RunToRoad_1(answer);
                    _mainCar.transform.SetSiblingIndex(0);
                    break;
                case 2:
                    RunToRoad_2(answer);
                    _mainCar.transform.SetSiblingIndex(1);
                    break;
                case 3:
                    RunToRoad_3(answer);
                    _mainCar.transform.SetSiblingIndex(2);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private Method
        private void SetupParallax()
        {
            _foregroundSpeed /= m_parallaxSpeedModify;
            _backgroundSpeed /= m_parallaxSpeedModify;

            _roadParallax.material = new Material(_roadParallax.material);
            _sideWalkParallax.material = new Material(_sideWalkParallax.material);
            _cityParallax.material = new Material(_cityParallax.material);
        }

        private void StartParallax()
        {
            var currentBackgroundOffset = _cityParallax.material.GetTextureOffset("_MainTex");
            currentBackgroundOffset = new Vector2(currentBackgroundOffset.x + Time.deltaTime * _backgroundSpeed, currentBackgroundOffset.y);

            var currentForegroundOffset = _roadParallax.material.GetTextureOffset("_MainTex");
            currentForegroundOffset = new Vector2(currentForegroundOffset.x + Time.deltaTime * _foregroundSpeed, currentForegroundOffset.y);

            _cityParallax.material.SetTextureOffset("_MainTex", currentBackgroundOffset);
            _roadParallax.material.SetTextureOffset("_MainTex", currentForegroundOffset);
            _sideWalkParallax.material.SetTextureOffset("_MainTex", currentForegroundOffset);
        }

        private void SetupRoadCar()
        {
            if (GameManager.Instance.CurrentQuestionIndex >= GameManager.Instance.CurrentQuestionDatas.Count) return;

            var tempRoadCar = Instantiate(_roadCar.gameObject, _roadCarListParrent).transform; 
            var carSprite = UIManager.Instance.CarsRoadSprites;
            var tempRoadCarImage = tempRoadCar.GetComponent<Image>();
            var tempRoadCarRect = tempRoadCar.GetComponent<RectTransform>();
            
            tempRoadCarImage.sprite = carSprite[Random.Range(0, carSprite.Count)];
            GameManager.Instance.CurrentRoadSprite = tempRoadCarImage.sprite;
            tempRoadCarRect.sizeDelta = tempRoadCarImage.sprite.rect.size;
            tempRoadCar.GetComponent<RectTransform>().localScale = Vector2.one * CarSize(tempRoadCarImage);

            RoadController.Instance.WheelRoad(tempRoadCarImage);

            tempRoadCar.transform.position = new Vector3(_afterEndPoint.position.x, _mainCar.transform.position.y);
            tempRoadCar.transform
                .DOMove(new Vector2(_startPoint.position.x + 7f, _mainCar.transform.position.y), 5)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _currentCarBlockTransform = tempRoadCar;

                    CheckBlockRoadNumber();
                });
        }

        private void SetupObtascleCar()
        {
            if (GameManager.Instance.CurrentQuestionIndex >= GameManager.Instance.CurrentQuestionDatas.Count) return;

            var tempObstacleCar = Instantiate(_obstacleCar.gameObject, _obstacleCarListParrent).transform;
            var carSprite = UIManager.Instance.CarObstacleSprites;
            var tempObstacleCarImage = tempObstacleCar.GetComponent<Image>();
            var tempObstacleCarRect = tempObstacleCar.GetComponent<RectTransform>();

            tempObstacleCarImage.sprite = carSprite[Random.Range(0, carSprite.Count)];
            GameManager.Instance.CurrentBlockSprite = tempObstacleCarImage.sprite;
            tempObstacleCarRect.sizeDelta = tempObstacleCarImage.sprite.rect.size;
            BlockController.Instance.WheelBlock(tempObstacleCarImage);
            
            var isSmallCar = tempObstacleCarRect.sizeDelta.x < 800;

            tempObstacleCar.transform.position = new Vector3(_afterEndPoint.position.x, _mainCar.transform.position.y);
            
            tempObstacleCar.transform
                .DOMove(new Vector2(_startPoint.position.x + (isSmallCar ? 8 : 9), _mainCar.transform.position.y), (isSmallCar ? 8 : 9))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _currentCarBlockTransform = tempObstacleCar;
                    _isParallaxOn = false;
                    _mainCar.transform.DOShakePosition(1f, 5f);

                    FeedbackManager.Instance.PlayStopCarSFX();
                    var StopVFX = FeedbackManager.Instance.PlayStopCarVFX();
                    StopVFX.transform.SetParent(_mainCar.transform, true);
                    if (StopVFX) StopVFX.transform.localPosition = new Vector2(100, 70);
                    
                    CheckBlockRoadNumber();
                    DOVirtual.DelayedCall(0.25f, () =>DOTween.PauseAll());
                });
        }
        private void CheckFinishRace()
        {
            if (GameManager.Instance.CurrentQuestionIndex < GameManager.Instance.CurrentQuestionDatas.Count) return;

            _lineDes.transform.position = new Vector2(_afterEndPoint.position.x, _lineDes.transform.position.y);
            _lineDes.transform
                .DOMove(new Vector2(_endPoint.position.x - 2, _lineDes.transform.position.y), duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _isParallaxOn = false;
                    _mainCar.transform
                    .DOMove(new Vector2(_endPoint.position.x + 3, _mainCar.transform.position.y), 1f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        //_mainCar.transform.DOMove(new Vector2(_afterEndPoint.position.x, _mainCar.transform.position.y), 1f).SetEase(Ease.Linear);

                        GameManager.Instance.IsGameOver = true;
                        _destination.gameObject.SetActive(true);
                        DOTween.PauseAll();
                        _mainCar.transform.SetParent(_destination.transform, true);
                        _mainCar.transform.position = _startPoint.position;
                        if (MiniGameEndController.instance != null)
                        {
                            MiniGameEndController.instance.ShowGameEnd();
                        }
                        else BaseGameUI.Instance.GetGameplayUILogic().ShowResult();
                        FeedbackManager.Instance.StopSound();
                        FeedbackManager.Instance.SpawnVictoryVFX();
                        FeedbackManager.Instance.PlayVictorySFX();
                    });
                });
        }

        private void CheckBlockRoadNumber()
        {
            if (Mathf.Abs(_currentCarBlockTransform.position.y - _road1Pos.position.y) <= 0.1f) BaseGameUI.Instance.GetGameplayUILogic().ShowQA(1);
            if (Mathf.Abs(_currentCarBlockTransform.position.y - _road2Pos.position.y) <= 0.1f) BaseGameUI.Instance.GetGameplayUILogic().ShowQA(2);
            if (Mathf.Abs(_currentCarBlockTransform.position.y - _road3Pos.position.y) <= 0.1f) BaseGameUI.Instance.GetGameplayUILogic().ShowQA(3);
        }

        private void RunToRoad_1(bool answer)
        {
            MoveCardToChossenRoad(_road1Pos, answer);
        }

        private void RunToRoad_2(bool answer)
        {
            MoveCardToChossenRoad(_road2Pos, answer);
        }

        private void RunToRoad_3(bool answer)
        {
            MoveCardToChossenRoad(_road3Pos, answer);
        }

        private void MoveCardToChossenRoad(Transform roadTransform, bool answer)
        {
            DOTween.PlayAll();
            if (!_isParallaxOn) FeedbackManager.Instance.PlayStartCarSFX();

            float centerX = (_mainCar.transform.position.x - _currentCarBlockTransform.position.x) / 2;
            Vector2 firstDodgeMove = new(_mainCar.transform.position.x + Mathf.Abs(centerX), roadTransform.position.y);

            _mainCar.transform.DOMove(firstDodgeMove, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                Vector2 newEndPoint = new(_endPoint.position.x, roadTransform.position.y);
                Vector2 newStartPoint = new(_startPoint.position.x, roadTransform.position.y);
                Vector2 newBeforeStartPoint = new(_beforeStartPoint.transform.position.x, _currentCarBlockTransform.position.y);
                float timeDodge = 2f;

                var index = _mainCar.transform.GetSiblingIndex();
                var next = (index == 2)? index -1 : index + 1;

                if (answer)
                {
                    _roadCarListParrent.SetSiblingIndex(next);
                    var TrueVFX = FeedbackManager.Instance.PlayTrueVFX();
                    if (TrueVFX) TrueVFX.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, TrueVFX.transform.position.z);
                    var StartVFX = FeedbackManager.Instance.PlayStartCarVFX();
                    StartVFX.transform.SetParent(_mainCar.transform, true);
                    if (StartVFX) StartVFX.transform.localPosition = new Vector2(-160, 30);

                    _mainCar.transform.DOMove(newEndPoint, timeDodge - (_isParallaxOn ? 0 : 1)).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        _isParallaxOn = true;
                        _mainCar.transform.DOMove(newStartPoint, timeDodge + 1).SetEase(Ease.Linear).OnComplete(CheckFinishRace);

                        GameManager.Instance.CurrentQuestionIndex++;

                        SetupRoadCar();
                    });
                }
                else
                {
                    _obstacleCarListParrent.SetSiblingIndex(next);
                    var FalseVFX = FeedbackManager.Instance.PlayFalseVFX();
                    if (FalseVFX) FalseVFX.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, FalseVFX.transform.position.z);
                    var StartVFX = FeedbackManager.Instance.PlayStartCarVFX();

                    StartVFX.transform.SetParent(_mainCar.transform, true);
                    if (StartVFX) StartVFX.transform.localPosition = new Vector2(-160, 30);
                    
                    _mainCar.transform.DOMove(newEndPoint, timeDodge - (_isParallaxOn ? 0 : 1)).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        _isParallaxOn = true;
                        _mainCar.transform.DOMove(newStartPoint, timeDodge + 1).SetEase(Ease.Linear).OnComplete(CheckFinishRace);
                    });

                    SetupObtascleCar();
                }

                _currentCarBlockTransform.DOMove(newBeforeStartPoint, 2).SetDelay(_isParallaxOn ? 0 : 1).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Destroy(_currentCarBlockTransform.gameObject);
                });
            });
        }
        #endregion
    }
}