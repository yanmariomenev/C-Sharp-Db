
namespace P01_StudentSystem.Data.Models
{
  public  class Resource
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public ResourcesType ResourceType { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }

  public enum ResourcesType
  {
      Video = 1,
      Presentation = 2,
      Document = 3,
      Other = 4
  }
}
