using System.Collections.Generic;

namespace TelcoAPIService.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }

        public int Files { get; set; }

        public int DamageFiles { get; set; }


        public List<FilesModel> Data { get; set; }

        public string Message {  get; set; }
    }
}