using System.Collections.Generic;
using Mine.Code.App.Model;
using Mine.Code.Framework.Manager.ResourceFactory;
using Mine.Code.Jelly;

namespace Mine.Code.Main.Model
{
    public class MainFactoryModel
    {
        public MainFactoryModel(JellyFarmJsonDBModel jellyFarmDBModel)
        {
            for (int i = 0; i < jellyFarmDBModel.JellyPresets.Count; i++)
            {
                JellyFactory.Add(ResourceFactory<JellyContext>.Builder.ByInject.Build(jellyFarmDBModel.JellyPresets[i].Value<string>("jellyPrefabPath")));
            }
        }
        public List<ResourceFactory<JellyContext>> JellyFactory { get; } = new();
    }
}
