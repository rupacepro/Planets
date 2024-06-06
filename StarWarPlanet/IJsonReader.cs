interface IJsonReader
{
    Task<string> Read(string baseAddress, string requestUri);
}
