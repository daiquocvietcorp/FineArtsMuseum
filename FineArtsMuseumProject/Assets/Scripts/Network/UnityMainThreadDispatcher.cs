using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace Network
{
    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>
    {
        private Queue<Action> _actionQueue;

        void Update()
        {
            lock (_actionQueue)
            {
                while (_actionQueue.Count > 0)
                {
                    Action action = _actionQueue.Dequeue();
                    action?.Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(action);
            }
        }

        public void Initialize()
        {
            _actionQueue = new Queue<Action>();
        }
    }
}