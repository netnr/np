using Netnr.TencentAI.Model;
using System.ComponentModel;

namespace Netnr.TencentAI.fcgi_bin
{
    /// <summary>
    /// 人脸与人体识别
    /// </summary>
    [Description("人脸与人体识别")]
    public class Face
    {
        /// <summary>
        /// 人脸与人体识别>人脸检测与分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("人脸检测与分析")]
        public static string Face_DetectFace(Face_DetectFaceRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_detectface";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>多人脸检测
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Description("多人脸检测")]
        public static string Face_DetectMultiFace(Face_DetectMultiFaceRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_detectmultiface";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>跨年龄人脸识别
        /// </summary>
        /// <returns></returns>
        [Description("跨年龄人脸识别")]
        public static string Face_DetectCrossAgeFace(Face_DetectCrossAgeFaceRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_detectcrossageface";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>五官定位
        /// </summary>
        /// <returns></returns>
        [Description("五官定位")]
        public static string Face_FaceShape(Face_FaceShapeRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_faceshape";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸对比
        /// </summary>
        /// <returns></returns>
        [Description("人脸对比")]
        public static string Face_FaceCompare(Face_FaceCompareRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_facecompare";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>人脸搜索
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>人脸搜索")]
        public static string Face_FaceIdentify(Face_FaceIdentifyRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_facecompare";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>个体创建
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>个体创建")]
        public static string Face_NewPerson(Face_NewPersonRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_newperson";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>删除个体
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>删除个体")]
        public static string Face_DelPerson(Face_DelPersonRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_delperson";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>增加人脸
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>增加人脸")]
        public static string Face_AddFace(Face_AddFaceRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_addface";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>删除人脸
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>删除人脸")]
        public static string Face_DelFace(Face_DelFaceRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_delface";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>设置信息
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>设置信息")]
        public static string Face_SetInfo(Face_SetInfoRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getinfo";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>获取信息
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>获取信息")]
        public static string Face_GetInfo(Face_GetInfoRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getinfo";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>获取组列表
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>获取组列表")]
        public static string Face_GetGroupIds(Face_GetGroupIdsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getgroupids";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>获取个体列表
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>获取个体列表")]
        public static string Face_GetPersonIds(Face_GetPersonIdsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getpersonids";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>获取人脸列表
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>获取人脸列表")]
        public static string Face_GetFaceIds(Face_GetFaceIdsRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getfaceids";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸搜索>获取人脸信息
        /// </summary>
        /// <returns></returns>
        [Description("人脸搜索>获取人脸信息")]
        public static string Face_GetFaceInfo(Face_GetFaceInfoRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_getfaceinfo";
            return Aid.Request(request, uri);
        }

        /// <summary>
        /// 人脸与人体识别>人脸验证
        /// </summary>
        /// <returns></returns>
        [Description("人脸验证")]
        public static string Face_FaceVerify(Face_FaceVerifyRequest request)
        {
            var uri = "https://api.ai.qq.com/fcgi-bin/face/face_faceverify";
            return Aid.Request(request, uri);
        }
    }
}