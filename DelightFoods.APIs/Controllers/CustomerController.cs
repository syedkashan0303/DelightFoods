using DelightFoods.APIs.Data;
using DelightFoods.APIs.Model;
using DelightFoods.APIs.Model.DTO;
using DelightFoods.APIs.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DelightFoods.APIs.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            if (customers == null || !customers.Any())
            {
                return NoContent();
            }

            var customerAddresses = _context.CustomerAddress
                .Where(x => customers.Select(c => c.AddressId).Contains(x.Id))
                .ToList();
            var cityClasses = Enum.GetValues(typeof(CityClass)).Cast<CityClass>().ToList();
            var utilities = new MapperClass<CustomerModel, CustomerDTO>();
            var customerDTOs = customers.Select(customer =>
            {
                var customerDTO = utilities.Map(customer);
                var address = customerAddresses.FirstOrDefault(addr => addr.Id == customer.AddressId);

                customerDTO.CityId = address?.CityId ?? 0;
                customerDTO.CityName = customerDTO.CityId > 0
                    ? cityClasses.FirstOrDefault(c => (int)c == customerDTO.CityId).ToString()
                    : string.Empty;
                customerDTO.Mobile = customer.Mobile;
                customerDTO.Email = customer.Email;

                return customerDTO;
            }).ToList();

            return Ok(customerDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<CustomerModel, CustomerDTO>();
            var customerDTO = utilities.Map(customer);

            return Ok(customerDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> CreateCustomer([FromBody] CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customerModel);
            await _context.SaveChangesAsync();

            var utilities = new MapperClass<CustomerModel, CustomerDTO>();
            var customerDTO = utilities.Map(customerModel);

            return CreatedAtAction(nameof(GetCustomerById), new { id = customerModel.Id }, customerDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerModel customerModel)
        {
            if (id != customerModel.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Entry(customerModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
