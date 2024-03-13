using AutoMapper;
using Domain.Entities;
using WebUi.Controllers;

namespace WebUi.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Escort, HomeViewItem>();
            CreateMap<Escort, ProfileViewModel>();
            CreateMap<Escort, BookingViewItem>();
        }
    }
}
