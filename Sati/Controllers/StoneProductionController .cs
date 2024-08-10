using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sati.Models;
using System.Linq;

namespace Sati.Controllers
{
    public class StoneProductionController : Controller
    {
        private readonly GraniteProductionContext _context;

        public StoneProductionController(GraniteProductionContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new StoneProductionViewModel
            {
                ProductionSteps = _context.ProductionSteps.ToList(),
                StoneProductions = new List<StoneProduction>() // Initialize to an empty list
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(StoneProductionViewModel viewModel)
        {
            // Create a new Stone record
            var stone = new Stone
            {
                StoneName = viewModel.StoneName
            };

            _context.Stones.Add(stone);
            _context.SaveChanges(); // Save to get the StoneId

            // Fetch all ProductionSteps from the database
            var productionSteps = _context.ProductionSteps.ToList();

            // Check if the productionSteps list is empty
            if (productionSteps.Count == 0)
            {
                // If no production steps exist, create some default steps
                var defaultSteps = new List<ProductionStep>
{
    new ProductionStep { StepName = "Cutting" },
    new ProductionStep { StepName = "Grinding" },
    new ProductionStep { StepName = "Smoothening" },
    new ProductionStep { StepName = "Cleaning" },
    new ProductionStep { StepName = "Watering" },
    new ProductionStep { StepName = "Polishing" },
    new ProductionStep { StepName = "Epoxy" },
    new ProductionStep { StepName = "Glittering" }
};


                // Add these steps to the database
                _context.ProductionSteps.AddRange(defaultSteps);
                _context.SaveChanges();

                // Re-fetch the production steps from the database
                productionSteps = _context.ProductionSteps.ToList();
            }

            // Create an entry in StoneProduction table for each production step with "Not Started" status
            foreach (var step in productionSteps)
            {
                var stoneProduction = new StoneProduction
                {
                    StoneId = stone.StoneId, // Use the newly created Stone's ID
                    StepId = step.StepId,
                    Status = "NotStarted",
                    StartTime = DateTime.Now, // Initialize StartTime to current time
                    EndTime = null // EndTime is null since the step hasn't started
                };

                _context.StoneProductions.Add(stoneProduction);
            }

            _context.SaveChanges(); // Save all StoneProduction entries

            TempData["SuccessMessage"] = "Stone production data saved successfully.";
            return RedirectToAction("Create");
        }


        [HttpGet]
        public IActionResult Search(string SearchStoneName)
        {
            var stones = _context.Stones
                .Where(s => s.StoneName.Contains(SearchStoneName))
                .ToList();

            var productionSteps = _context.ProductionSteps.ToList();

            var searchResults = stones.Select(stone => new StoneProductionTableViewModel
            {
                StoneId = stone.StoneId,
                StoneName = stone.StoneName,
                ProductionStatuses = productionSteps.Select(step => _context.StoneProductions
                    .Where(sp => sp.StoneId == stone.StoneId && sp.StepId == step.StepId)
                    .Select(sp => sp.Status)
                    .FirstOrDefault() ?? "NotStarted").ToList()
            }).ToList();

            return View("Create", new StoneProductionViewModel
            {
                SearchStoneName = SearchStoneName,
                StoneProductionTableData = searchResults,
                ProductionSteps = productionSteps
            });
        }


        [HttpPost]
        public IActionResult UpdateStepStatus(int stoneProductionId, string status)
        {
            var stoneProduction = _context.StoneProductions
                .Include(sp => sp.Stone)
                .Include(sp => sp.ProductionStep)
                .FirstOrDefault(sp => sp.StoneProductionId == stoneProductionId);

            if (stoneProduction != null)
            {
                stoneProduction.Status = status;

                if (status == "InProgress")
                {
                    stoneProduction.StartTime = DateTime.Now;
                }
                else if (status == "Complete")
                {
                    stoneProduction.EndTime = DateTime.Now;

                    // Move to the next step if available
                    var nextStep = _context.ProductionSteps
                        .OrderBy(ps => ps.StepId)
                        .FirstOrDefault(ps => ps.StepId > stoneProduction.StepId);

                    if (nextStep != null)
                    {
                        var nextStoneProduction = new StoneProduction
                        {
                            StoneId = stoneProduction.StoneId,
                            StepId = nextStep.StepId,
                            Status = "NotStarted"
                        };
                        _context.StoneProductions.Add(nextStoneProduction);
                    }
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Stone production step status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Stone production step not found.";
            }

            return RedirectToAction("Search", new { stoneName = stoneProduction?.Stone?.StoneName });
        }

        public IActionResult GetStoneProductionSteps(int stoneId)
        {
            var stoneProductions = _context.StoneProductions
                .Include(sp => sp.ProductionStep)
                .Where(sp => sp.StoneId == stoneId)
                .ToList();

            return PartialView("_StoneProductionStepsPartial", stoneProductions);
        }


        [HttpPost]
        public IActionResult EditStoneProduction(int StoneId, string StoneName, Dictionary<int, string> Statuses)
        {
            foreach (var status in Statuses)
            {
                var stoneProduction = _context.StoneProductions
                    .FirstOrDefault(sp => sp.StoneProductionId == status.Key);

                if (stoneProduction != null)
                {
                    stoneProduction.Status = status.Value;
                }
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Production steps updated successfully.";
            return RedirectToAction("Search", new { SearchStoneName = StoneName });
        }




    }
}
