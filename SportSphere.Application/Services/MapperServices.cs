using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace MagiXSquad.Application.Services
{
    public static class MapperServices
    {
        public static IMapper Mapper { get; set; }

        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source)
        {
            return source.ProjectTo<TDestination>(Mapper.ConfigurationProvider);
        }
        public static IEnumerable<TDestination> MapEnumerableTo<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            return Mapper.Map<IEnumerable<TDestination>>(source);
        }

        public static IEnumerable<TDestination> ProjectEnumrableTo<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            return source.AsQueryable().ProjectTo<TDestination>(Mapper.ConfigurationProvider);
        }


        public static TDestination Map<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public static TDestination MapOnto<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}
