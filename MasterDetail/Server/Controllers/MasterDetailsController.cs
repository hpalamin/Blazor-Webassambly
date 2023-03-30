using MasterDetail.Server.Models;
using MasterDetail.Server.Pages;
using MasterDetail.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace MasterDetail.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
  
    public class MasterDetailsController : ControllerBase
    {
        private readonly TourDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MasterDetailsController(TourDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this._env = env;
        }

        [HttpGet]
        [Route("GetSpots")]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            return await _context.Spots.ToListAsync();
        }

        [HttpGet]
        [Route("GetCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetClients()
        {
            return await _context.Customers.Include(c => c.BookingEntries).ThenInclude(b => b.Spot).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Customer> GetCustomerById(int id)
        {
            return await _context.Customers.Where(x => x.CustomerId == id).Include(c => c.BookingEntries).ThenInclude(b => b.Spot).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CustomerVM customerVM)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer()
                {
                    CustomerName = customerVM.CustomerName,
                    BirthDate = customerVM.BirthDate,
                    Phone = customerVM.Phone,
                    MaritialStatus = customerVM.MaritialStatus
                };

                //Image
                if (customerVM.PictureFile is not null)
                {
                    string webroot = _env.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Guid.NewGuid().ToString() + Path.GetExtension(customerVM.PictureFile.FileName);
                    string fileToWrite = Path.Combine(webroot, folder, imgFileName);

                    using (var stream = new FileStream(fileToWrite, FileMode.Create))
                    {
                        await customerVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = imgFileName;
                    }
                }
                else
                {
                    customer.Picture = "default.jpg";
                }
                _context.Customers.Add(customer);

                if (customerVM.SpotList.Count() > 0)
                {
                    foreach (Spot spot in customerVM.SpotList)
                    {
                        _context.BookingEntries.Add(new BookingEntry
                        {
                            Customer = customer,
                            CustomerId = customer.CustomerId,
                            SpotId = spot.SpotId
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(customer);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] CustomerVM customerVM)
        {

            if (ModelState.IsValid)
            {
                Customer customer = _context.Customers.Find(customerVM.CustomerId);
                customer.CustomerName = customerVM.CustomerName;
                customer.BirthDate = customerVM.BirthDate;
                customer.Phone = customerVM.Phone;
                customer.MaritialStatus = customerVM.MaritialStatus;

                //Image
                if (customerVM.PictureFile is not null)
                {
                    string webroot = _env.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Guid.NewGuid().ToString() + Path.GetExtension(customerVM.PictureFile.FileName);
                    string fileToWrite = Path.Combine(webroot, folder, imgFileName);

                    using (var stream = new FileStream(fileToWrite, FileMode.Create))
                    {
                        await customerVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = imgFileName;
                    }
                }

                var existsBookings = _context.BookingEntries.Where(x => x.CustomerId == customerVM.CustomerId).ToList();
                if (existsBookings is not null)
                {
                    foreach (var entry in existsBookings)
                    {
                        _context.Remove(entry);
                    }
                }
                

                if (customerVM.SpotList.Count() > 0)
                {
                    foreach (Spot spot in customerVM.SpotList)
                    {
                        _context.BookingEntries.Add(new BookingEntry
                        {
                            CustomerId = customer.CustomerId,
                            SpotId = spot.SpotId
                        });
                    }
                }

                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(customer);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            Customer customer = _context.Customers.Find(id);

            var existsBookings = _context.BookingEntries.Where(x => x.CustomerId == customer.CustomerId).ToList();
            if (existsBookings is not null)
            {
                foreach (var entry in existsBookings)
                {
                    _context.Remove(entry);
                }
            }
            _context.Remove(customer);
            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(customer);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
