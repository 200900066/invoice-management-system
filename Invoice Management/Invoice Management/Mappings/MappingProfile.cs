using AutoMapper;
using Invoice_Management.Models.ViewModels;
using Invoice_Management.Models.ViewModels.InvoiceModel;
using Invoice_Management.Models.ViewModels.ReportViewModel;
using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure__new_.Identity;

namespace Invoice_Management.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Report mappings  
            CreateMap<Report, ReportsViewModel>();

            // Invoice mappings
            CreateMap<InvoiceItem, InvoiceItemViewModel>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

            CreateMap<Invoice, InvoiceViewModel>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // Product mappings
            CreateMap<Product, ProductViewModel>();

            CreateMap<CreateProductViewModel, Product>();

            CreateMap<Product, EditProductViewModel>();

            CreateMap<EditProductViewModel, Product>();

            CreateMap<ProductSale, ProductSaleViewModel>();

            CreateMap<InvoiceItemViewModel, InvoiceItem>();

            // User mappings
            CreateMap<ApplicationUser, UserViewModel>()
           .ForMember(dest => dest.FullName,
               opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
           .ForMember(dest => dest.Roles,
               opt => opt.Ignore());

            CreateMap<CreateUserViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, EditUserViewModel>()
                
                .ForMember(dest => dest.SelectedRole, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<EditUserViewModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}