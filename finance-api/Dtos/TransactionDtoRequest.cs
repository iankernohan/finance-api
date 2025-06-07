using System;

namespace finance_api.Dtos;

public class TransactionDtoRequest
{
    public double Amount { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
}
