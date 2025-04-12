using AutoMapper;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null));

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.Name != null ? src.User.Name : null))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.Address != null ? $"{src.Address.FullName}, {src.Address.Street}, {src.Address.Ward}, {src.Address.District}, {src.Address.City}" : null))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod != null ? src.PaymentMethod.Name : null))
                .ForMember(dest => dest.DiscountCode, opt => opt.MapFrom(src => src.DiscountCode != null ? src.DiscountCode.Code : null));

            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderDetailCreateDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductColor != null && src.ProductColor.Variant != null && src.ProductColor.Variant.Product != null ? src.ProductColor.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColor != null ? src.ProductColor.ColorName : null));

            CreateMap<User, UserResponseDto>();
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));
            CreateMap<CategoryCreateDto, Category>();

            CreateMap<Brand, BrandResponseDto>();
            CreateMap<BrandCreateDto, Brand>();

            CreateMap<CartItem, CartItemResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductColor != null && src.ProductColor.Variant != null && src.ProductColor.Variant.Product != null ? src.ProductColor.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColor != null ? src.ProductColor.ColorName : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductColor != null ? src.ProductColor.SalePrice : (decimal?)null));

            CreateMap<CartItemCreateDto, CartItem>();

            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductColor != null && src.ProductColor.Variant != null && src.ProductColor.Variant.Product != null ? src.ProductColor.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColor != null ? src.ProductColor.ColorName : null));

            CreateMap<InventoryUpdateDto, Inventory>();

            CreateMap<DiscountCode, DiscountCodeResponseDto>();
            CreateMap<DiscountCodeCreateDto, DiscountCode>();

            CreateMap<PaymentMethod, PaymentMethodResponseDto>();
            CreateMap<PaymentMethodCreateDto, PaymentMethod>();

            CreateMap<Review, ReviewResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.Name != null ? src.User.Name : null));

            CreateMap<ReviewCreateDto, Review>();

            CreateMap<Question, QuestionResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.Name != null ? src.User.Name : null));

            CreateMap<QuestionCreateDto, Question>();

            CreateMap<Feedback, FeedbackResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.Name != null ? src.User.Name : null));

            CreateMap<FeedbackCreateDto, Feedback>();

            CreateMap<ProductVariant, ProductVariantResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<ProductVariantCreateDto, ProductVariant>();

            CreateMap<ProductColor, ProductColorResponseDto>()
                .ForMember(dest => dest.VariantStorage, opt => opt.MapFrom(src => src.Variant != null ? src.Variant.Storage : null));

            CreateMap<ProductColorCreateDto, ProductColor>();

            CreateMap<ProductSale, ProductSaleResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductColor != null && src.ProductColor.Variant != null && src.ProductColor.Variant.Product != null ? src.ProductColor.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColor != null ? src.ProductColor.ColorName : null));

            CreateMap<ProductSaleCreateDto, ProductSale>();

            CreateMap<ProductSpecification, ProductSpecificationResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<ProductSpecificationCreateDto, ProductSpecification>();

            CreateMap<Image, ImageResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<ImageCreateDto, Image>();
        }
    }
}
