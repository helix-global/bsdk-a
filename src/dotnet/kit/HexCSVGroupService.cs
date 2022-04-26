using System;
using System.Collections.Generic;
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

    /// <inheritdoc/>
    public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption)
        {
        var fileindex = 0;
        var culture = CultureInfo.GetCultureInfo(GetSystemDefaultLCID());
        var filename = $"hexgroup-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv";
        using (var stream = File.OpenWrite(filename))
        using (var target = new StreamWriter(stream, Encoding.UTF8)) {
            target.WriteLine("IdentifyDocumentId;DocumentCategoryName;CountryName;CountryICAO;Year;Month;RegisterCode;RegisterNumber;IssueDate;ValidToDate;InscribeId;DataFormatId;DataTypeId;Order;Dense;Size");
            using (var sourcestream = Source.OpenRead())
            using (var reader = new CSVDataReader(
                    new StreamReader(sourcestream, Encoding.GetEncoding(culture.TextInfo.ANSICodePage)), ";",
                    null, CultureInfo.CurrentCulture, Encoding.ASCII,
                    CSVDataReaderFlags.Delimited|
                    CSVDataReaderFlags.FirstRowContainsFieldNames, 0, Environment.NewLine)) {
                while (reader.Read())
                    {
                    var DocumentCategoryName = ((String)reader["DocumentCategoryName"]).Trim();
                    var CountryName = ((String)reader["CountryName"]).Trim();
                    var CountryICAO = GetCountry(((String)reader["CountryICAO"]).Trim());
                    var Month = ((String)reader["Month"]).Trim();
                    var IdentifyDocumentId = ((String)reader["IdentifyDocumentID"]).Trim();
                    var RegisterCode = ((String)reader["RegisterCod"]).Trim();
                    var RegisterNumber = ((String)reader["RegisterNumber"]).Trim();
                    var IssueDate = ((String)reader["IssueDate"]).Trim();
                    var Year = IssueDate.Substring(0,4);
                    var ValidToDate = ((String)reader["ValidToDate"]).Trim();
                    var InscribeId = ((String)reader["InscribeID"]).Trim();
                    var DataFormatId = ((String)reader["DataFormatID"]).Trim();
                    var DataTypeId = ((String)reader["DataTypeID"]).Trim();
                    var Order = ((String)reader["Ord"]).Trim();
                    var Dense = ((String)reader["DENSE"]).Trim();
                    var Body = ToArray((String)reader["CHIP_1D"]);
                    var r = new HexFile(Body,
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
                        Dense = Dense
                        };
                    if (PathUtils.IsMatch(searchpattern, r.FileName))
                        {
                        yield return r;
                        target.WriteLine($"{r.IdentifyDocumentId};{DocumentCategoryName};{CountryName};{CountryICAO};{Year};{Month};{RegisterCode};{RegisterNumber};{IssueDate};{ValidToDate};{InscribeId};{DataFormatId};{DataTypeId};{Order};{Dense};{Body.Length}");
                        fileindex++;
                        }
                    }
                }
            }
        yield break;
        }

    [DllImport("kernel32.dll")] private static extern Int32 GetSystemDefaultLCID();

    private static Byte[] ToArray(String source)
        {
        if (source == "NULL") { return new Byte[0]; }
        if (String.IsNullOrWhiteSpace(source)) { return new Byte[0]; }
        return Convert.FromBase64String(source);
        }
    }
