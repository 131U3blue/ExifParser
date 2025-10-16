namespace ExifParser.APIs
{
    public interface IApiResponse
    {
        public string ResponseMessage { get; }
        public int ResponseCode { get; }
        public string RawResponse { get; }        
    }
}
