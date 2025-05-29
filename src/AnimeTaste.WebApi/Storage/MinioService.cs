using AnimeTaste.Core.Utils;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace AnimeTaste.WebApi.Storage
{
    public class MinioService(IMinioClient minioClient)
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

        private async Task<bool> CheckBucket(string bucket, bool autoCreate = true)
        {
            var beArgs = new BucketExistsArgs().WithBucket(bucket);
            var found = await minioClient.BucketExistsAsync(beArgs);

            if (!found && autoCreate)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(bucket);
                await minioClient.MakeBucketAsync(mbArgs);
            }
            return found;
        }

        //public async Task CheckBucket1(string bucket)
        //{
        //    var mbArgs = new MakeBucketArgs().WithBucket(bucket);
        //    await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        //}

        public async Task<bool> UploadFile(string bucket, params List<string> fileFullPaths)
        {
            try
            {
                await CheckBucket(bucket);

                foreach (var file in fileFullPaths)
                {
                    if (!ContentTypeProvider.TryGetContentType(file, out string? contentType))
                    {
                        contentType = "application/octet-stream"; // 默认类型
                    }

                    var name = Path.GetFileName(file);

                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(name)
                        .WithFileName(file)
                        .WithContentType(contentType);

                    var d = await minioClient.PutObjectAsync(putObjectArgs);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return false;
            }
        }

        public async Task<bool> DownloadFile(string folderPath, string bucketName, string objectName)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var statObjectArgs = new StatObjectArgs()
                                                    .WithBucket(bucketName)
                                                    .WithObject(objectName);
                await minioClient.StatObjectAsync(statObjectArgs);

                var getObjectArgs = new GetObjectArgs()
                                                  .WithBucket(bucketName)
                                                  .WithObject(objectName)
                                                  .WithFile(folderPath + objectName);
                await minioClient.GetObjectAsync(getObjectArgs);
                return true;
            }
            catch (MinioException ex)
            {
                Logger.Exception(ex);
                return false;
            }
        }

        public async Task<bool> DeleteFile(string bucketName, string objectName)
        {
            try
            {
                var exists = await CheckBucket(bucketName, false);

                if (!exists)
                {
                    return false;
                }

                var removeObjectArgs = new RemoveObjectArgs()
                                .WithBucket(bucketName)
                                .WithObject(objectName);
                await minioClient.RemoveObjectAsync(removeObjectArgs);

                return true;
            }
            catch (MinioException e)
            {
                Logger.Exception(e);
                return false;
            }
        }


        public async Task<string?> GetFileUrl(string bucketName, string objectName, int expiry = 604800)
        {
            try
            {
                var args = new PresignedGetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithExpiry(expiry);
                return await minioClient.PresignedGetObjectAsync(args);
            }
            catch (MinioException e)
            {
                Logger.Exception(e);
                return default;
            }
        }
    }
}
