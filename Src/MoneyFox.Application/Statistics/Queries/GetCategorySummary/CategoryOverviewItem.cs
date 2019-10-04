﻿namespace MoneyFox.Application.Statistics.Queries.GetCategorySummary
{
    public class CategoryOverviewItem
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public decimal Average { get; set; }
        public decimal Percentage { get; set; }
    }
}
