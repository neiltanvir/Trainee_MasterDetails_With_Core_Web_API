namespace TraineeCoreAPI.DTOs
{
    public class Common
    {
        public int TraineeId { get; set; }

        public string? TraineeName { get; set; }

        public bool IsRegular { get; set; }

        public DateTime BirhDate { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ImageName { get; set; }
        public string? Courses { get; set; }
    }
}
