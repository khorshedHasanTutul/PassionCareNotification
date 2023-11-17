namespace PassionCareNotification
{
    public class ResponseMessage
    {
        public int? ObjectId { get; set; }
        public object ResponseObj { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public int Rows { get; set; }
    }
}
