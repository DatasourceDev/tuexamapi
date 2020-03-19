using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using tuexamapi.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace tuexamapi.DAL
{
    public class TuExamContext : DbContext
    {
        public TuExamContext(DbContextOptions options) : base(options) { }

        public TuExamContext() {
           
        }

        public DbSet<SubjectGroup> SubjectGroups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectSub> SubjectSubs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQRandom> TestQRandoms { get; set; }
        public DbSet<TestQCustom> TestQCustoms { get; set; }
        
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamRegister> ExamRegisters { get; set; }
        public DbSet<AttitudeSetup> AttitudeSetups { get; set; }
        public DbSet<ExamSetup> ExamSetups { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAns> QuestionAnies { get; set; }
        public DbSet<SendResultSetup> SendResultSetups { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestResultStudent> TestResultStudents { get; set; }
        public DbSet<TestResultStudentQAns> TestResultStudentQAnies { get; set; }
        public DbSet<TestResultStudentQAnsChild> TestResultStudentQAnsChilds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginStudentHistory> LoginStudentHistorys { get; set; }
        public DbSet<LoginStaffHistory> LoginStaffHistorys { get; set; }
        public DbSet<LoginToken> LoginTokens { get; set; }
        public DbSet<Faculty> Facultys { get; set; }
        public DbSet<TestApproval> TestApprovals { get; set; }
        public DbSet<TestApprovalStaff> TestApprovalStaffs { get; set; }

        public DbSet<QuestionApproval> QuestionApprovals { get; set; }
        public DbSet<QuestionApprovalStaff> QuestionApprovalStaffs { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelBuilder);
        }


    }
}
