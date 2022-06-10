using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using BinaryStudio.DirectoryServices;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ChainReport
        {
        private static Color InfoColor  = Color.FromArgb(0xff, 0xee, 0xff, 0xee);
        private static Color ErrorColor = Color.FromArgb(0xff, 0xff, 0xee, 0xee);
        private static Color HeaderColor = Color.FromArgb(0xff, 0xee, 0xee, 0xff);
        public String InputFolder { get; }
        public String OutputFolder { get; }
        public ChainReport(String inputfolder, String outputfolder) {
            InputFolder = inputfolder;
            OutputFolder = outputfolder;
            if (String.IsNullOrWhiteSpace(inputfolder)) {
                InputFolder = Directory.GetCurrentDirectory();
                }
            }

        private class CertificateInfo
            {
            public X509Certificate Certificate;
            public readonly ChainContextInfo ChainContext = new ChainContextInfo();
            }

        private class ChainElementInfo
            {
            public CertificateChainErrorStatus ErrorStatus;
            public CertificateChainInfoStatus  InfoStatus;
            public IX509Certificate Certificate;
            public IX509CertificateRevocationList CertificateRevocationList;
            }

        private class ChainInfo
            {
            public CertificateChainErrorStatus ErrorStatus;
            public CertificateChainInfoStatus  InfoStatus;
            public readonly List<ChainElementInfo> Elements = new List<ChainElementInfo>();
            }

        private class ChainContextInfo
            {
            public CertificateChainErrorStatus ErrorStatus;
            public CertificateChainInfoStatus  InfoStatus;
            public readonly List<ChainInfo> Chains = new List<ChainInfo>();
            }

        private static readonly CertificateChainInfoStatus[] DesiredInfoStatuses = new CertificateChainInfoStatus[]{
            CertificateChainInfoStatus.IsSelfSigned,
            CertificateChainInfoStatus.HasExactMatchIssuer,
            CertificateChainInfoStatus.HasKeyMatchIssuer,
            CertificateChainInfoStatus.HasNameMatchIssuer,
            CertificateChainInfoStatus.HasPreferredIssuer,
            CertificateChainInfoStatus.HasIssuanceChainPolicy,
            CertificateChainInfoStatus.HasValidNameConstraints,
            CertificateChainInfoStatus.HasCRLValidityExtended,
            CertificateChainInfoStatus.IsPeerTrusted,
            CertificateChainInfoStatus.IsFromExclusiveTrustStore,
            CertificateChainInfoStatus.IsComplexChain,
            CertificateChainInfoStatus.IsCATrusted
            };

        private static readonly CertificateChainErrorStatus[] DesiredErrorStatuses = new CertificateChainErrorStatus[]{
            CertificateChainErrorStatus.TrustIsNotTimeValid,
            CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED,
            CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED,
            CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID,
            CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE,
            CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT,
            CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN,
            CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC,
            CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION,
            CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS,
            CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS,
            CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS,
            CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT,
            CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT,
            CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT,
            CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT,
            CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION,
            CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY,
            CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST,
            CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT,
            CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN,
            CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID,
            CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID,
            CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE,
            CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE,
            CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE
            };

        private void Header(ReportCell cell, Color color, Object header)
            {
            cell.Value = header;
            cell.Background = color;
            }

        private void BuildReport(IDictionary<String,CertificateInfo> data) {
            var reportsource = new ExcelReportSource();
            var reportsheet = reportsource[0];
            var RowIndex = 0;
            Header(reportsheet[RowIndex][05],InfoColor, "Info");
            Header(reportsheet[RowIndex][17],ErrorColor, "Error");
            reportsheet[RowIndex][5].ColumnSpan = 12;
            reportsheet[RowIndex][5].HorizontalAlignment = ReportHorizontalAlignment.Center;
            reportsheet[RowIndex][17].ColumnSpan = 26;
            reportsheet[RowIndex][17].HorizontalAlignment = ReportHorizontalAlignment.Center;
            const Int32 InfoStatusFirstColumnIndex = 5;
            const Int32 ErrorStatusFirstColumnIndex = InfoStatusFirstColumnIndex + 12;
            for (var i = InfoStatusFirstColumnIndex + 1; i < ErrorStatusFirstColumnIndex + 25; i++) {
                reportsheet[RowIndex][i].BorderThickness = new ReportThickness{
                    Top  = BorderStyleValues.Thin,
                    Left = BorderStyleValues.Thin,
                    };
                }
            reportsheet[RowIndex][6].BorderThickness = new ReportThickness{ Top = BorderStyleValues.Thin };
            RowIndex++;
            Header(reportsheet[RowIndex][0],HeaderColor,"Subject");
            Header(reportsheet[RowIndex][1],HeaderColor,"ChainIndex");
            Header(reportsheet[RowIndex][2],HeaderColor,"ElementIndex");
            Header(reportsheet[RowIndex][3],HeaderColor,"ChainCount");
            Header(reportsheet[RowIndex][4],HeaderColor,"HasChainError");
            var ColumnIndex = 5;
            foreach (var i in DesiredInfoStatuses) {
                reportsheet[RowIndex][ColumnIndex].Value = i;
                reportsheet[RowIndex][ColumnIndex].Background = InfoColor;
                ColumnIndex++;
                }
            foreach (var i in DesiredErrorStatuses) {
                reportsheet[RowIndex][ColumnIndex].Value = i;
                reportsheet[RowIndex][ColumnIndex].Background = ErrorColor;
                ColumnIndex++;
                }
            foreach (var item in data) {
                RowIndex++;
                reportsheet[RowIndex][0].Value = item.Key;
                reportsheet[RowIndex][0].Font = new ReportFont {
                    FontName = "Lucida Sans Typewriter",
                    FontSize = 9,
                    Bold = true
                    };
                reportsheet[RowIndex][3].Value = item.Value.ChainContext.Chains.Count;
                reportsheet[RowIndex][4].Value = item.Value.ChainContext.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR;
                reportsheet[RowIndex][InfoStatusFirstColumnIndex     ].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  1].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  2].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  3].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  4].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  5].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  6].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  7].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  8].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex +  9].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex + 10].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain);
                reportsheet[RowIndex][InfoStatusFirstColumnIndex + 11].Value = item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  0].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  1].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  2].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  3].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  4].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  5].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  6].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  7].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  8].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  9].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 10].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 11].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 12].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 13].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 14].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 15].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 16].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 17].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 18].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 19].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 20].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 21].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 22].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 23].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 24].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE);
                reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 25].Value = item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE);
                var ChainIndex = 0;
                foreach (var chain in item.Value.ChainContext.Chains) {
                    RowIndex++;
                    reportsheet[RowIndex][1].Value = ChainIndex;
                    reportsheet[RowIndex][3].Value = chain.Elements.Count;
                    reportsheet[RowIndex][4].Value = chain.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR;
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex     ].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  1].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  2].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  3].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  4].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  5].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  6].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  7].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  8].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex +  9].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex + 10].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain);
                    reportsheet[RowIndex][InfoStatusFirstColumnIndex + 11].Value = chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  0].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  1].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  2].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  3].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  4].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  5].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  6].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  7].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  8].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  9].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 10].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 11].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 12].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 13].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 14].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 15].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 16].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 17].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 18].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 19].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 20].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 21].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 22].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 23].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 24].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE);
                    reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 25].Value = chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE);
                    var ElementIndex = 0;
                    foreach (var element in chain.Elements) {
                        RowIndex++;
                        reportsheet[RowIndex][0].Value = element.Certificate.Thumbprint;
                        reportsheet[RowIndex][0].Font = new ReportFont {
                            FontName = "Lucida Sans Typewriter",
                            FontSize = 9
                            };
                        reportsheet[RowIndex][1].Value = ChainIndex;
                        reportsheet[RowIndex][2].Value = ElementIndex;
                        reportsheet[RowIndex][4].Value = element.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR;
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  0].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  1].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  2].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  3].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  4].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  5].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  6].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  7].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  8].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex +  9].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex + 10].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain);
                        reportsheet[RowIndex][InfoStatusFirstColumnIndex + 11].Value = element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  0].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  1].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  2].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  3].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  4].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  5].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  6].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  7].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  8].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex +  9].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 10].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 11].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 12].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 13].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 14].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 15].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 16].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 17].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 18].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 19].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 20].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 21].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 22].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 23].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 24].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE);
                        reportsheet[RowIndex][ErrorStatusFirstColumnIndex + 25].Value = element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE);
                        ElementIndex++;
                        }
                    ChainIndex++;
                    }
                }
            reportsource.BuildTarget();
            }

        private void BuildReport1(IDictionary<String,CertificateInfo> data) {
            using (var document = SpreadsheetDocument.Create("zzzz.xlsx", SpreadsheetDocumentType.Workbook)) {
                SheetData SheetData;
                var WorkbookPart = document.AddWorkbookPart();
                WorkbookPart.Workbook = new Workbook();
                var WorksheetPart = WorkbookPart.AddNewPart<WorksheetPart>();
                WorksheetPart.Worksheet = new Worksheet(SheetData = new SheetData());
                var Sheets = WorkbookPart.Workbook.AppendChild(new Sheets());
                var Sheet  = Sheets.AppendChild(new Sheet
                    {
                    Name = "Certificate Chain Info",
                    SheetId = 1,
                    Id = WorkbookPart.GetIdOfPart(WorksheetPart)
                    });
                ExcelReportFile.CreateDefaultWorkbookStylesPart(WorkbookPart);
                var RowIndex = 1;
                var
                row = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex++)});
                row.AppendChildCell("F", 1, "Info");
                row = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex++)});
                row.AppendChildCell("A", 1, "Subject");
                row.AppendChildCell("B", 1, "ChainIndex");
                row.AppendChildCell("C", 1, "ElementIndex");
                row.AppendChildCell("D", 1, "ChainCount");
                row.AppendChildCell("E", 1, "HasChainError");

                var InfoStatusFirstColumnIndex = 6;
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  0), 1, CertificateChainInfoStatus.IsSelfSigned);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  1), 1, CertificateChainInfoStatus.HasExactMatchIssuer);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  2), 1, CertificateChainInfoStatus.HasKeyMatchIssuer);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  3), 1, CertificateChainInfoStatus.HasNameMatchIssuer);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  4), 1, CertificateChainInfoStatus.HasPreferredIssuer);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  5), 1, CertificateChainInfoStatus.HasIssuanceChainPolicy);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  6), 1, CertificateChainInfoStatus.HasValidNameConstraints);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  7), 1, CertificateChainInfoStatus.HasCRLValidityExtended);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  8), 1, CertificateChainInfoStatus.IsPeerTrusted);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  9), 1, CertificateChainInfoStatus.IsFromExclusiveTrustStore);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 10), 1, CertificateChainInfoStatus.IsComplexChain);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 11), 1, CertificateChainInfoStatus.IsCATrusted);
                var ErrorStatusFirstColumnIndex = InfoStatusFirstColumnIndex + 12;
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  0), 1, CertificateChainErrorStatus.TrustIsNotTimeValid);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  1), 1, CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  2), 1, CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  3), 1, CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  4), 1, CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  5), 1, CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  6), 1, CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  7), 1, CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  8), 1, CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  9), 1, CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 10), 1, CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 11), 1, CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 12), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 13), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 14), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 15), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 16), 1, CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 17), 1, CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 18), 1, CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 19), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 20), 1, CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 21), 1, CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 22), 1, CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 23), 1, CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 24), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE);
                row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 25), 1, CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE);
                foreach (var item in data) {
                    RowIndex++;
                    row = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex)});
                    row.AppendChildCell("A", 6, item.Key);
                    row.AppendChildCell("D", 1, item.Value.ChainContext.Chains.Count);
                    row.AppendChildCell("E", 1, item.Value.ChainContext.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR);
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex     ), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  1), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  2), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  3), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  4), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  5), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  6), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  7), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  8), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  9), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 10), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 11), 1, item.Value.ChainContext.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  0), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  1), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  2), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  3), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  4), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  5), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  6), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  7), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  8), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  9), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 10), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 11), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 12), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 13), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 14), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 15), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 16), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 17), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 18), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 19), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 20), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 21), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 22), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 23), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 24), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE));
                    row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 25), 1, item.Value.ChainContext.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE));
                    var ChainIndex = 0;
                    foreach (var chain in item.Value.ChainContext.Chains) {
                        RowIndex++;
                        row = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex)});
                        row.AppendChildCell("B", 1, ChainIndex);
                        row.AppendChildCell("D", 1, chain.Elements.Count);
                        row.AppendChildCell("E", 1, chain.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR);
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex     ), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  1), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  2), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  3), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  4), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  5), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  6), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  7), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  8), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  9), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 10), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 11), 1, chain.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  0), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  1), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  2), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  3), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  4), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  5), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  6), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  7), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  8), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  9), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 10), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 11), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 12), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 13), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 14), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 15), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 16), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 17), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 18), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 19), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 20), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 21), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 22), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 23), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 24), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE));
                        row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 25), 1, chain.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE));
                        var ElementIndex = 0;
                        foreach (var element in chain.Elements) {
                            RowIndex++;
                            row = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex)});
                            row.AppendChildCell("A", 2, element.Certificate.Thumbprint);
                            row.AppendChildCell("B", 1, ChainIndex);
                            row.AppendChildCell("C", 1, ElementIndex);
                            row.AppendChildCell("E", 1, element.ErrorStatus != CertificateChainErrorStatus.CERT_TRUST_NO_ERROR);
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  0), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsSelfSigned));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  1), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasExactMatchIssuer));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  2), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasKeyMatchIssuer));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  3), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasNameMatchIssuer));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  4), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasPreferredIssuer));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  5), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasIssuanceChainPolicy));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  6), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasValidNameConstraints));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  7), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.HasCRLValidityExtended));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  8), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsPeerTrusted));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex +  9), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsFromExclusiveTrustStore));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 10), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsComplexChain));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(InfoStatusFirstColumnIndex + 11), 1, element.InfoStatus.HasFlag(CertificateChainInfoStatus.IsCATrusted));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  0), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.TrustIsNotTimeValid));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  1), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_TIME_NESTED));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  2), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  3), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_SIGNATURE_VALID));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  4), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_NOT_VALID_FOR_USAGE));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  5), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_UNTRUSTED_ROOT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  6), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_REVOCATION_STATUS_UNKNOWN));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  7), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_CYCLIC));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  8), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_EXTENSION));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex +  9), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_POLICY_CONSTRAINTS));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 10), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_BASIC_CONSTRAINTS));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 11), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_INVALID_NAME_CONSTRAINTS));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 12), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 13), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 14), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 15), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 16), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 17), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 18), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 19), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 20), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 21), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_TIME_VALID));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 22), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 23), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 24), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_SIGNATURE));
                            row.AppendChildCell(ExcelReportFile.ExcelColumnName(ErrorStatusFirstColumnIndex + 25), 1, element.ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_HAS_WEAK_HYGIENE));
                            ElementIndex++;
                            }
                        ChainIndex++;
                        }
                    }
                WorksheetPart.Worksheet.AppendChildMergeCells(new MergeCell{
                    Reference = "F1:Q1"
                    });
                document.WorkbookPart.Workbook.Save();
                document.Save();
                }
            }

        public void BuildReport() {
            var data = new Dictionary<String, CertificateInfo>();
            using (var context = new CryptographicContext(null, CRYPT_PROVIDER_TYPE.PROV_RSA_FULL, CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT)) {
                foreach (var file in DirectoryService.GetService<IDirectoryService>(InputFolder).GetFiles("*{e040}.cer", DirectoryServiceSearchOptions.Recursive).Take(10)) {
                    Console.WriteLine(file);
                    var certificate = new X509Certificate(file.ReadAllBytes());
                    var thumbprint = certificate.Thumbprint;
                    if (!data.ContainsKey(thumbprint)) {
                        var info = new CertificateInfo {
                            Certificate = certificate
                            };
                        data[thumbprint] = info;
                        using (var chaincontext = context.GetCertificateChain(
                            certificate, null, null, null,
                            TimeSpan.Zero, DateTime.Now,
                            CERT_CHAIN_FLAGS.CERT_CHAIN_REVOCATION_CHECK_CHAIN, IntPtr.Zero)) {
                            info.ChainContext.ErrorStatus = chaincontext.ErrorStatus;
                            info.ChainContext.InfoStatus = chaincontext.InfoStatus;
                            foreach (var chainT in chaincontext) {
                                ChainInfo chaininfo;
                                info.ChainContext.Chains.Add(chaininfo = new ChainInfo{
                                    ErrorStatus = chainT.ErrorStatus,
                                    InfoStatus = chainT.InfoStatus
                                    });
                                foreach (var chainE in chainT) {
                                    chaininfo.Elements.Add(new ChainElementInfo{
                                        ErrorStatus = chainE.ErrorStatus,
                                        InfoStatus = chainE.InfoStatus,
                                        Certificate = chainE.Certificate,
                                        CertificateRevocationList = chainE.CertificateRevocationList
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            BuildReport(data);
            }
        }
    }