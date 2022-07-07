using System;

namespace Nanoleaf.Models
{
    [Serializable]
    internal class AuthTokenResponse
    {
        public string auth_token = null;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
