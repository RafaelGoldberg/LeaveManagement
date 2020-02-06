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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.Controllers
{

    [Authorize]
    public class LeaveRequestController : Controller
    {

        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(
            ILeaveRequestRepository leaveRequestRepo,
            ILeaveTypeRepository leaveTypeRepo,
             ILeaveAllocationRepository leaveAllocationRepo,
            IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: LeaveRequest
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveRequests = _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestViewModel>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestsModel.Count,
                ApprovedRequests = leaveRequestsModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequestsModel.Count(q => q.Approved == null),
                RejectedRequested = leaveRequestsModel.Count(q => q.Approved == false),
                LiveRequests = leaveRequestsModel

            };
            return View(model);
        }

        // GET: LeaveRequest/Details/5
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestViewModel>(leaveRequest);
            return View(model);
        }

        public ActionResult ApproveRequest(int id)
      {
            try
            {
                var leaveRequest = _leaveRequestRepo.FindById(id);
                var user = _userManager.GetUserAsync(User).Result;
                var employeeId = leaveRequest.RequestingEmployeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employeeId, leaveTypeId);

                var daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberOfDays -= daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                 _leaveRequestRepo.Update(leaveRequest);
                _leaveAllocationRepo.Update(allocation);
       
        
            }
            catch (Exception)
            {

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));

        }


       public ActionResult RejectRequest(int id)
        {
            try
            {
                var leaveRequest = _leaveRequestRepo.FindById(id);
                var user = _userManager.GetUserAsync(User).Result;
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                var isSuccsess = _leaveRequestRepo.Update(leaveRequest);

           
         

            }
            catch (Exception)
            {

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveRequest/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            });
            var model = new CreateLeaveRequestViewModel
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var leaveTypes = _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (DateTime.Compare(model.StartDate, model.EndDate) > 1)
                {
                    ModelState.AddModelError("", "Invalid Date selection...");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                var daysRequested = (int)(model.EndDate.Date - model.StartDate.Date).TotalDays;

                if(daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You Do not have sufficien days for this request.");
                    return View(model);
                }

                var leaveRequest = new LeaveRequestViewModel
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId
                };

                var leaveRequst = _mapper.Map<LeaveRequest>(leaveRequest);
                var isSuccess = _leaveRequestRepo.Create(leaveRequst);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View();
                }


                return RedirectToAction("UserLeaveRequests");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View();
            }
        }

        public ActionResult UserLeaveRequests()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var employeeId = user.Id;

            var userAllocations = _leaveAllocationRepo.GetLeaveAllocationsByEmployee(employeeId);
            var userRequests = _leaveRequestRepo.GetLeaveRequestsByEmployee(employeeId);

            var userAllocationModel = _mapper.Map<List<LeaveAllocationViewModel>>(userAllocations);
            var userRequestModel = _mapper.Map<List<LeaveRequestViewModel>>(userRequests);

            var model = new EmployeeLeaveRequestsViewModel
            {
                LeaveAllocations = userAllocationModel,
                LeaveRequests = userRequestModel
            };

            return View(model);
        }


        // GET: LeaveRequest/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequest/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        public ActionResult CancelRequest(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var leaveRequest = _leaveRequestRepo.FindById(id);

                if (leaveRequest == null)
                {
                    return NotFound();
                }

                var isSuccess = _leaveRequestRepo.Delete(leaveRequest);
                if (!isSuccess)
                {
                    return BadRequest();
                }

                return RedirectToAction("UserLeaveRequests");
            }
            catch
            {
                return View();
            }
        }

        // POST: LeaveRequest/Delete/5
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