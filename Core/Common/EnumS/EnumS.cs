using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.EnumS
{

    public enum AllRole
    {
        user = 1,
        admin = 2,
    }

    public enum PublicRole
    {
        user = 1,
    }

    public enum reaction_type
    {
        /// <summary>
        /// Like
        /// </summary>
        Like,
        /// <summary>
        /// Dislike
        /// </summary>
        Dislike,
        /// <summary>
        ///None (что бы убрать реакцию)
        /// </summary>
        None 
    }

    public enum SetImageStatus 
    {
    Remove,
    Upload,
    }

}
