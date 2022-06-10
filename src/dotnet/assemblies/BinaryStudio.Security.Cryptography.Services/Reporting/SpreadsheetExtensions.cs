using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public static class SpreadsheetExtensions
        {
        #region M:AppendChildMergeCells(MergeCell[]):MergeCells
        public static MergeCells AppendChildMergeCells(this Worksheet Worksheet, params MergeCell[] values) {
            return Worksheet.AppendChild(new MergeCells(values.OfType<OpenXmlElement>())
                {
                Count = (UInt32)values.Length
                });
            }
        #endregion
        #region M:AppendChildMergeCells(IEnumerable<MergeCell>):MergeCells
        public static MergeCells AppendChildMergeCells(this Worksheet Worksheet, IEnumerable<MergeCell> values) {
            var r = values.OfType<OpenXmlElement>().ToArray();
            return Worksheet.AppendChild(new MergeCells(r)
                {
                Count = (UInt32)r.Length
                });
            }
        #endregion
        #region M:AppendChildColumns(Column[]):Columns
        public static Columns AppendChildColumns(this Worksheet Worksheet, params Column[] values) {
            return Worksheet.AppendChild(new Columns(values.OfType<OpenXmlElement>())
                {
                });
            }
        #endregion

        public static Cell UpdateCellValue(this Cell target, Object value) {
            if (value != null) {
                var type = value.GetType();
                switch (value.GetType().Name) {
                    case nameof(String)         : target.DataType = CellValues.String;  target.CellValue = new CellValue((String)value);         break;
                    case nameof(Int32)          : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Int32)value);          break;
                    case nameof(DateTime)       : target.DataType = CellValues.Date;    target.CellValue = new CellValue((DateTime)value);       break;
                    case nameof(DateTimeOffset) : target.DataType = CellValues.Date;    target.CellValue = new CellValue((DateTimeOffset)value); break;
                    case nameof(Boolean)        : target.DataType = CellValues.Boolean; target.CellValue = new CellValue((Boolean)value);        break;
                    case nameof(Double)         : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Double)value);         break;
                    case nameof(Single)         : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Single)value);         break;
                    case nameof(Decimal)        : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Decimal)value);        break;
                    case nameof(Guid)           : target.DataType = CellValues.String;  target.CellValue = new CellValue(value.ToString());      break;
                    case nameof(CellFormula)    : target.CellFormula = (CellFormula)value;                                                       break;
                    default:
                        {
                        return UpdateCellValue(target, value.ToString());
                        }
                    }
                }
            else
                {
                target.CellValue = new CellValue();
                }
            return target;
            }
        }
    }