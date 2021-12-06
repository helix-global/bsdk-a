using System;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace kit
    {
    public static class JsonOperations
        {
        #region M:LoadFile(String):Object
        private static Object LoadFile(String filename) {
            var o = Asn1Object.Load(filename).FirstOrDefault();
            if (o != null) {
                return (Object)LoadCertificate(o) ??
                       (Object)LoadCRL(o);
                }
            return null;
            }
        #endregion
        #region M:LoadCertificate(Asn1Object):Asn1Certificate
        private static Asn1Certificate LoadCertificate(Asn1Object o) {
            if (o != null) {
                var r = new Asn1Certificate(o);
                if (!r.IsFailed) {
                    return r;
                    }
                }
            return null;
            }
        #endregion
        #region M:LoadCRL(Asn1Object):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList LoadCRL(Asn1Object o) {
            if (o != null) {
                var r = new Asn1CertificateRevocationList(o);
                if (!r.IsFailed) {
                    return r;
                    }
                }
            return null;
            }
        #endregion

        #region M:Serialize(String,String)
        private static void Serialize(String sourcefilename, String targetfolder) {
            var folder = String.IsNullOrWhiteSpace(targetfolder)
                ? Path.GetDirectoryName(sourcefilename)
                : targetfolder;
            var targetfilename = Path.Combine(folder, Path.GetFileName(sourcefilename) + ".json");
            var o = LoadFile(sourcefilename);
            if (o != null) {
                Serialize(o, new StreamWriter(File.Create(targetfilename)));
                }
            }
        #endregion
        #region M:Serialize(Object,TextWriter)
        private static void Serialize(Object value, TextWriter output) {
            using (var writer = new JsonTextWriter(output){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    })
                {
                var serializer = new JsonSerializer();
                if (value is IJsonSerializable o)
                    {
                    o.WriteJson(writer, serializer);
                    }
                else
                    {
                    serializer.Serialize(writer, value);
                    }
                writer.Flush();
                }
            }
        #endregion

        public static void GenerateJson(String sourcefolder, String targetfolder, String filter) {
            if (File.Exists(sourcefolder)) {
                var ext = Path.GetExtension(sourcefolder);
                if (!String.Equals(ext, ".json", StringComparison.OrdinalIgnoreCase)) {
                    Serialize(sourcefolder, targetfolder);
                    return;
                    }
                }
            foreach (var filename in Directory.EnumerateFiles(sourcefolder, filter, SearchOption.AllDirectories).Where(i => Path.GetExtension(i).ToUpper() != ".JSON")) {
                Serialize(filename, targetfolder);
                }
            }
        }
    }