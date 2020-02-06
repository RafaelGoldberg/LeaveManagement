using LeaveManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
        //  ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string employeeId);
        ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeId);
    }
}
