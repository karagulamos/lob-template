using AutoMapper;
using Business.Core.Services;

namespace Business.Core.Configuration
{
    public class MapperProxy<TSource> : IMapperProxy<TSource>
    {
        public TDestination Map<TDestination>(TSource source)
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}
