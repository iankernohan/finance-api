using System;

namespace finance_api.Models;

public class Transaction
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public DateTime DateCreated { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }

    public Category? Category { get; set; }
    public SubCategory? SubCategory { get; set; }
}
