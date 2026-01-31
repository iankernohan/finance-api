using System;

namespace finance_api.Dtos;

public class UpdateCategoryRequest
{
    public required int CategoryId { get; set; }

    public required string TransactionId { get; set; }
}
