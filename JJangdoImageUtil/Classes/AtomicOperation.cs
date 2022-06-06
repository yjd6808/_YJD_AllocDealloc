// 작성자 : 윤정도
// 아토믹 연산 기능중 부족한 기능

using System.Threading;

namespace JJangdoImageUtil
{
    public class AtomicOperation
    {
        // Interlocked Read는 64비트 정수형만 지원하네; 어쩔수없이 이렇게함
        public static int Read(ref int value)
        {
            return Interlocked.CompareExchange(ref value, 0, 0);
        }
    }
}
