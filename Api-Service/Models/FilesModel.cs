
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ApiService.Models;
public class FilesModel : BaseModel
{
    public List<IFormFile> Attachment { get; set; }
    public List<String> AttachmentBase64 { get; set; }

    public List<IFormFile> DamagedFilesAttachment { get; set; }

    public List<String> DamagedFilesAttachmentBase64 { get; set; }


}
