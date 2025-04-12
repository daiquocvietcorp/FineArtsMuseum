using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trigger
{
    [CreateAssetMenu(fileName = "ExitData", menuName = "Trigger/ExitData")]
    public class ExitData : ScriptableObject
    {
        public List<ExitDataObject> exitDataObjects;
    }
    
    [Serializable]
    public class ExitDataObject
    {
        [Header("General")]
        public int id;
        public int toSceneId;
    }
}
