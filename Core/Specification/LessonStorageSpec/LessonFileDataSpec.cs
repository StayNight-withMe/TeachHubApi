using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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