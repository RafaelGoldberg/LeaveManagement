using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Contracts;
using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveTypeRepository _leaverepo;
        private readonly ILeaveAllocationRepository _leaveallocationrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController
            (
            ILeaveTypeRepository leaverepo, 
            ILeaveAllocationRepository leaveallocationrepo, 
            IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _leaverepo = leaverepo;
            _leaveallocationrepo = leaveallocationrepo;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: LeaveAllocation
        public ActionResult Index()
        {
            var leavetypes = _leaverepo.FindAll().ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leavetypes);
            var model = new CreateLeaveAllocationViewModel
            {
                LeaveType = mappedLeaveTypes,
                NumberUpdated = 0
            };
        
            return View(model);
        }

        public ActionResult SetLeave(int id)
        {
            var leavetype = _leaverepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if (_leaveallocationrepo.CheckAllocation(id, emp.Id))
                    continue;
                    var allocation = new LeaveAllocationViewModel
                    {
                        DateCreated = DateTime.Now,
                        EmployeeId = emp.Id,
                        LeaveTypeId = id,
                        NumberOfDays = leavetype.DefaultDays,
                        Period = DateTime.Now.Year
                    };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                _leaveallocationrepo.Create(leaveallocation); 
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(model);

        }

        // GET: LeaveAllocation/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeViewModel>(_userManager.FindByIdAsync(id).Result);
            var allocations = _mapper.Map<List<LeaveAllocationViewModel>>(_leaveallocationrepo.GetLeaveAllocationsByEmployee(id));
            var model = new ViewAllocationViewModel
            {
                Employee = employee,
                LeaveAllocations = allocations
            };

            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var leaveallocation = _leaveallocationrepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveallocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var record = _leaveallocationrepo.FindById(model.Id);

                // var allocation = _mapper.Map<LeaveAllocation>(model);
                record.NumberOfDays = model.NumberOfDays;
                var success = _leaveallocationrepo.Update(record);
                if (!success)
                {
                    ModelState.AddModelError("", "Error while Saving");
                    return View(model);
                }


                return RedirectToAction(nameof(Details), new {id = model.EmployeeId });
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}