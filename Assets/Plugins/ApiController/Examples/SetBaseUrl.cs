using UnityEngine;

namespace ApiController.Examples
{
    public class SetBaseUrl : MonoBehaviour
    {
        public string baseUrl;
        private void Awake()
        {
            Controller.Initialize(baseUrl);
        }
    }
}