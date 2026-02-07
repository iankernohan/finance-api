using System;

namespace finance_api.Dtos;

public class UpdateTransactionCategoryRequest
{
    public required int CategoryId { get; set; }

    public required string TransactionId { get; set; }
}
