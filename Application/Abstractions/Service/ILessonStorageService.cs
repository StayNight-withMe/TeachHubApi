using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.LessonFile.input;
using Core.Models.TargetDTO.LessonFile.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Abstractions.Service
{
    public interface ILessonStorageService
    {
        Task<TResult<PagedResponseDTO<LessonFileOutputDTO>>> GetLessonUrlFile(
              int lessonid,
            PaginationDTO pagination,
            CancellationToken ct = default
            );
        Task<TResult> UploadFile(
          Stream stream,
            int userid,
            MetaDataLessonDTO metaData,
            string contentType,
            CancellationToken ct = default
            );
        Task<TResult> DeleteLessonUrlFile(
            int fileid,
            int userid,
            CancellationToken ct = default
            );
    }
}
