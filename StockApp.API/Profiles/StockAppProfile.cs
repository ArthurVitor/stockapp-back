using AutoMapper;
using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.EntryNoteDto;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.OverrideDto;
using StockApp.Models.Dtos.Parameters;
using StockApp.Models.Dtos.ProductDto;
using StockApp.Models.Dtos.SubCategoryDto;
using StockApp.Models.Dtos.Transactions;
using StockApp.Models.Models;

namespace StockApp.API.Profiles;

public class StockAppProfile : Profile
{
    public StockAppProfile()
    {
        // Product
        CreateMap<Product, ListProductDTO>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<CreateImportingProductDto, Product>();

        // SubCategory
        CreateMap<CreateSubCategoryDto, SubCategory>();
        CreateMap<SubCategory, CreateSubCategoryDto>();
        CreateMap<SubCategory, ListSubCategoryDto>();
        CreateMap<IEnumerable<SubCategory>, List<ListSubCategoryDto>>()
            .ConvertUsing(src => src.Select(sc => new ListSubCategoryDto
            {
                Id = sc.Id,
                Name = sc.Name
            }).ToList());
        CreateMap<IEnumerable<CreateSubCategoryDto>, List<SubCategory>>()
            .ConvertUsing(src => src.Select(sc => new SubCategory()
            {
                Name = sc.Name
            }).ToList());

        // EntryNote
        CreateMap<EntryNote, ListEntryNoteDto>().ReverseMap();
        CreateMap<CreateEntryNoteDto, EntryNote>().ReverseMap();
        CreateMap<CreateReallocateProductDto, CreateEntryNoteDto>()
            .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.RecipientInventoryId));

        // Batch
        CreateMap<EntryNote, CreateBatchDto>()
           .ForMember(dest => dest.EntryNoteId, opt => opt.MapFrom(src => src.Id)).ReverseMap();
        CreateMap<CreateBatchDto, Batch>().ReverseMap();
        CreateMap<Batch, ListInventoryBatchDto>().ReverseMap();
        CreateMap<Batch, ListExitNoteBatchDto>().ReverseMap();
        CreateMap<ExitNoteBatch, CreateEntryNoteDto>()
                    .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.Batch.ExpiryDate))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Batch.Price))
                    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Batch.ProductId))
                    .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.Batch.InventoryId));

        CreateMap<IEnumerable<Batch>, List<ListBatchDto>>()
            .ConvertUsing(src => src.Select(bc => new ListBatchDto()
            {
                ProductId = bc.ProductId,
                Id = bc.Id,
                Quantity = bc.Quantity,
                InventoryId = bc.InventoryId,
                Price = bc.Price,
                ExpiryDate = bc.ExpiryDate,
                EntryNoteId = bc.EntryNoteId,
                TotalValue = bc.TotalValue
            }).ToList());
        
        // Inventory
        CreateMap<CreateInventoryDto, Inventory>().ReverseMap();
        CreateMap<Inventory, ListInventoryDto>()
            .ForMember(dest => dest.Batches, opt => opt.MapFrom(src => src.Batches));
        CreateMap<ListBatchDto, Batch>().ReverseMap();

        //Parameter
        CreateMap<Parameters, ListParameterDto>();
        CreateMap<CreateParameterDto, Parameters>();

        //ExitNote
        CreateMap<CreateExitNoteDto, ExitNote>().ReverseMap();
        CreateMap<ExitNote, ListExitNoteDto>()
            .ForMember(dest => dest.Batches, opt => opt.MapFrom(src => src.ExitNoteBatches.Select(enb => new ListExitNoteBatchDto
            {
                Id = enb.BatchId,
                ProductId = enb.Batch.ProductId,
                Quantity = enb.Quantity
            }))).ReverseMap();
        CreateMap<CreateReallocateProductDto, CreateExitNoteDto>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.SenderInventoryId));
        CreateMap<CreateExitNoteDto, EntryNote>().ReverseMap(); 

        // ExitNoteBatch
        CreateMap<ExitNoteBatch, ListExitNoteBatchDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BatchId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Batch.ProductId)).ReverseMap();
        
        // Transactions
        CreateMap<Transactions, ListTransactionsDto>();

        //Override
        CreateMap<Override, ListOverrideDto>();

        //EntryNote reversed    
        CreateMap<(List<ListEntryNoteDto> EntryNotes, List<ListBatchDto> Batches), EntryNotesReversedDto>()
            .ForMember(dest => dest.EntryNotes, opt => opt.MapFrom(src => src.EntryNotes))
            .ForMember(dest => dest.Batches, opt => opt.MapFrom(src => src.Batches));
    }
}
