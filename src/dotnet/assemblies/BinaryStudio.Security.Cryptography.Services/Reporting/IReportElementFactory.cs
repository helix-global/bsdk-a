namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public interface IReportElementFactory<T>
        {
        T CreateElement();
        }
    }