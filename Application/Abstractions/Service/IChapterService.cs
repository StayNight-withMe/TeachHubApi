using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Chapter.output;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IChapterService
    {
        Task<TResult> Create(
            CreateChapterDTO chapter, 
            int userid,
            CancellationToken ct = default);
        Task<TResult<ChapterOutDTO>> UpdateChapter(
            ChapterUpdateDTO newchapter, 
            int userid,
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<ChapterOutDTO>>> GetChaptersByCourseId(
            int courseid,  
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<ChapterOutDTO>>> GetChaptersByCourseIdAndUserId(
            int courseid, 
            int userid, 
            SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default);
        Task<TResult> DeleteChapter(
            int chapterid, 
            int userid = default, 
            CancellationToken ct = default);
    }
}
