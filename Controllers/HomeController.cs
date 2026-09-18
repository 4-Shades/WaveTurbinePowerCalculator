using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WavePowerApp.Data;
using WavePowerApp.Models;

namespace WavePowerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.History = await _context.WaveCalculations
                    .OrderByDescending(x => x.CalculatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database warning: Could not load calculation history. " + ex.Message;
            }

            return View(new WaveCalculation { WaveHeight = 1.6, WavePeriod = 5.0, ArrayLength = 25.0, Efficiency = 30.0 });
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(WaveCalculation model)
        {
            try
            {
                // Physical Constraint Validation inside Try Block
                if (model.WaveHeight < 0 || model.WavePeriod < 0 || model.ArrayLength < 0 || model.Efficiency < 0)
                {
                    throw new ArgumentException("Parameter values cannot be negative.");
                }

                if (model.Efficiency > 100)
                {
                    throw new ArgumentException("Efficiency percentage cannot exceed 100%.");
                }

                // Wave Power Density Formula: P = 0.5 * H_s^2 * T_e (kW/m)
                double powerPerMeter = 0.5 * Math.Pow(model.WaveHeight, 2) * model.WavePeriod;
                double rawPowerKw = powerPerMeter * model.ArrayLength;
                double netPowerKw = rawPowerKw * (model.Efficiency / 100.0);

                model.RawPowerKw = rawPowerKw;
                model.NetPowerKw = netPowerKw;
                model.CalculatedAt = DateTime.UtcNow;

                // Save to PostgreSQL DB
                _context.WaveCalculations.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.Result = model;
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = "Validation Error: " + ex.Message;
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Error = "Database Write Error: Failed to log result. " + ex.InnerException?.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An unexpected error occurred: " + ex.Message;
            }

            // Reload recent history
            try
            {
                ViewBag.History = await _context.WaveCalculations
                    .OrderByDescending(x => x.CalculatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            catch
            {
                // Fail silently for history fetch during main error
            }

            return View("Index", model);
        }
    }
}