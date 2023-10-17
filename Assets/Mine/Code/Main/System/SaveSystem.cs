using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mine.Code.App.Model;
using Mine.Code.Framework.Extension;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Jelly;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.System
{
    public class SaveSystem : VObject<MainContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        [Inject] readonly MainFactoryModel factoryModel;
        [Inject] readonly MainFolderModel mainFolderModel;
        [Inject] readonly FieldModel fieldModel;
        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly MainSetting mainSetting;

        #endregion
    
        #region Entry Point

        async void IStartable.Start()
        {
            fieldModel.Jellies = await LoadJellies();
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => Save());
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            jellyFarmDBModel.DB["Currency"] = JObject.FromObject(currencyModel.Data);
            jellyFarmDBModel.DB["Field"]["jellies"] = JArray.FromObject(fieldModel.Jellies.Select(model => model.Data));
            jellyFarmDBModel.DB["Plant"] = JObject.FromObject(upgradeModel.Data);
            jellyFarmDBModel.SaveDB();
        }

        async UniTask<List<JellyModel>> LoadJellies()
        {
            var jellies = await UniTask.WhenAll(jellyFarmDBModel.Jellies.Select(async data =>
            {
                var jellyContext = await factoryModel.JellyFactory[(int)data["id"]].LoadAsync();
                jellyContext.gameObject.SetActive(false);
                Transform jellyTransform = jellyContext.transform;
                jellyTransform.SetParent(mainFolderModel.JellyFolder);
                jellyTransform.position = mainSetting.RandomPositionInField;
                var jellyModel = jellyContext.Model;
                Context.Container.Inject(jellyModel);
                jellyModel.Load(data.Value<int>("level"), data.Value<int>("exp"));
                return jellyContext;
            }));
            
            return jellies.Select(jelly =>
            {
                jelly.gameObject.SetActive(true);
                return jelly.Model;
            }).ToList();
        }
    
        #endregion
    }
}