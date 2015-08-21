using AutoMapper;
using dto;
using model;
using System.ServiceProcess;
using utility;

namespace notifyservice
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        private static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            RedisHelper.Init();
            ModelMapper();


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HandleService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void ModelMapper()
        {
            Mapper.CreateMap<ErrorEntityDto, ErrorEntity>()
                .ForMember(dest => dest.ServerVariables,
                    opt => opt.MapFrom(src => DictionaryHelper.PrintDictionary(src.ServerVariables)))
                .ForMember(dest => dest.QueryString,
                    opt => opt.MapFrom(src => DictionaryHelper.PrintDictionary(src.QueryString)))
                .ForMember(dest => dest.Form,
                    opt => opt.MapFrom(src => DictionaryHelper.PrintDictionary(src.Form)))
                .ForMember(dest => dest.Cookies,
                    opt => opt.MapFrom(src => DictionaryHelper.PrintDictionary(src.Cookies)));
        }
    }
}