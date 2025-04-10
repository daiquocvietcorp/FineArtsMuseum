using System;
using System.Collections;
using Camera;
using DesignPatterns;
using LayerMasks;
using Player;
using Trigger;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputController
{
    public class MouseInput : MonoSingleton<MouseInput>
    {
        private bool _isClick;
        private bool _isHold;
        private bool _isAvailable;
        private bool _isDragSlider;

        public bool IsClick => _isClick;
        public bool IsHold => _isHold;

        private float _holdTimer;
        private Action<Vector3> _onClick;
        private UnityEngine.Camera _mainCamera;

        private bool _isFirstPerson;
        private bool _canClickMove;
        private bool _isDragImage;
        private Coroutine _clickMoveCoroutine;

        [field: SerializeField] private MouseInputData data;
        [field: SerializeField] private Transform goToPointer;

        private GoToPointCloud _goToPointCloud;

        private void Awake()
        {
            _isAvailable = true;
            if (!PlatformManager.Instance.IsCloud) return;
            _goToPointCloud = goToPointer.AddComponent<GoToPointCloud>();
            goToPointer.gameObject.layer = LayerManager.Instance.pointCloudLayer.GetFirstLayerIndex();
            _goToPointCloud.RegisterActionClick(position => { _onClick?.Invoke(position); });
        }

        private void Start()
        {
            _mainCamera = CameraManager.Instance.mainCamera;
            _isClick = false;
            _isHold = false;
            _isFirstPerson = false;
            _canClickMove = false;
            goToPointer.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (PlatformManager.Instance.IsVR) return;
            if (!_isAvailable) return;

            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, data.View3RdGoToPointLimitDistance,
                        LayerManager.Instance.groundLayer))
                {
                    goToPointer.gameObject.SetActive(true);
                    goToPointer.position = hit.point;
                    _canClickMove = true;
                }
                else
                {
                    goToPointer.gameObject.SetActive(false);
                    _canClickMove = false;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    _holdTimer = Time.time;
                }

                if (Input.GetMouseButtonUp(0) && Time.time - _holdTimer < 0.2f)
                {
                    _isClick = true;
                    _isHold = false;
                    if (!_canClickMove) return;
                    _onClick?.Invoke(Input.mousePosition);
                    //_onClick?.Invoke(goToPointer.position);
                }
                else if (Input.GetMouseButton(0) && Time.time - _holdTimer > 0.1f)
                {
                    _isClick = false;

                    if (IsPointerOverUI() || _isDragImage) return;
                    PaintingDetailManager.Instance.SetColliderPainting(false);
                    _isHold = true;
                }
                else
                {
                    _isClick = false;
                    _isHold = false;
                    PaintingDetailManager.Instance.SetColliderPainting(true);
                }
            }

            if (PlatformManager.Instance.IsMobile)
            {
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        _holdTimer = Time.time;
                    }

                    if (touch.phase == TouchPhase.Ended && Time.time - _holdTimer < 0.2f)
                    {
                        _isClick = true;
                        _isHold = false;
                        
                        //ShowGoToPointer(touch.position);
                        //_onClick?.Invoke(touch.position);
                    }
                    else if (touch.phase == TouchPhase.Moved && Time.time - _holdTimer > 0.2f)
                    {
                        _isClick = false;

                        if (IsPointerOverUI() || _isDragImage) return;
                        PaintingDetailManager.Instance.SetColliderPainting(false);
                        /*var ray = _mainCamera.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out var hit, data.View3RdGoToPointLimitDistance,
                                LayerManager.Instance.groundLayer))
                        {
                            goToPointer.gameObject.SetActive(true);
                            goToPointer.position = hit.point;
                        }
                        else
                        {
                            goToPointer.gameObject.SetActive(false);
                        }*/
                        _isHold = true;
                    }
                    else if (touch.phase == TouchPhase.Stationary && Time.time - _holdTimer > 0.2f)
                    {
                        if (IsPointerOverUI() || _isDragImage) return;
                        PaintingDetailManager.Instance.SetColliderPainting(false);
                    }
                    else
                    {
                        _isClick = false;
                        _isHold = false;
                        PaintingDetailManager.Instance.SetColliderPainting(true);
                    }
                }
                else
                {
                    goToPointer.gameObject.SetActive(false);
                }
            }
            
            if (PlatformManager.Instance.IsTomko)
            {
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        _holdTimer = Time.time;
                    }

                    if (touch.phase == TouchPhase.Ended && Time.time - _holdTimer < 0.2f)
                    {
                        _isClick = true;
                        _isHold = false;
                        
                        //ShowGoToPointer(touch.position);
                        //_onClick?.Invoke(touch.position);
                    }
                    else if (touch.phase == TouchPhase.Moved && Time.time - _holdTimer > 0.2f)
                    {
                        _isClick = false;

                        if (IsPointerOverUI() || _isDragImage || _isDragSlider) return;
                        PaintingDetailManager.Instance.SetColliderPainting(false);
                        _isHold = true;
                    }
                    else if (touch.phase == TouchPhase.Stationary && Time.time - _holdTimer > 0.2f)
                    {
                        if (IsPointerOverUI() || _isDragImage || _isDragSlider) return;
                        PaintingDetailManager.Instance.SetColliderPainting(false);
                    }
                    else
                    {
                        _isClick = false;
                        _isHold = false;
                        PaintingDetailManager.Instance.SetColliderPainting(true);
                    }
                }
                else
                {
                    goToPointer.gameObject.SetActive(false);
                }
            }

            if (PlatformManager.Instance.IsCloud)
            {
                //ray center camera
                if (!_isFirstPerson)
                {
                    goToPointer.gameObject.SetActive(false);
                    return;
                }

                var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out var hit, data.View3RdGoToPointLimitDistance,
                        LayerManager.Instance.groundLayer))
                {
                    goToPointer.gameObject.SetActive(true);
                    goToPointer.position = hit.point;
                }
                else
                {
                    goToPointer.gameObject.SetActive(false);
                }
            }
        }

        public void ShowGoToPointer(Vector3 position)
        {
            if (_clickMoveCoroutine != null) StopCoroutine(_clickMoveCoroutine);
            _clickMoveCoroutine = StartCoroutine(ShowGotoPointerCoroutine(position));
        }

        private IEnumerator ShowGotoPointerCoroutine(Vector3 position)
        {
            goToPointer.position = position;
            goToPointer.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            goToPointer.gameObject.SetActive(false);
        }

        public void ChangeView(bool isFirstPerson)
        {
            _isFirstPerson = isFirstPerson;
        }

        public void RegisterClick(Action<Vector3> action)
        {
            _onClick = action;
        }

        public bool IsPointerOverUI()
        {
            if (!EventSystem.current) return false;

            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }

            if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
            }

            return false;
        }

        public void DisableMouseOrTouchInput()
        {
            _isAvailable = false;
        }

        public void EnableMouseOrTouchInput()
        {
            _isAvailable = true;
        }

        public void SetIsDragImage(bool isDragImage)
        {
            _isDragImage = isDragImage;
        }
        
        public void SetSliderDrag(bool isDrag)
        {
            _isDragSlider = isDrag;
        }
    }
}
