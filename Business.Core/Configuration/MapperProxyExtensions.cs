namespace Business.Core.Configuration
{
    public static class MapperProxyExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            return new MapperProxy<object>().Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            return new MapperProxy<TSource>().Map<TDestination>(source);
        }
    }
}
