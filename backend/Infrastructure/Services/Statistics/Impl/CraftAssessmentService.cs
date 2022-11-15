using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.CraftAssessment;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.Statistics.Impl;

public class CraftAssessmentService : ICraftAssessmentService
{
    private readonly IRepository<CraftReport> _crRepo;
    private readonly IRepository<MonthCraftReport> _mcrRepo;
    private readonly Settings _settings;

    public CraftAssessmentService(IRepository<CraftReport> crRepo,
        IRepository<MonthCraftReport> mcrRepo,
        IOptions<Settings> settings)
    {
        _crRepo = crRepo;
        _mcrRepo = mcrRepo;
        _settings = settings.Value;
    }

    private IEnumerable<TableResultDto> getData(QueryInfoDto dto)
    {
        var begin = Convert.ToDateTime(dto.BeginDate);
        var end = Convert.ToDateTime(dto.EndDate);
        var tempList = _crRepo.All().Where(c => c.Group.BeginTime.Date >= begin &&
                                                c.Group.EndTime.Date <= end).Select(c => new
        {
            WorkShopId = c.Group.MachineModel.WorkShopId, WorkShopName = c.Group.MachineModel.WorkShop.Name,
            MachineModelId = c.Group.MachineModelId, MachineName = c.Group.MachineModel.Name,
            Score = c.Score, TypeId = c.Group.MeasureTypeId
        }).ToList();

        var tableResultList = new List<TableResultDto>();
        var workShopGroups = tempList.GroupBy(c => c.WorkShopId).ToList();
        foreach (var workShopGroup in workShopGroups)
        {
            var firstWorkShop = workShopGroup.FirstOrDefault();
            if (firstWorkShop == null)
                continue;

            double mole = 0;
            double deno = 0;
            var tempResultList = new List<TableResultDto>();
            var index = 0;
            var machineModelGroups = workShopGroup.GroupBy(c => c.MachineModelId).ToList();
            foreach (var groups in machineModelGroups)
            {
                var firstGroup = groups.FirstOrDefault();
                if (firstGroup == null)
                    continue;
                var temp = new TableResultDto()
                {
                    WorkShopName = firstWorkShop.WorkShopName,
                    WorkShopNameCount = index == 0 ? machineModelGroups.Count : 0,
                    MachineModelName = firstGroup.MachineName,
                    MachineModelRowCount = 1
                };

                var thirdScoreList = groups.Where(c => c.TypeId == _settings.ThirdCraftTypeId).Select(c => c.Score)
                    .ToList();
                var secondScoreList = groups.Where(c => c.TypeId == _settings.SecondCraftTypeId).Select(c => c.Score)
                    .ToList();
                if (thirdScoreList.Count > 0)
                {
                    temp.ThirdScore = Math.Round(thirdScoreList.Average(), 2);
                    temp.ThirdScoreRowCount = 1;
                }
                else
                {
                    temp.ThirdScore = -1;
                    temp.ThirdScoreRowCount = 1;
                }

                if (secondScoreList.Count > 0)
                {
                    temp.SecondScore = Math.Round(secondScoreList.Average(), 2);
                    temp.SecondScoreRowCount = 1;
                }
                else
                {
                    temp.SecondScore = -1;
                    temp.SecondScoreRowCount = 1;
                }

                tempResultList.Add(temp);
                index++;
            }

            var thirdScoreLs = tempResultList.Where(c=>c.ThirdScore > -1).Select(c => c.ThirdScore).ToList();
            var secondScoreLs = tempResultList.Where(c => c.SecondScore > -1).Select(c => c.SecondScore).ToList();
            double thirdScoreMean = 0;
            double secondScoreMean = 0;
            double firstScoreMean = 0;
            if (thirdScoreLs.Count > 0)
            {
                var mean = Math.Round(thirdScoreLs.Average(), 2);
                thirdScoreMean = mean;
                tempResultList[0].ThirdMeanScore = mean;
                tempResultList[0].ThirdMeanScoreRowCount = tempResultList.Count;
            }
            else
            {
                tempResultList[0].ThirdMeanScore = -1;
                tempResultList[0].ThirdMeanScoreRowCount = tempResultList.Count;
            }

            if (secondScoreLs.Count > 0)
            {
                var mean = Math.Round(secondScoreLs.Average(), 2);
                secondScoreMean = mean;
                tempResultList[0].SecondMeanScore = mean;
                tempResultList[0].SecondMeanScoreRowCount = tempResultList.Count;
            }
            else
            {
                tempResultList[0].SecondMeanScore = -1;
                tempResultList[0].SecondMeanScoreRowCount = tempResultList.Count;
            }
            
            if (thirdScoreMean > 0)
            {
                mole += thirdScoreMean * 0.25;
                deno += 0.25;
            }

            if (secondScoreMean > 0)
            {
                mole += secondScoreMean * 0.25;
                deno += 0.25;
            }
            var firstCraftScoreList = _mcrRepo.All().Where(c => c.PartName == firstWorkShop.WorkShopName &&
                                                                c.Time.Date >= begin &&
                                                                c.Time.Date <= end)
                .Select(c => c.Score).ToList();
            if (firstCraftScoreList.Count > 0)
            {
                firstScoreMean = Math.Round(firstCraftScoreList.Average());
                if (firstScoreMean > 0)
                {
                    mole += firstScoreMean * 0.25;
                    deno += 0.25;
                }
            }
            var i = 0;
            foreach (var tempResult in tempResultList)
            {
                tempResult.FirstScore = firstScoreMean;
                tempResult.FirstScoreRowCount = i == 0 ? tempResultList.Count : 0;

                tempResult.CraftScore = Math.Round(mole / deno, 2);
                tempResult.CraftScoreRowCount = i == 0 ? tempResultList.Count : 0;

                i++;
            }

            if (tempResultList.Count > 0)
                tableResultList.AddRange(tempResultList);
        }

        return tableResultList;
    }

    public IEnumerable<TableResultDto> Search(QueryInfoDto dto)
    {
        return getData(dto);
    }

    public MemoryStream Download(QueryInfoDto dto)
    {
        var data = getData(dto);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("车间工艺考核");
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        ws.Cells.Style.WrapText = true;

        var columns = new List<string>() { "生产车间", "机台名称", "三级工艺巡检得分", "三级工艺巡检平均得分", "二级工艺巡检得分", "二级工艺巡检平均得分", "一级工艺检查得分", "工艺综合得分" };
        var headerCol = 1;
        foreach (var column in columns)
        {
            ws.Column(headerCol).Width = 25;
            ws.Cells[1, headerCol].Value = column;
            ws.Cells[1, headerCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            headerCol++;
        }

        ws.Row(1).Style.Font.Bold = true;
        ws.Row(1).Height = 30;

        var row = 2;
        var col = 1;
        foreach (var item in data)
        {
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.WorkShopNameCount > 0)
            {
                ws.Cells[row, col, row + item.WorkShopNameCount - 1, col].Merge = true;
                ws.Cells[row, col, row + item.WorkShopNameCount - 1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }
            ws.Cells[row, col++].Value = item.WorkShopName;
            
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, col++].Value = item.MachineModelName;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, col++].Value = item.ThirdScore.Equals(-1) ? "无数据" : item.ThirdScore;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.ThirdMeanScoreRowCount > 0)
            {
                ws.Cells[row, col, row + item.ThirdMeanScoreRowCount - 1, col].Merge = true;
                ws.Cells[row, col, row + item.ThirdMeanScoreRowCount - 1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }
            ws.Cells[row, col++].Value = item.ThirdMeanScore.Equals(-1) ? "无数据" : item.ThirdMeanScore;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, col++].Value = item.SecondScore.Equals(-1) ? "无数据" : item.SecondScore;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.SecondMeanScoreRowCount > 0)
            {
                ws.Cells[row, col, row + item.SecondMeanScoreRowCount - 1, col].Merge = true;
                ws.Cells[row, col, row + item.SecondMeanScoreRowCount - 1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }
            ws.Cells[row, col++].Value = item.SecondMeanScore.Equals(-1) ? "无数据" : item.SecondMeanScore;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.FirstScoreRowCount > 0)
            {
                ws.Cells[row, col, row + item.FirstScoreRowCount - 1, col].Merge = true;
                ws.Cells[row, col, row + item.FirstScoreRowCount - 1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }
            ws.Cells[row, col++].Value = item.FirstScore.Equals(-1) ? "无数据" : item.FirstScore;
            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.CraftScoreRowCount > 0)
            {
                ws.Cells[row, col, row + item.CraftScoreRowCount - 1, col].Merge = true;
                ws.Cells[row, col, row + item.CraftScoreRowCount - 1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }
            ws.Cells[row, col++].Value = item.CraftScore;

            ws.Row(row).Height = 30;
            row++;
            col = 1;
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }
}