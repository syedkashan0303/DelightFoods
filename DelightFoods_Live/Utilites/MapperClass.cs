using AutoMapper;

namespace DelightFoods_Live.Utilites
{
    public class MapperClass<TSource, TDestination>
    {
        private MapperConfiguration _config;

        public MapperClass()
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>();
            });
        }

        public void CreateMap()
        {
            _config.CreateMapper().ConfigurationProvider.AssertConfigurationIsValid();
        }

        public TDestination Map(TSource source)
        {
            var mapper = _config.CreateMapper();
            return mapper.Map<TSource, TDestination>(source);
        }

    }

    public class SessionUtilities
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionUtilities(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public void AddDetailsInSessions(Users users)
        //{
        //    _httpContextAccessor.HttpContext.Session.SetInt32("Id", users.Id);
        //    _httpContextAccessor.HttpContext.Session.SetString("Email", users.Email);
        //    _httpContextAccessor.HttpContext.Session.SetString("UserName", users.FirstName + " " + users.LastName);
        //    _httpContextAccessor.HttpContext.Session.SetString("UserRole", (users.UserRoleId == 1 ? "Admin" : users.UserRoleId == 2 ? "Lawyer" : "Client"));
        //}
    }

}
