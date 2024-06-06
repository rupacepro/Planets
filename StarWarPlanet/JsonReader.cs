class JsonReader : IJsonReader
{
    public async Task<string> Read(string baseAddress, string requestUri)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(baseAddress);
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
