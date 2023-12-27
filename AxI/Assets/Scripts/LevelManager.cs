    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.IO.LowLevel.Unsafe;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using AxieCore.AxieMixer;
    using AxieMixer.Unity;
    using Newtonsoft.Json.Linq;
    using Spine.Unity;
    using UnityEngine.Networking;
    using Unity.VisualScripting;
    
    public class LevelManager : MonoBehaviour
    {
        public bool Won;
        public static LevelManager LInstance { get; private set; }
        public string axieSelect;
        public Vector2 spawnPos;
        public bool flag;
        Axie2dBuilder builder => Mixer.Builder;
        public UIController ui;
        bool isFetchingGenes = false;

        private void Awake()
        {
            if (LInstance != null && LInstance != this)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                LInstance = this;
                DontDestroyOnLoad(this);
            }
        }
        private void OnEnable()
        {
            Mixer.Init();
        }

    void ProcessMixer(string axieId, string genesStr, bool isGraphic, bool giftUse, int order)   //Lụm
        {
            if (string.IsNullOrEmpty(genesStr))
            {
                Debug.LogError($"[{axieId}] genes not found!!!");
                flag = false;
            }
            else
            {
                flag = true;
                float scale = 0.007f;
                ui = GameObject.Find("UIController").GetComponent<UIController>();
                var meta = new Dictionary<string, string>();
                        //foreach (var accessorySlot in ACCESSORY_SLOTS)
                        //{
                        //    meta.Add(accessorySlot, $"{accessorySlot}1{System.Char.ConvertFromUtf32((int)('a') + accessoryIdx - 1)}");
                        //}
                var builderResult = builder.BuildSpineFromGene(axieId, genesStr, meta, scale, isGraphic);

                        //Test
                if (giftUse) SpawnSkeletonGraphicForGift(builderResult, axieId, order);
                else
                {
                    if (isGraphic) SpawnSkeletonGraphic(builderResult, axieId, order);
                    else SpawnSkeletonAnimation(builderResult);
                }
            }
                
        }

        void SpawnSkeletonAnimation(Axie2dBuilderResult builderResult)
            {
                GameObject go = new GameObject("DemoAxie");
                go.transform.localPosition = new Vector3(spawnPos.x, spawnPos.y, 0f);
                SkeletonAnimation runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(builderResult.skeletonDataAsset);
                //runtimeSkeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Player");
                runtimeSkeletonAnimation.gameObject.tag = "Player";
                runtimeSkeletonAnimation.transform.SetParent(go.transform, false);
                runtimeSkeletonAnimation.transform.localScale = new Vector3(-0.5f,0.5f,1);

                runtimeSkeletonAnimation.gameObject.AddComponent<AutoBlendAnimController>();
                runtimeSkeletonAnimation.gameObject.AddComponent<AxieController>();
                runtimeSkeletonAnimation.state.SetAnimation(0, "action/idle/normal", true);

                if (builderResult.adultCombo.ContainsKey("body") &&
                    builderResult.adultCombo["body"].Contains("mystic") &&
                    builderResult.adultCombo.TryGetValue("body-class", out var bodyClass) &&
                    builderResult.adultCombo.TryGetValue("body-id", out var bodyId))
                {
                    runtimeSkeletonAnimation.gameObject.AddComponent<MysticIdController>().Init(bodyClass, bodyId);
                }
                runtimeSkeletonAnimation.skeleton.FindSlot("shadow").Attachment = null;
            }
        void SpawnSkeletonGraphicForGift(Axie2dBuilderResult builderResult, string axieId, int order)        // Load Axie for Gift
            {
                RectTransform rootTFforGift = GameObject.Find("Axie").GetComponent<RectTransform>();
                var skeletonGraphic = SkeletonGraphic.NewSkeletonGraphicGameObject(builderResult.skeletonDataAsset, rootTFforGift, builderResult.sharedGraphicMaterial);
                skeletonGraphic.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
                skeletonGraphic.gameObject.tag = "Gift";
                skeletonGraphic.Initialize(true);
                skeletonGraphic.Skeleton.SetSkin("default");
                skeletonGraphic.Skeleton.SetSlotsToSetupPose();

                skeletonGraphic.gameObject.AddComponent<AutoBlendAnimGraphicController>();
                skeletonGraphic.AnimationState.SetAnimation(0, "action/idle/normal", true);

                if (builderResult.adultCombo.ContainsKey("body") &&
                builderResult.adultCombo["body"].Contains("mystic") &&
                builderResult.adultCombo.TryGetValue("body-class", out var bodyClass) &&
                builderResult.adultCombo.TryGetValue("body-id", out var bodyId))
                {
                    skeletonGraphic.gameObject.AddComponent<MysticIdGraphicController>().Init(bodyClass, bodyId);
                }
            }
        void SpawnSkeletonGraphic(Axie2dBuilderResult builderResult, string axieId, int order)        // Load Axie for Home
            {
                RectTransform rootTFforHome = GameObject.Find("ListAxie").GetComponent<RectTransform>();
                var skeletonGraphic = SkeletonGraphic.NewSkeletonGraphicGameObject(builderResult.skeletonDataAsset, rootTFforHome, builderResult.sharedGraphicMaterial);
                skeletonGraphic.rectTransform.sizeDelta = new Vector2(1, 1);
                skeletonGraphic.rectTransform.localScale = new Vector3(0.33f,0.33f,0.1f);
                skeletonGraphic.rectTransform.anchoredPosition = new Vector2(1f, -1.5f);
                skeletonGraphic.Initialize(true);
                skeletonGraphic.Skeleton.SetSkin("default");
                skeletonGraphic.Skeleton.SetSlotsToSetupPose();
                
                skeletonGraphic.gameObject.AddComponent<AutoBlendAnimGraphicController>();
                skeletonGraphic.AnimationState.SetAnimation(0, "action/idle/normal", true);

                if (builderResult.adultCombo.ContainsKey("body") &&
                builderResult.adultCombo["body"].Contains("mystic") &&
                builderResult.adultCombo.TryGetValue("body-class", out var bodyClass) &&
                builderResult.adultCombo.TryGetValue("body-id", out var bodyId))
                {
                    skeletonGraphic.gameObject.AddComponent<MysticIdGraphicController>().Init(bodyClass, bodyId);
                }

                Axie a = new Axie(order, axieId, skeletonGraphic.gameObject);
                ui.axies.Add(a);
                if (ui.axies.Count > 1) skeletonGraphic.gameObject.SetActive(false);
            }

        public IEnumerator GetAxiesGenes(string axieId, bool UIUse, bool giftUse, int order)     // Lụm
            {
                isFetchingGenes = true;
                
                string searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
                JObject jPayload = new JObject();
                jPayload.Add(new JProperty("query", searchString));

                var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
                //var wr = new UnityWebRequest("https://testnet-graphql.skymavis.one/graphql", "POST");
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
                wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                wr.SetRequestHeader("Content-Type", "application/json");
                wr.timeout = 10;
                yield return wr.SendWebRequest();
                if (wr.error == null)
                {
                    var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
                    if (!string.IsNullOrEmpty(result))
                    {
                        JObject jResult = JObject.Parse(result);
                        string genesStr = (string)jResult["data"]["axie"]["newGenes"];
                        ProcessMixer(axieId, genesStr, UIUse, giftUse, order);
                    }
                }
                isFetchingGenes = false;
            }
    }
