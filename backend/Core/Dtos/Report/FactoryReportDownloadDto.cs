using System.Collections.Generic;

namespace Core.Dtos.Report;

public class FactoryReportDownloadDto
{
    public string SpecificationTypeName { get; set; }
    public string SpecificationName { get; set; }
    public string SpecificationOrderNo { get; set; }
    public string InitialDate { get; set; }
    public string OrderNo { get; set; }
    public string ProductDate { get; set; }
    public string ChemicalDate { get; set; }
    public string TestDate { get; set; }
    public string MeasureMethod { get; set; }
    public string Factory { get; set; }
    public int FilterGroups { get; set; }
    public List<IndicatorDataItem> IndicatorDataItems { get; set; }
    public List<IndicatorDataItem> ChemicalDataItems { get; set; }
    public List<FilterDataItem> FilterDataItems { get; set; }
    public List<FilterDataItem> OtherFilterDataItems { get; set; }
    public List<FilterGroupDataItem> FilterGroupDataItems { get; set; }
    public List<int> FilterBurstItems { get; set; }
    public List<int> FilterGlueHoleItems { get; set; }
    public List<int> FilterPeculiarSmellItems { get; set; }
    public List<int> FilterInnerBondLineItems { get; set; }
    public List<int> FilterOtherItems { get; set; }
    public string FilterGroupWater { get; set; }
    public string Menu { get; set; }
    public string Checker { get; set; }
    public string Result { get; set; }
    public int ReportType { get; set; }
    public string Count { get; set; }
    public string AuditUser { get; set; }

    public class IndicatorDataItem
    {
        public int No { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Standard { get; set; }
        public List<string> Values { get; set; }
        public string Value { get; set; }
    }

    public class FilterDataItem
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Standard { get; set; }
        public string Max { get; set; }
        public string Min { get; set; }
        public string Mean { get; set; }
        public string Sd { get; set; }
        public string Cv { get; set; }
        public string Value { get; set; }
        public int UnQualified { get; set; }
        public string Judgment { get; set; }
        public string Remark { get; set; }
    }

    public class FilterGroupDataItem
    {
        public string Name { get; set; }
        public string Standard { get; set; }
        public string ItemName { get; set; }
        public List<string> MaxList { get; set; } = new();
        public List<string> MinList { get; set; } = new();
        public List<string> MeanList { get; set; } = new();
        public List<string> SdList { get; set; } = new();
        public List<int> UnqualifiedList { get; set; } = new();
    }
}