using AutoMapper;
using Invoice_Management.Models.ViewModels;
using Invoice_Management.Models.ViewModels.InvoiceModel;
using Invoice_Management.Models.ViewModels.ReportViewModel;
using InvoiceManagement.Domain.Entities;

namespace Invoice_Management.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Report, ReportsViewModel>();

            CreateMap<InvoiceItem, InvoiceItemViewModel>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

            CreateMap<Invoice, InvoiceViewModel>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            CreateMap<Product, ProductViewModel>();

            CreateMap<CreateProductViewModel, Product>();

            CreateMap<Product, EditProductViewModel>();

            CreateMap<EditProductViewModel, Product>();

            CreateMap<ProductSale, ProductSaleViewModel>();

            CreateMap<InvoiceItemViewModel, InvoiceItem>();
        }
    }
}