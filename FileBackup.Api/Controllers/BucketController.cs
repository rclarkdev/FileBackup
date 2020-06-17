using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileBackup.Core.Communication.Bucket;
using FileBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FileBackup.Api.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [Route("create/{bucketName}")]
        [HttpPost]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket([FromRoute] string bucketName)
        {
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);

            if (bucketExists)
            {
                return BadRequest("Bucket Already Exists");
            }

            var result = await _bucketRepository.CreateBucket(bucketName);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Route("list")]
        [HttpGet]
        public async Task<ActionResult<GetS3BucketResponse>> ListS3Buckets ()
        {
            var result = await _bucketRepository.ListBuckets();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteS3Bucket (string bucketName)
        {
            await _bucketRepository.DeleteBucket(bucketName);

            return Ok();
        }
    }
}