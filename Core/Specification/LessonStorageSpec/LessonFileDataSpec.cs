using Ardalis.Specification;
using Core.Models.Entitiеs;


namespace Core.Specification.LessonStorageSpec
{
    public record LessonFileData(int LessonId, string FileKey);

    public class LessonFileDataSpec : Specification<LessonfileEntity, LessonFileData>
    {
        public LessonFileDataSpec(int fileId)
        {
            Query.AsNoTracking()
                 .Where(f => f.id == fileId)
                 .Select(f => new LessonFileData(f.lessonid, f.filekey));
        }
    }
}