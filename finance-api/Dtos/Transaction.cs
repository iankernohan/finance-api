using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class Transaction
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public Category Category { get; set; } = new Category();
    public SubCategory SubCategory { get; set; } = new SubCategory();
}
