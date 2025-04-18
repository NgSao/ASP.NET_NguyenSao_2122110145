using AutoMapper;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses));
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Address, AddressResponseDto>();
            CreateMap<AddressCreateDto, Address>();
            CreateMap<AddressUpdateDto, Address>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Category, CategoryResponseDto>()
             .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Brand, BrandResponseDto>();
            CreateMap<BrandCreateDto, Brand>();
            CreateMap<BrandUpdateDto, Brand>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null));
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Media, MediaResponseDto>();
            CreateMap<MediaRequestDto, Media>();


            CreateMap<Variant, VariantResponseDto>();
            CreateMap<VariantCreateDto, Variant>();
            CreateMap<VariantUpdateDto, Variant>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));



            CreateMap<Color, ColorResponseDto>();

            CreateMap<ColorCreateDto, Color>();
            CreateMap<ColorUpdateDto, Color>()
                .ForMember(dest => dest.Media, opt =>
                {
                    opt.PreCondition(src => src.Media != null);
                    opt.MapFrom(src => src.Media);
                })
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ProductSpecification, ProductSpecificationResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
            CreateMap<ProductSpecificationUpdateDto, ProductSpecification>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProductSpecificationCreateDto, ProductSpecification>();

            CreateMap<CartItem, CartItemResponseDto>()
                       .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Color != null && src.Color.Variant != null && src.Color.Variant.Product != null ? src.Color.Variant.Product.Name : null))
                       .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null))
                       .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Color != null ? src.Color.SalePrice : (decimal?)null));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.FullName != null ? src.User.FullName : null))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.Address != null ? $"{src.Address.FullName}, {src.Address.AddressDetail}, {src.Address.Ward}, {src.Address.District}, {src.Address.City}" : null))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod != null ? src.PaymentMethod.Name : null))
                .ForMember(dest => dest.DiscountCode, opt => opt.MapFrom(src => src.DiscountCode != null ? src.DiscountCode.Code : null));

            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderDetailCreateDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Color != null && src.Color.Variant != null && src.Color.Variant.Product != null ? src.Color.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null));




            CreateMap<CartItemCreateDto, CartItem>();

            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Color != null && src.Color.Variant != null && src.Color.Variant.Product != null ? src.Color.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null));

            CreateMap<InventoryUpdateDto, Inventory>();

            CreateMap<DiscountCode, DiscountCodeResponseDto>();
            CreateMap<DiscountCodeCreateDto, DiscountCode>();

            CreateMap<PaymentMethod, PaymentMethodResponseDto>();
            CreateMap<PaymentMethodCreateDto, PaymentMethod>();

            CreateMap<Review, ReviewResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.FullName != null ? src.User.FullName : null));

            CreateMap<ReviewCreateDto, Review>();

            CreateMap<Question, QuestionResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.FullName != null ? src.User.FullName : null));

            CreateMap<QuestionCreateDto, Question>();

            CreateMap<Feedback, FeedbackResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null && src.User.FullName != null ? src.User.FullName : null));

            CreateMap<FeedbackCreateDto, Feedback>();


            CreateMap<Sale, ProductSaleResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Color != null && src.Color.Variant != null && src.Color.Variant.Product != null ? src.Color.Variant.Product.Name : null))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null));

            CreateMap<ProductSaleCreateDto, Sale>();





        }
    }
}
