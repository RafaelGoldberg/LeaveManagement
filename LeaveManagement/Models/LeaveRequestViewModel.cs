using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Requesting Employee")]
        public EmployeeViewModel RequestingEmployee { get; set; }
        public string RequestingEmployeeId { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Leave Type")]
        public LeaveTypeViewModel LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime DateActioned { get; set; }
        public bool? Approved { get; set; }
        [Display(Name = "Approved By")]
        public EmployeeViewModel ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
    }

    public class AdminLeaveRequestViewVM
    {
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequested { get; set; }

        public List<LeaveRequestViewModel> LiveRequests { get; set; }
    }

    public class CreateLeaveRequestViewModel
    {
        [Display(Name ="Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name ="Leave Type")]
        public int LeaveTypeId { get; set; }
    }
    public class EmployeeLeaveRequestsViewModel
    {
        public List<LeaveAllocationViewModel> LeaveAllocations { get; set; }
        public List<LeaveRequestViewModel> LeaveRequests { get; set; }
    }
}
