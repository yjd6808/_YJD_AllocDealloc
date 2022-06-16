
namespace JJangdoImageUtil
{

    // Exif 정보에서 이미지 초기 오리엔테이션 정보가 기록되어있다.
    // @Exif 표 참고 : https://en.wikipedia.org/wiki/Exif#Example
    // @Exif - Orientation이 의미하는바 참고 : https://sirv.com/help/articles/rotate-photos-to-be-upright/
    public enum ImageOrientation
    {
        Unknown,
        Default = 1,                // 0도
        DefaultHorizontalMirrored,  // 0도, 좌우반전
        Rotate180,                  // 180도 우측방향으로 회전
        Rotate180HorizontalMirrored,// 180도 우측방향으로 회전, 좌우 반전
        HorizontalMirroredRotate270,// 좌우반전 후 270도 회전
        Rotate270,                  // 270도 우측방향으로 회전
        HorizontalMirroredRotate90, // 좌우반전 후 90도 회전
        Rotate90                    // 90도 우측방향으로 회전
    }
}
