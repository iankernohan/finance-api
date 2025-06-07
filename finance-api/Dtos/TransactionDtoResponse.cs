using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class TransactionDtoResponse
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public CategoryDtoResponse Category { get; set; } = new CategoryDtoResponse();
    public SubCategory? SubCategory { get; set; }
}
