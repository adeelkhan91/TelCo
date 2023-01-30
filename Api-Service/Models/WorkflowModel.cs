using ApiService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService
{
    public class WorkflowModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // [ForeignKey("ReferenceNumber")]
        public ICollection<LaptopModel> laptop {  get; set; }
        public ICollection<VehicleModel> vechile {  get; set; }
        public ICollection<HandsetModel> handset {  get; set; }
        public ICollection<CashModel> cash {  get; set; }
        public ICollection<MarineModel> marine {  get; set; }
        public ICollection<BsdModel> bsd {  get; set; }
        public ICollection<MarineInlandModel> marineInland {  get; set; }
        public ICollection<CellSiteModel> cellsite {  get; set; }
        public ICollection<PocModel> pocCommon {  get; set; }
       
        public DateTime Date { get; set; }
        public int UserId { get; set; } //coming from telco api
    }
    /*public class Standard
    {
        public int StandardId { get; set; }
        public string StandardName { get; set; }
        public ICollection<Student> Students { get; set; }
    }*/
}
