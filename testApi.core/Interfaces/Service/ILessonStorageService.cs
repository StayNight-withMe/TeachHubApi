using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.LessonFile.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.ReturnEntity;
namespace Core.Interfaces.Service
{
    public interface ILessonStorageService
    {
        Task<TResult<PagedResponseDTO<LessonFileOutputDTO>>> GetLessonUrlFile(int lessonid);
        Task<TResult> UploadFile(Stream stream, int userid, int lessonid, string contentType);
        Task DeleteLessonUrlFile(string fileid,int lessonid, int userid);
    }
}
