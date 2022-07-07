using System;

namespace Nanoleaf.Models
{
    [Serializable]
    internal class OnState
    {
        public BoolValue on;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }

    [Serializable]
    internal class BoolValue
    {
        public bool value;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
