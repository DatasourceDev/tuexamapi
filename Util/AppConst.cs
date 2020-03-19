using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class AppConst
    {
    }
    public enum SubjectType
    {
        Type1 = 1,
        Type2 ,
        Type3,
    }
    public enum StatusType
    {
        InActive = 0,
        Active = 1,
    }
    public enum Prefix
    {
        Mr = 0,
        Miss = 1,
        Mrs = 2,
    }
    public enum QuestionType
    {
        MultipleChoice = 1,
        TrueFalse =2,
        MultipleMatching =3,
        ShortAnswer =4,
        Essay =5,
        Assignment =6,
        ReadingText = 7,
        Attitude = 8,
        ReadingMultipleChoice =9,
        ReadingTrueFalse =10,
        ReadingMultipleMatching = 11,
        ReadingShortAnswer =12,
        ReadingEssay =13,
        ReadingAssignment =14,
        ReadingAttitude =15,
        MultipleMatchingSub =16,
    }

    public enum Course
    {
        Thai = 0,
        English,
        Bilingual,
    }
    public enum ShowResult
    {
        Yes = 0,
        No,
    }
    public enum QuestionLevel
    {
        VeryEasy = 0,
        Easy,
        Mid,
        Hard,
        VeryHard
    }

    public enum QuestionApprovalType
    {
        Draft = 0,
        Pending,
        Approved,
        Reject,
    }
    public enum TestApprovalType
    {
        Draft = 0,
        Pending,
        Approved,
        Reject,
    }
    public enum TimeType
    {
        Second = 0,
        Minute,
        Hour,
    }

   
    public enum Role
    {
        Admin = 0,
        SuperAdmin,
        QuestionApproval,
        SuperQuestionApproval,
        TestApproval,
        SuperTestApproval,
    }

    public enum TestQuestionType
    {
        Random = 0,
        Custom,
    }

    public enum TestCustomOrderType
    {
        Order = 0,
        Random,
    }

    public enum TestDoExamType
    {
        ForwardAndBackword = 0,
        ForwardOnly,
    }

    public enum ExamPeriod
    {
        Morning = 0,
        Afternoon,
        Evening,
    }

    public enum ExamTestType
    {
        Random = 0,
        Custom,
    }

    public enum ExamRegisterType
    {
        Advance = 0,
        WalkIn,
    }

    public enum AttitudeAnsType
    {
        Type2 = 2,
        Type3,
        Type4,
        Type5,
        Type6,
        Type7,
    }
    public enum AttitudeAnsSubType
    {
        Sub1 = 1,
        Sub2,
    }
    public enum ProveStatus
    {
        Pending,
        Proved,
    }

    public enum ResultCode
    {
        Success = 200,
        InputHasNotFound = -100,
        DataHasNotFound = -101,
        DuplicateData = -102,
        InvalidInput = -103,
        DataInUse = -104,
        InactiveAccount = -105,
        WrongAccountorPassword = -106,
    }

    public static class ResultMessage
    {
        public static string Success = "สำเร็จ";
        public static string InputHasNotFound = "ไม่พบข้อมูลที่ระบุ";
        public static string DataHasNotFound = "ไม่พบข้อมูล";
        public static string DuplicateData = "ข้อมูลซ้ำในระบบ";
        public static string InvalidInput = "ระบุข้อมูลไม่ถูกต้อง";
        public static string DataInUse = "ไม่สามารถลบข้อมูลได้ เนื่องจากรายการนี้ได้ถูกนำไปใช้งานแล้ว";
        public static string InactiveAccount = "ผู้ใช้นี้ถูกระงับการใช้งาน";
        public static string WrongAccountorPassword = "รหัสผู้ใช้หรือรหัสผ่านผิดพลาด";
    }

    public enum ExamingStatus
    {
        None,
        Examing,
        Absent,
        Done,
    }

    public enum AuthType
    {
        Login,
        Logout
    }
}
