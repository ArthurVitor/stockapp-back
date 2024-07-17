using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.Parameters;
using StockApp.Models.Models;

namespace StockApp.Tests.Utils;

public static class CreateArrange
{
    public static ListInventoryDto CreateListInventoryDto(List<ListInventoryBatchDto> batches, int id = 1, string name = "Inventory1")
    {
        return new ListInventoryDto
        {
            Id = id,
            Name = name,
            Batches = batches
        };
    }

    public static ListExitNoteDto CreateListExitNoteDto(double quantity = 10, int productId = 1, int inventoryId = 1)
    {
        return new ListExitNoteDto
        {
            Quantity = quantity,
            ProductId = productId,
            InventoryId = inventoryId
        };
    }

    public static Inventory CreateInventory(int id = 1, string name = "Inventory1")
    {
        return new Inventory
        {
            Id = id,
            Name = name
        };
    }
    public static Product CreateProduct(int id = 1, string name = "product", Models.Enum.CategoryEnum category = Models.Enum.CategoryEnum.Grains)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Category = category
        };
    }

    public static ListEntryNoteDto CreateListEntryNoteDto(DateTime noteGenerationTime, DateTime? expiryDate, double price = 10,
       double quantity = 10, int productId = 1, int inventoryId = 1, int id = 1)
    {
        return new ListEntryNoteDto()
        {
            Id = id,
            NoteGenerationTime = noteGenerationTime,
            ExpiryDate = expiryDate,
            Price = price,
            Quantity = quantity,
            ProductId = productId,
            InventoryId = inventoryId
        };
    }

    public static CreateEntryNoteDto CreateEntryNoteDto(double price = 10, double quantity = 10, int productId = 1, int inventoryId = 1)
    {
        return new CreateEntryNoteDto()
        {
            Price = price,
            Quantity = quantity,
            ProductId = productId,
            InventoryId = inventoryId
        };
    }

    public static Batch CreateBatch(int id = 1, double quantity = 10, double price = 10, int productId = 1, int inventoryId = 1, int entryNoteId = 1, DateTime? expiryDate = null)
    {
        return new Batch
        {
            Id = id,
            Price = price,
            Quantity = quantity,
            ExpiryDate = expiryDate,
            ProductId = productId,
            InventoryId = inventoryId,
            EntryNoteId = entryNoteId,
            TotalValue = price * quantity,
        };
    }

    public static EntryNote CreateEntryNote(int id = 1, double price = 10, double quantity = 10, int productId = 1, int inventoryId = 1)
    {
        return new EntryNote
        {
            Id = id,
            Price = price,
            Quantity = quantity,
            ProductId = productId,
            InventoryId = inventoryId
        };
    }

    public static ExitNote CreateExitNote(int id = 1, int productId = 1, int inventoryId = 1, double quantity = 10, List<ExitNoteBatch>? exitNoteBatches = null)
    {
        exitNoteBatches ??= [];

        return new ExitNote
        {
            Id = id,
            ProductId = productId,
            InventoryId = inventoryId,
            Quantity = quantity, 
            ExitNoteBatches = exitNoteBatches
        };
    }

    public static CreateExitNoteDto CreateExitNoteDto(int productId = 1, int inventoryId = 1, double quantity = 10)
    {
        return new CreateExitNoteDto
        {
            ProductId = productId,
            InventoryId = inventoryId,
            Quantity = quantity
        };
    }

    public static Parameters CreateParameter(int productId = 1, double maximumAmount = 100,
      double minimumAmount = 0, int inventoryId = 1)
    {
        return new Parameters
        {
            ProductId = productId,
            MaximumAmount = maximumAmount,
            MinimumAmount = minimumAmount,
            InventoryId = inventoryId
        };
    }

    public static CreateParameterDto CreateParameterDto(int productId = 1, double maximumAmount = 100,
        double minimumAmount = 0, int inventoryId = 1)
    {
        return new CreateParameterDto
        {
            ProductId = productId,
            MaximumAmount = maximumAmount,
            MinimumAmount = minimumAmount,
            InventoryId = inventoryId
        };
    }

    public static ListParameterDto CreateListParameterDto(int productId = 1, double maximumAmount = 100,
       double minimumAmount = 0, int inventoryId = 1)
    {
        return new ListParameterDto
        {
            ProductId = productId,
            MaximumAmount = maximumAmount,
            MinimumAmount = minimumAmount,
            InventoryId = inventoryId
        };
    }

    public static ExitNoteBatch CreateExitNoteBatch(Batch batch, int batchId = 1, int exitNoteId = 1, double quantity = 10)
    {
        return new ExitNoteBatch
        {
            BatchId = batchId,
            ExitNoteId = exitNoteId,
            Batch = batch,
            Quantity = quantity
        };
    }

    public static CreateReallocateProductDto CreateReallocateProductDto(DateTime expiryDate,  int senderInventoryId = 1, int recipientInventoryId = 2, int productId = 1, double quantity = 10, double price = 5)
    {
        return new CreateReallocateProductDto
        {
            ExpiryDate = expiryDate,
            SenderInventoryId = senderInventoryId,
            RecipientInventoryId = recipientInventoryId,
            ProductId = productId,
            Quantity = quantity,
            Price = price
        }; 
    }
}
