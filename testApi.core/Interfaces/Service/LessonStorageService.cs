using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.LessonFile.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface LessonStorageService
    {
        Task<PagedResponseDTO<LessonFileOutputDTO>> GetLessonsFile(int lessonid);
    }
}
