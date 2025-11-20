using AutoMapper;
using Core.Interfaces.Repository;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using infrastructure.Entitiеs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Service.LessonService
{
    public class LessonService
    {
        private readonly IBaseRepository<LessonEntities> _lessonRepository;

        private readonly IBaseRepository<ChapterEntity> _chapterRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public LessonService(ILogger logger, IUnitOfWork unitOfWork, IBaseRepository<ChapterEntity> chapterRepository, IBaseRepository<LessonEntities> lessonRepository) 
        { 
           _logger = logger;
            _unitOfWork = unitOfWork;
            _chapterRepository = chapterRepository;
            _lessonRepository = lessonRepository;
        }


       

    }
}
