using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.LessonFile.input;
using Core.Model.TargetDTO.LessonFile.output;
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
