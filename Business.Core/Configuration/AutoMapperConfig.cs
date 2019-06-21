using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace Business.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AutoMapperConfig : Attribute
    {
        public AutoMapperConfig()
        {
            RegisterMappings();
        }

        private static void RegisterMappings()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            var profiles =
                types.Where(x => x.IsSubclassOf(typeof(Profile)))
                     .Select(Activator.CreateInstance)
                     .OfType<Profile>()
                     .ToList();

            Mapper.Initialize(cfg => profiles.ForEach(cfg.AddProfile));
        }
    }
}