using System;
using Camera;
using DesignPatterns;
using LayerMasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputController
{
    public class MouseInput : MonoSingleton<MouseInput>
    {
        private bool _isClick;
        private bool _isHold;
        
        public bool IsClick => _isClick;
        public bool IsHold => _isHold;
        
        private float _holdTimer;
        private Action<Vector3> _onClick;
        private UnityEngine.Camera _mainCamera;
        
        private bool _isFirstPerson;
        private bool _canClickMove;

        [field: SerializeField] private MouseInputData data;
        [field: SerializeField] private Transform goToPointer;

        private void Start()
        {
            _mainCamera = CameraManager.Instance.mainCamera;
            _isClick = false;
            _isHold = false;
            _isFirstPerson = false;
            _canClickMove = false;
        }

        private void Update()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, data.View3RdGoToPointLimitDistance, LayerManager.Instance.groundLayer))
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
            
#if UNITY_EDITOR || UNITY_STANDALONE
            
            if (Input.GetMouseButtonDown(0))
            {
                _holdTimer = Time.time;
            }
            
            if(Input.GetMouseButtonUp(0) && Time.time - _holdTimer < 0.2f)
            {
                _isClick = true;
                _isHold = false;
                if(!_canClickMove) return;
                _onClick?.Invoke(Input.mousePosition);
                //_onClick?.Invoke(goToPointer.position);
            }
            else if (Input.GetMouseButton(0) && Time.time - _holdTimer > 0.2f)
            {
                _isClick = false;
                
                if(IsPointerOverUI()) return;
                _isHold = true;
            }
            else
            {
                _isClick = false;
                _isHold = false;
            }
            
#else
            
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
                    _onClick?.Invoke(touch.position);
                }
                else if (touch.phase == TouchPhase.Moved && Time.time - _holdTimer > 0.2f)
                {
                    _isHold = true;
                    _isClick = false;
                }
                else
                {
                    _isClick = false;
                    _isHold = false;
                }
            }
            
#endif
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
#if UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject();
#else
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
#endif
        }
    }
}
