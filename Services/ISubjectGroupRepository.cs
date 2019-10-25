using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.DAL;
using tuexamapi.Models;

namespace tuexamapi.Services
{
    public interface ISubjectGroupRepository
    {
        ICollection<SubjectGroup> GetSubjectGroups();
        SubjectGroup GetSubjectGroup(int id);
    }

    public class SubjectGroupRepository : ISubjectGroupRepository
    {
        private TuExamContext _context;
        public ICollection<SubjectGroup> GetSubjectGroups()
        {
            return _context.SubjectGroups.OrderBy(o=>o.ID).ToList();
        }
        public SubjectGroup GetSubjectGroup(int id)
        {
            return _context.SubjectGroups.Where(w=>w.ID == id).FirstOrDefault();
        }
    }
}
