using System;

namespace ExamTwo.Models;

public class OrderRequest
{
    public Dictionary<string, int> Order { get; set; } = new();
    public Payment Payment { get; set; } = new();
}