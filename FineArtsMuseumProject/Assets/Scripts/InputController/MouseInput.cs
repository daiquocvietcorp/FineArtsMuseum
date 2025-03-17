using System;
using DesignPatterns;
using UnityEngine;

namespace InputController
{
    public class MouseInput : MonoSingleton<MouseInput>
    {
        private bool _isClick;
        private bool _isHold;
        
        public bool IsClick => _isClick;
        public bool IsHold => _isHold;
        
        private float _holdTimer;

        private void Start()
        {
            _isClick = false;
            _isHold = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _holdTimer = Time.time;
            }
            
            if(Input.GetMouseButtonUp(0) && Time.time - _holdTimer < 0.2f)
            {
                _isClick = true;
                _isHold = false;
            }
            else if (Input.GetMouseButton(0) && Time.time - _holdTimer > 0.2f)
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
    }
}
