using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _roleRepository;
        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }
        
        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x => 
                new EmployeeShortResponse()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        FullName = x.FullName,
                    }).ToList();

            return employeesModelList;
        }
        
        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            
            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="appliedPromocodesCount"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateAsync(string firstName, string lastName,string email,
            Guid roleId, int appliedPromocodesCount)
        {
            var Roles = await _roleRepository.GetAllAsync();

            var employee = new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Roles = Roles.Where(x => x.Id == roleId).ToList(),                
                AppliedPromocodesCount = appliedPromocodesCount
            };            

            var newEmployee = await _employeeRepository.CreateAsync(employee);
            
            if (newEmployee == null)
            {
                return BadRequest();
            }

            var employeesModelList =
                new EmployeeResponse()
                {
                    Id = newEmployee.Id,
                    Email = newEmployee.Email,
                    FullName = newEmployee.FullName,
                    Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
                };

            return Ok(employeesModelList);
        }

        /// <summary>
        /// Обновить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, string firstName, string lastName, string email,
            Guid roleId, int appliedPromocodesCount)
        {
            var Roles = await _roleRepository.GetAllAsync();
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Email = email;
            employee.Roles = Roles.Where(x => x.Id == roleId).ToList();
            employee.AppliedPromocodesCount = appliedPromocodesCount;
            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);

            if (updatedEmployee == null)
            {
                return BadRequest();
            }

            var employeesModelList =
                new EmployeeResponse()
                {
                    Id = updatedEmployee.Id,
                    Email = updatedEmployee.Email,
                    FullName = updatedEmployee.FullName,
                    Roles = updatedEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    AppliedPromocodesCount = updatedEmployee.AppliedPromocodesCount
                };

            return Ok(employeesModelList);
        }

        /// <summary>
        /// Удалить сотрудник по Id
        /// </summary>        
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            await _employeeRepository.DeleteAsync(employee);
            return Ok();
        }
    }
}