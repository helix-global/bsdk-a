using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.DataProcessing;
using BinaryStudio.DirectoryServices;

internal class HexCSVGroupService : IDirectoryService
    {
    public IFileService Source { get; }
    public HexCSVGroupService(IFileService source)
        {
        Source = source;
        }

    private static String GetCountry(String value) {
        if (String.IsNullOrWhiteSpace(value)) { return null; }
        if (value.Length == 3) { return value.ToLowerInvariant(); }
        if (value.Length == 2)
            {
            if (IcaoCountry.TwoLetterCountries.TryGetValue(value, out var r)) {
                return r.ThreeLetterISOCountryName;
                }
            if (value == "РC") { return "rus"; }
            }
        return value;
        }

    #region M:ToString(Object):String
    private static String ToString(Object value) {
        return value.ToString().Trim();
        }
    #endregion
    #region M:ToInt32(Object):Int32?
    private static Int32? ToInt32(Object value) {
        if ((value == null) || (value is DBNull)) { return null; }
        if (Int32.TryParse(value.ToString().Trim(), out var r)) { return r; }
        return null;
        }
    #endregion
    #region M:ToString(IDataRecord,String):String
    private static String ToString(IDataRecord reader, String fieldname) {
        return (reader.GetOrdinal(fieldname) != -1)
            ? ToString(reader[fieldname])
            : null;
        }
    #endregion
    #region M:ToInt32(IDataRecord,String):Int32?
    private static Int32? ToInt32(IDataRecord reader, String fieldname) {
        return (reader.GetOrdinal(fieldname) != -1)
            ? ToInt32(reader[fieldname])
            : null;
        }
    #endregion

    /// <inheritdoc/>
    public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption)
        {
        var fileindex = 0;
        var culture = CultureInfo.GetCultureInfo(GetSystemDefaultLCID());
        var filename = $"hexgroup-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv";
        using (var stream = File.OpenWrite(filename))
        using (var target = new StreamWriter(stream, Encoding.UTF8)) {
            target.WriteLine("IdentifyDocumentId;DocumentCategoryName;CountryName;CountryICAO;Year;Month;RegisterCode;RegisterNumber;IssueDate;ValidToDate;InscribeId;DataFormatId;DataTypeId;Order;Dense;XmlSize,Size,EFName");
            using (var sourcestream = Source.OpenRead())
            using (var reader = new CSVDataReader(
                    new StreamReader(sourcestream, Encoding.GetEncoding(culture.TextInfo.ANSICodePage)), ";",
                    null, CultureInfo.CurrentCulture, Encoding.ASCII,
                    CSVDataReaderFlags.Delimited|
                    CSVDataReaderFlags.FirstRowContainsFieldNames|
                    CSVDataReaderFlags.HasMultiLineValue, 0, Environment.NewLine)) {
                while (reader.Read()) {
                    var IdentifyDocumentId   = ToString(reader,"IdentifyDocumentID");
                    var DocumentCategoryName = ToString(reader["DocumentCategoryName"]);
                    var CountryName          = ToString(reader["CountryName"]);
                    var CountryICAO          = GetCountry(ToString(reader["CountryICAO"]));
                    var RegisterCode         = ToString(reader["RegisterCod"]);
                    var RegisterNumber       = ToString(reader["RegisterNumber"]);
                    var IssueDate            = ToString(reader["IssueDate"]);
                    var Year                 = (IssueDate == "NULL") ? "NULL": IssueDate.Substring(0,4);
                    var Month                = (IssueDate == "NULL") ? "NULL": IssueDate.Substring(5,2);
                    var ValidToDate          = ToString(reader["ValidToDate"]);
                    var InscribeId           = ToString(reader,"InscribeID");
                    var DataFormatId         = ToString(reader,"DataFormatID");
                    var DataTypeId           = ToString(reader,"DataTypeID");
                    var Order                = ToString(reader,"Ord");
                    var Dense                = ToString(reader,"DENSE");
                    var XmlSize              = ToInt32(reader,"XML_SIZE");
                    var CHIP_1D              = ToArray(ToString(reader["CHIP_1D"]));
                    var CHIP_77              = ToArray(ToString(reader["CHIP_77"]));
                    var CHIP_E040            = ToArray(ToString(reader["CHIP_E040"]));

                    var r = new HexFile(CHIP_1D,
                        IdentifyDocumentId, fileindex,
                        Source.FileName)
                        {
                        DocumentCategoryName = DocumentCategoryName,
                        CountryName = CountryName,
                        CountryICAO = CountryICAO,
                        Year = Year,
                        Month = Month,
                        RegisterCode = RegisterCode,
                        RegisterNumber = RegisterNumber,
                        IssueDate = IssueDate,
                        ValidToDate = ValidToDate,
                        InscribeId = InscribeId,
                        DataFormatId = DataFormatId,
                        DataTypeId = DataTypeId,
                        Order = Order,
                        Dense = Dense,
                        XmlSize = XmlSize,
                        EFName = "1d"
                        };
                    if (PathUtils.IsMatch(searchpattern, r.FileName)) {
                        target.WriteLine($"{r.IdentifyDocumentId};{DocumentCategoryName};{CountryName};{CountryICAO};{Year};{Month};{RegisterCode};{RegisterNumber};{IssueDate};{ValidToDate};{InscribeId};{DataFormatId};{DataTypeId};{Order};{Dense};{r.Body?.Length.ToString()??"NULL"}");
                        if (r.Body != null) {
                            yield return r;
                            }
                        }
                    r = new HexFile(CHIP_77,
                        IdentifyDocumentId, fileindex,
                        Source.FileName)
                        {
                        DocumentCategoryName = DocumentCategoryName,
                        CountryName = CountryName,
                        CountryICAO = CountryICAO,
                        Year = Year,
                        Month = Month,
                        RegisterCode = RegisterCode,
                        RegisterNumber = RegisterNumber,
                        IssueDate = IssueDate,
                        ValidToDate = ValidToDate,
                        InscribeId = InscribeId,
                        DataFormatId = DataFormatId,
                        DataTypeId = DataTypeId,
                        Order = Order,
                        Dense = Dense,
                        XmlSize = XmlSize,
                        EFName = "77"
                        };
                    if (PathUtils.IsMatch(searchpattern, r.FileName)) {
                        target.WriteLine($"{r.IdentifyDocumentId};{DocumentCategoryName};{CountryName};{CountryICAO};{Year};{Month};{RegisterCode};{RegisterNumber};{IssueDate};{ValidToDate};{InscribeId};{DataFormatId};{DataTypeId};{Order};{Dense};{r.Body?.Length.ToString()??"NULL"}");
                        if (r.Body != null) {
                            yield return r;
                            }
                        }
                    r = new HexFile(CHIP_E040,
                        IdentifyDocumentId, fileindex,
                        Source.FileName)
                        {
                        DocumentCategoryName = DocumentCategoryName,
                        CountryName = CountryName,
                        CountryICAO = CountryICAO,
                        Year = Year,
                        Month = Month,
                        RegisterCode = RegisterCode,
                        RegisterNumber = RegisterNumber,
                        IssueDate = IssueDate,
                        ValidToDate = ValidToDate,
                        InscribeId = InscribeId,
                        DataFormatId = DataFormatId,
                        DataTypeId = DataTypeId,
                        Order = Order,
                        Dense = Dense,
                        XmlSize = XmlSize,
                        EFName = "e040"
                        };
                    if (PathUtils.IsMatch(searchpattern, r.FileName)) {
                        target.WriteLine($"{r.IdentifyDocumentId};{DocumentCategoryName};{CountryName};{CountryICAO};{Year};{Month};{RegisterCode};{RegisterNumber};{IssueDate};{ValidToDate};{InscribeId};{DataFormatId};{DataTypeId};{Order};{Dense};{r.Body?.Length.ToString()??"NULL"}");
                        if (r.Body != null) {
                            yield return r;
                            }
                        }
                    fileindex++;
                    }
                }
            }
        yield break;
        }

    [DllImport("kernel32.dll")] private static extern Int32 GetSystemDefaultLCID();

    private static Byte[] ToArray(String source)
        {
        if (source == "NULL") { return null; }
        if (String.IsNullOrWhiteSpace(source)) { return null; }
        return Convert.FromBase64String(source);
        }
    }
