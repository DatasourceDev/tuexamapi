using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class AppConst
    {
    }

    public enum StatusType
    {
        InActive = 0,
        Active = 1,
    }

    public enum QuestionType
    {
        MultipleChoice = 1,
        TrueFalse,
        MultipleMatching,
        ShortAnswer,
        Essay,
        Assignment,
        ReadingTextAndMultipleChoice,
        Attitude
    }

    public enum Course
    {
        Thai = 0,
        English,
        Bilingual,
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

    public enum Prefix
    {
        Mr = 0,
        Miss,
        Mrs,
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
        Type3,
        Type4,
        Type5,
        Type6,
        Type7,
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
    }

    public static class ResultMessage
    {
        public static string Success = "Success";
        public static string InputHasNotFound = "Input has not found.";
        public static string DataHasNotFound = "Data has not found.";
        public static string DuplicateData = "Duplicate data";
        public static string InvalidInput = "Invalid input";
    }
}
