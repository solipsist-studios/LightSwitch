
using Proyecto26;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

using Nanoleaf.Models;

namespace Nanoleaf
{
    public class NanoleafManager : NetworkBehaviour
    {
        private readonly string basePath = "https://jsonplaceholder.typicode.com";
        private RequestHelper currentRequest;
        //private bool isDeviceOn = false;
        public NetworkVariable<bool> IsDeviceOn = new NetworkVariable<bool>();

        [SerializeField] private string deviceAddress;
        [SerializeField] private int devicePort;
        [SerializeField] private string authToken;

        private void Start()
        {
            if (ConfigurationManager.Instance != null)
            {
                deviceAddress = ConfigurationManager.Instance.nanoleafAddr;
                devicePort = ConfigurationManager.Instance.nanoleafPort;
                authToken = ConfigurationManager.Instance.nanoleafAuthToken;
            }
        }

        public override void OnNetworkSpawn()
        {
            RestClient.DefaultRequestHeaders["Connection"] = "Keep-Alive";
            RestClient.DefaultRequestHeaders["Accept"] = "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml";
            
            if (IsOwner && !String.IsNullOrEmpty(authToken))
            {
                GetCurrentState();
            }
        }

        public void AbortRequest()
        {
            if (currentRequest != null)
            {
                currentRequest.Abort();
                currentRequest = null;
            }
        }

        public void Pair()
        {
            currentRequest = new RequestHelper
            {
                Uri = String.Format("http://{0}:{1}/api/v1/new", deviceAddress, devicePort),
                EnableDebug = true
            };
            RestClient.Post<AuthTokenResponse>(currentRequest)
            .Then(res =>
            {
                Debug.Log(JsonUtility.ToJson(res, true));

                authToken = res.auth_token;
            })
            .Catch(err => Debug.LogException(err));
        }

        private void GetCurrentState()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                currentRequest = new RequestHelper
                {
                    Uri = String.Format("http://{0}:{1}/api/v1/{2}/state/on", deviceAddress, devicePort, authToken),
                    UseHttpContinue = false,
                    EnableDebug = true
                };
                RestClient.Get<BoolValue>(currentRequest)
                .Then(res =>
                {
                    Debug.Log("Current State: " + JsonUtility.ToJson(res, true));

                    IsDeviceOn.Value = res.value;
                })
                .Catch(err => Debug.LogException(err));
            }
        }

        public void ToggleState()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                SendToggleStateRequest();
            }
            else
            {
                SubmitToggleStateServerRpc();
            }
        }

        private void SendToggleStateRequest()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("[Warning] Trying to send REST request from client.");
                return;
            }

            currentRequest = new RequestHelper
            {
                Uri = String.Format("http://{0}:{1}/api/v1/{2}/state", deviceAddress, devicePort, authToken),
                Body = new OnState
                {
                    on = new BoolValue
                    {
                        value = !IsDeviceOn.Value
                    }
                },
                UseHttpContinue = false,
                EnableDebug = true
            };
            RestClient.Put<OnState>(currentRequest, (err, res, body) =>
            {
                if (err != null)
                {
                    Debug.LogException(err);
                }
                else
                {
                    Debug.Log("Success: " + JsonUtility.ToJson(body, true));
                    IsDeviceOn.Value = !IsDeviceOn.Value;
                }
            });
        }

        [ServerRpc(RequireOwnership = false)]
        private void SubmitToggleStateServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("[ServerRpc] ToggleStateServerRpc invoked");
            SendToggleStateRequest();
        }
    }
}