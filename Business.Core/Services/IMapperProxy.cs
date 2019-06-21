namespace Business.Core.Services
{
    public interface IMapperProxy<in TSource> : IServiceDependencyMarker
    {
        TDestination Map<TDestination>(TSource source);
    }
}