using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class InvalidImageFormatException : Exception
    {
        public InvalidImageFormatException() : base("잘못된 이미지 형식입니다.") 
        {
        }
    }
}
