using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly BlobServiceClient client;
        private readonly BlobContainerClient container;

        public TagsController(HousesDBContext context, BlobServiceClient client) 
        {
            this.context = context;
            this.client = client;
            container = this.client.GetBlobContainerClient("tagimages");
            container.CreateIfNotExists();
            container.SetAccessPolicy(PublicAccessType.Blob);
        }
        [HttpGet]
        public async Task<IActionResult> GetTags() 
        {
            var tags = context.Tags.Select(t=>new TagDTO() { Id=t.Id,ImagePath=t.ImagePath,Name=t.Name}).ToList();
            return Ok(tags);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(int id)
        {
            var tag = await context.Tags.Select(t => new TagDTO() { Id = t.Id, ImagePath = t.ImagePath, Name = t.Name }).FirstOrDefaultAsync(t=>t.Id==id);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTag(TagCreateDTO tag)
        {
            if(ModelState.IsValid)
            {
                var uri = await BlobContainerFunctions.UploadImage(container, tag.Image);
                Tag createdTag = new Tag()
                {
                    ImagePath = uri,
                    Name = tag.Name
                };
                await context.Tags.AddAsync(createdTag);
                await context.SaveChangesAsync();
                return Ok(createdTag);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            if (ModelState.IsValid)
            {
                var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
                if(tag == null) return NotFound();
                BlobContainerFunctions.DeleteImage(container, tag.ImagePath);
                context.Remove(tag);
                await context.SaveChangesAsync();
                return Ok($"Tag with id {id} was deleted");
            }
            return BadRequest(ModelState);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> EditTag(TagEditDTO edittedTag)
        {
            if (ModelState.IsValid)
            {
                var tag = await context.Tags.FirstOrDefaultAsync(t=>t.Id == edittedTag.Id);
                if(tag == null) return NotFound();
                if(edittedTag.Image!=null)
                {
                    BlobContainerFunctions.DeleteImage(container, tag.ImagePath);
                    string uri = await BlobContainerFunctions.UploadImage(container, edittedTag.Image);
                    tag.ImagePath = uri;
                }
                tag.Name= edittedTag.Name;
                context.Update(tag);
                await context.SaveChangesAsync();
                return Ok($"Tag with id {edittedTag.Id} was editted");
            }
            return BadRequest(ModelState);
        }
    }

}
