using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Sati.Models
{
    public class Stone
    {
        public int StoneId { get; set; }
        public string StoneName { get; set; }
    }

    public class ProductionStep
    {
        [Key]
        public int StepId { get; set; }
        public string StepName { get; set; }
    }


    public class StoneProduction
    {
        public int StoneProductionId { get; set; }
        public int StoneId { get; set; }
        public int StepId { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Stone Stone { get; set; }
        public ProductionStep ProductionStep { get; set; }
    }

    public class StoneProductionTableViewModel
    {
        public int StoneId { get; set; }
        public string StoneName { get; set; }
        public List<string> ProductionStatuses { get; set; } // Each element corresponds to a step's status
    }

    public class StoneProductionViewModel
    {
        public string StoneName { get; set; }
        public string SearchStoneName { get; set; }
        public List<ProductionStep> ProductionSteps { get; set; }
        public List<StoneProduction> StoneProductions { get; set; } = new List<StoneProduction>();

        // Add this property to hold the table data
        public List<StoneProductionTableViewModel> StoneProductionTableData { get; set; } = new List<StoneProductionTableViewModel>();
    }


    public class StepStatusViewModel
    {
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class StoneProductionStepsViewModel
    {
        public int StoneId { get; set; }
        public List<StoneProduction> StoneProductions { get; set; }
    }



}
