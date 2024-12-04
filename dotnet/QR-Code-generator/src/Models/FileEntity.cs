namespace QRCodeGeneratorApp.Models
{
    public class FileEntity
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public DateTime SavedDate { get; set; }
        public DateTime DeletionDate { get; set; }
        public required string Link { get; set; }
    }
}
