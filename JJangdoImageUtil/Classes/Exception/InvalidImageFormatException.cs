// 작성자 : 윤정도
// 걍 만들어본 커스텀 예외처리
// 이렇게까지 만들기는 귀찮아서 이제 추가안할꺼

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
