using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.Models;

namespace tuexamapi.Util
{
    public static class EnumStatus
    {
        public static StatusType toStatus(this string text)
        {
            StatusType str = StatusType.Active;
            switch (text)
            {
                case "ใช้งาน":
                    str = StatusType.InActive;
                    break;
                case "ยกเลิก":
                    str = StatusType.Active;
                    break;
                case "0":
                    str = StatusType.InActive;
                    break;
                case "1":
                    str = StatusType.Active;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toStatusName(this StatusType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case StatusType.InActive:
                    str = "ยกเลิก";
                    break;
                case StatusType.Active:
                    str = "ใช้งาน";
                    break;
                default:
                    break;
            }
            return str;
        }


        public static Prefix toPrefix(this string text)
        {
            Prefix status = Prefix.Mr;
            switch (text)
            {
                case "นาย":
                    status = Prefix.Mr;
                    break;
                case "นางสาว":
                    status = Prefix.Miss;
                    break;
                case "นาง":
                    status = Prefix.Mrs;
                    break;
                case "0":
                    status = Prefix.Mr;
                    break;
                case "1":
                    status = Prefix.Miss;
                    break;
                case "2":
                    status = Prefix.Mrs;
                    break;
                default:
                    break;
            }
            return status;
        }
        public static string toPrefixName(this Prefix prefix)
        {
            string str = "";
            switch (prefix)
            {
                case Prefix.Mr:
                    str = "นาย";
                    break;
                case Prefix.Miss:
                    str = "นางสาว";
                    break;
                case Prefix.Mrs:
                    str = "นาง";
                    break;
                
                default:
                    break;
            }
            return str;
        }


        public static QuestionLevel toLevel(this string text)
        {
            QuestionLevel level = QuestionLevel.Mid;
            switch (text)
            {
                case "ง่ายมาก":
                    level = QuestionLevel.VeryEasy;
                    break;
                case "ง่าย":
                    level = QuestionLevel.Easy;
                    break;
                case "ปานกลาง":
                    level = QuestionLevel.Mid;
                    break;
                case "ยาก":
                    level = QuestionLevel.Hard;
                    break;
                case "ยากมาก":
                    level = QuestionLevel.VeryHard;
                    break;
                case "0":
                    level = QuestionLevel.VeryEasy;
                    break;
                case "1":
                    level = QuestionLevel.Easy;
                    break;
                case "2":
                    level = QuestionLevel.Mid;
                    break;
                case "3":
                    level = QuestionLevel.Hard;
                    break;
                case "4":
                    level = QuestionLevel.VeryHard;
                    break;
                default:
                    break;
            }
            return level;
        }


        public static Course toCourse(this string text)
        {
            Course course = Course.Thai;
            switch (text)
            {
                case "ไทย":
                    course = Course.Thai;
                    break;
                case "อังกฤษ":
                    course = Course.English;
                    break;
                case "สองภาษา":
                    course = Course.Bilingual;
                    break;
                case "0":
                    course = Course.Thai;
                    break;
                case "1":
                    course = Course.English;
                    break;
                case "2":
                    course = Course.Bilingual;
                    break;
                default:
                    break;
            }
            return course;
        }
        public static string toCourseName(this Course course)
        {
            string status = "";
            switch (course)
            {
                case Course.Thai:
                    status = "ไทย";
                    break;
                case Course.English:
                    status = "อังกฤษ";
                    break;
                case Course.Bilingual:
                    status = "สองภาษา";
                    break;
                default:
                    break;
            }
            return status;
        }


        public static QuestionApprovalType toApprovalStatus(this string text)
        {
            QuestionApprovalType str = QuestionApprovalType.Draft;
            switch (text)
            {
                case "ร่าง":
                    str = QuestionApprovalType.Draft;
                    break;
                case "รอการกลั่นกรอง":
                    str = QuestionApprovalType.Pending;
                    break;
                case "กลั่นกรองแล้ว":
                    str = QuestionApprovalType.Approved;
                    break;
                case "ไม่ผ่านการกลั่นกรอง":
                    str = QuestionApprovalType.Reject;
                    break;
                case "0":
                    str = QuestionApprovalType.Draft;
                    break;
                case "1":
                    str = QuestionApprovalType.Pending;
                    break;
                case "2":
                    str = QuestionApprovalType.Approved;
                    break;
                case "3":
                    str = QuestionApprovalType.Reject;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toApprovalStatusName(this QuestionApprovalType statusType)
        {
            string status = "";
            switch (statusType)
            {
                case QuestionApprovalType.Draft:
                    status = "ร่าง";
                    break;
                case QuestionApprovalType.Pending:
                    status = "รอการกลั่นกรอง";
                    break;
                case QuestionApprovalType.Approved:
                    status = "กลั่นกรองแล้ว";
                    break;
                case QuestionApprovalType.Reject:
                    status = "ไม่ผ่านการกลั่นกรอง";
                    break;
                default:
                    break;
            }
            return status;
        }


        public static QuestionType toQuestionType(this string text)
        {
            QuestionType str = QuestionType.MultipleChoice;
            switch (text)
            {
                case "ข้อสอบแบบเลือกตอบ (Multiple Choice)":
                    str = QuestionType.MultipleChoice;
                    break;
                case "ข้อสอบแบบถูก-ผิด (True-False)":
                    str = QuestionType.TrueFalse;
                    break;
                case "ข้อสอบแบบจับคู่ (Multiple Matching)":
                    str = QuestionType.MultipleMatching;
                    break;
                case "ข้อสอบแบบตอบสั้น (Short Answer)":
                    str = QuestionType.ShortAnswer;
                    break;
                case "ข้อสอบอัตนัยหรือเรียงความ (Essay)":
                    str = QuestionType.Essay;
                    break;
                case "ข้อสอบแบบส่งงาน (Assignment)":
                    str = QuestionType.Assignment;
                    break;
                case "ข้อสอบแบบเลือกตอบประกอบบทความ (Reading Text And Multiple Choice)":
                    str = QuestionType.ReadingText;
                    break;
                case "ข้อสอบแบบวัดทัศนคติ (Attitude)":
                    str = QuestionType.Attitude;
                    break;
                case "1":
                    str = QuestionType.MultipleChoice;
                    break;
                case "2":
                    str = QuestionType.TrueFalse;
                    break;
                case "3":
                    str = QuestionType.MultipleMatching;
                    break;
                case "4":
                    str = QuestionType.ShortAnswer;
                    break;
                case "5":
                    str = QuestionType.Essay;
                    break;
                case "6":
                    str = QuestionType.Assignment;
                    break;
                case "7":
                    str = QuestionType.ReadingText;
                    break;
                case "8":
                    str = QuestionType.Attitude;
                    break;
                case "9":
                    str = QuestionType.ReadingMultipleChoice;
                    break;
                case "10":
                    str = QuestionType.ReadingTrueFalse;
                    break;
                case "11":
                    str = QuestionType.ReadingMultipleMatching;
                    break;
                case "12":
                    str = QuestionType.ReadingShortAnswer;
                    break;
                case "13":
                    str = QuestionType.ReadingEssay;
                    break;
                case "14":
                    str = QuestionType.ReadingAssignment;
                    break;
                case "15":
                    str = QuestionType.ReadingAttitude;
                    break;
                case "16":
                    str = QuestionType.MultipleMatchingSub;
                    break;
                case "MC":
                    str = QuestionType.MultipleChoice;
                    break;
                case "TF":
                    str = QuestionType.TrueFalse;
                    break;
                case "MM":
                    str = QuestionType.MultipleMatching;
                    break;
                case "MS":
                    str = QuestionType.MultipleMatchingSub;
                    break;
                case "SA":
                    str = QuestionType.ShortAnswer;
                    break;
                case "ES":
                    str = QuestionType.Essay;
                    break;
                case "AS":
                    str = QuestionType.Assignment;
                    break;
                case "RT":
                    str = QuestionType.ReadingText;
                    break;
                case "AT":
                    str = QuestionType.Attitude;
                    break;
                case "RMC":
                    str = QuestionType.ReadingMultipleChoice;
                    break;
                case "RTF":
                    str = QuestionType.ReadingTrueFalse;
                    break;
                case "RMM":
                    str = QuestionType.ReadingMultipleMatching;
                    break;
                case "RSA":
                    str = QuestionType.ReadingShortAnswer;
                    break;
                case "RES":
                    str = QuestionType.ReadingEssay;
                    break;
                case "RAS":
                    str = QuestionType.ReadingAssignment;
                    break;
                case "RAT":
                    str = QuestionType.ReadingAttitude;
                    break;

                default:
                    break;
            }
            return str;
        }
        public static string toQuestionType(this QuestionType type)
        {
            string str = "";
            switch (type)
            {
                case QuestionType.MultipleChoice:
                    str = "ข้อสอบแบบเลือกตอบ (Multiple Choice)";
                    break;
                case QuestionType.TrueFalse:
                    str = "ข้อสอบแบบถูก-ผิด (True-False)";
                    break;
                case QuestionType.MultipleMatching:
                    str = "ข้อสอบแบบจับคู่ (Multiple Matching)";
                    break;
                case QuestionType.ShortAnswer:
                    str = "ข้อสอบแบบตอบสั้น (Short Answer)";
                    break;
                case QuestionType.Essay:
                    str = "อสอบอัตนัยหรือเรียงความ (Essay)";
                    break;
                case QuestionType.Assignment:
                    str = "ข้อสอบแบบส่งงาน (Assignment)";
                    break;
                case QuestionType.ReadingText:
                    str = "ข้อสอบแบบเลือกตอบประกอบบทความ (Reading Text And Multiple Choice)";
                    break;
                case QuestionType.Attitude:
                    str = "ข้อสอบแบบวัดทัศนคติ (Attitude)";
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toQuestionTypeMin(this QuestionType type)
        {
            string str = "";
            switch (type)
            {
                case QuestionType.MultipleChoice:
                    str = "Multiple Choice";
                    break;
                case QuestionType.TrueFalse:
                    str = "True-False";
                    break;
                case QuestionType.MultipleMatching:
                    str = "Multiple Matching";
                    break;
                case QuestionType.ShortAnswer:
                    str = "Short Answer";
                    break;
                case QuestionType.Essay:
                    str = "Essay";
                    break;
                case QuestionType.Assignment:
                    str = "Assignment";
                    break;
                case QuestionType.ReadingText:
                    str = "Reading Text And Multiple Choice";
                    break;
                case QuestionType.Attitude:
                    str = "Attitude";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static string toQuestionTypeMin2(this QuestionType type)
        {
            string str = "";
            switch (type)
            {
                case QuestionType.MultipleChoice:
                    str = "MC";
                    break;
                case QuestionType.TrueFalse:
                    str = "TF";
                    break;
                case QuestionType.MultipleMatching:
                    str = "MM";
                    break;
                case QuestionType.MultipleMatchingSub:
                    str = "MS";
                    break;
                case QuestionType.ShortAnswer:
                    str = "SA";
                    break;
                case QuestionType.Essay:
                    str = "ES";
                    break;
                case QuestionType.Assignment:
                    str = "AS";
                    break;
                case QuestionType.ReadingText:
                    str = "RT";
                    break;
                case QuestionType.Attitude:
                    str = "AT";
                    break;
                default:
                    break;
            }
            return str;
        }
        public static TestApprovalType toTestApprovalStatus(this string text)
        {
            TestApprovalType str = TestApprovalType.Draft;
            switch (text)
            {
                case "ร่าง":
                    str = TestApprovalType.Draft;
                    break;
                case "รอการคัดเลือก":
                    str = TestApprovalType.Pending;
                    break;
                case "คัดเลือกแล้ว":
                    str = TestApprovalType.Approved;
                    break;
                case "ไม่ผ่านการคัดเลือก":
                    str = TestApprovalType.Reject;
                    break;
                case "0":
                    str = TestApprovalType.Draft;
                    break;
                case "1":
                    str = TestApprovalType.Pending;
                    break;
                case "2":
                    str = TestApprovalType.Approved;
                    break;
                case "3":
                    str = TestApprovalType.Reject;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toTestApprovalStatusName(this TestApprovalType statusType)
        {
            string status = "";
            switch (statusType)
            {
                case TestApprovalType.Draft:
                    status = "ร่าง";
                    break;
                case TestApprovalType.Pending:
                    status = "รอการคัดเลือก";
                    break;
                case TestApprovalType.Approved:
                    status = "คัดเลือกแล้ว";
                    break;
                case TestApprovalType.Reject:
                    status = "ไม่ผ่านการคัดเลือก";
                    break;
                default:
                    break;
            }
            return status;
        }

        public static QuestionApprovalType toQuestionApprovalStatus(this string text)
        {
            QuestionApprovalType str = QuestionApprovalType.Draft;
            switch (text)
            {
                case "ร่าง":
                    str = QuestionApprovalType.Draft;
                    break;
                case "รอการกลั่นกรอง":
                    str = QuestionApprovalType.Pending;
                    break;
                case "กลั่นกรองแล้ว":
                    str = QuestionApprovalType.Approved;
                    break;
                case "ไม่ผ่านการกลั่นกรอง":
                    str = QuestionApprovalType.Reject;
                    break;
                case "0":
                    str = QuestionApprovalType.Draft;
                    break;
                case "1":
                    str = QuestionApprovalType.Pending;
                    break;
                case "2":
                    str = QuestionApprovalType.Approved;
                    break;
                case "3":
                    str = QuestionApprovalType.Reject;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toQuestionApprovalStatusName(this QuestionApprovalType statusType)
        {
            string status = "";
            switch (statusType)
            {
                case QuestionApprovalType.Draft:
                    status = "ร่าง";
                    break;
                case QuestionApprovalType.Pending:
                    status = "รอการกลั่นกรอง";
                    break;
                case QuestionApprovalType.Approved:
                    status = "กลั่นกรองแล้ว";
                    break;
                case QuestionApprovalType.Reject:
                    status = "ไม่ผ่านการกลั่นกรอง";
                    break;
                default:
                    break;
            }
            return status;
        }
        public static QuestionLevel toQuestionLevel(this string text)
        {
            QuestionLevel lvl = QuestionLevel.Mid;
            switch (text)
            {
                case "ง่ายมาก":
                    lvl = QuestionLevel.VeryEasy;
                    break;
                case "ง่าย":
                    lvl = QuestionLevel.Easy;
                    break;
                case "ปานกลาง":
                    lvl = QuestionLevel.Mid;
                    break;
                case "ยาก":
                    lvl = QuestionLevel.Hard;
                    break;
                case "ยากมาก":
                    lvl = QuestionLevel.VeryHard;
                    break;
                case "0":
                    lvl = QuestionLevel.VeryEasy;
                    break;
                case "1":
                    lvl = QuestionLevel.Easy;
                    break;
                case "2":
                    lvl = QuestionLevel.Mid;
                    break;
                case "3":
                    lvl = QuestionLevel.Hard;
                    break;
                case "4":
                    lvl = QuestionLevel.VeryHard;
                    break;
                default:
                    break;
            }
            return lvl;
        }
        public static string toQuestionLevelName(this QuestionLevel lvl)
        {
            string status = "";
            switch (lvl)
            {
                case QuestionLevel.VeryEasy:
                    status = "ง่ายมาก";
                    break;
                case QuestionLevel.Easy:
                    status = "ง่าย";
                    break;
                case QuestionLevel.Mid:
                    status = "ปานกลาง";
                    break;
                case QuestionLevel.Hard:
                    status = "ยาก";
                    break;
                case QuestionLevel.VeryHard:
                    status = "ยากมาก";
                    break;
                default:
                    break;
            }
            return status;
        }

        public static AnswerType toAnswerType(this string text)
        {
            AnswerType str = AnswerType.Point;
            switch (text)
            {
                case "คะแนน":
                    str = AnswerType.Point;
                    break;
                case "วิชาย่อย":
                    str = AnswerType.SubjectSub;
                    break;               
                case "0":
                    str = AnswerType.Point;
                    break;
                case "1":
                    str = AnswerType.SubjectSub;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toAnswerType(this AnswerType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case AnswerType.Point:
                    str = "คะแนน";
                    break;
                case AnswerType.SubjectSub:
                    str = "วิชาย่อย";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static ExamPeriod toExamPeriod(this string text)
        {
            ExamPeriod str = ExamPeriod.Morning;
            switch (text)
            {
                case "เช้า":
                    str = ExamPeriod.Morning;
                    break;
                case "บ่าย":
                    str = ExamPeriod.Afternoon;
                    break;
                case "ค่ำ":
                    str = ExamPeriod.Evening;
                    break;
                case "0":
                    str = ExamPeriod.Morning;
                    break;
                case "1":
                    str = ExamPeriod.Afternoon;
                    break;
                case "2":
                    str = ExamPeriod.Evening;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toExamPeriodName(this ExamPeriod statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ExamPeriod.Morning:
                    str = "เช้า";
                    break;
                case ExamPeriod.Afternoon:
                    str = "บ่าย";
                    break;
                case ExamPeriod.Evening:
                    str = "ค่ำ";
                    break;
                default:
                    break;
            }
            return str;
        }


        public static ExamTestType toExamTestType(this string text)
        {
            ExamTestType str = ExamTestType.Random;
            switch (text)
            {
                case "สุ่ม":
                    str = ExamTestType.Random;
                    break;
                case "กำหนดเอง":
                    str = ExamTestType.Custom;
                    break;
                case "0":
                    str = ExamTestType.Random;
                    break;
                case "1":
                    str = ExamTestType.Custom;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toExamTestType(this ExamTestType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ExamTestType.Random:
                    str = "สุ่ม";
                    break;
                case ExamTestType.Custom:
                    str = "กำหนดเอง";
                    break;
              
                default:
                    break;
            }
            return str;
        }

        public static TimeType toTimeType(this string text)
        {
            TimeType str = TimeType.Second;
            switch (text)
            {
                case "วินาที":
                    str = TimeType.Second;
                    break;
                case "นาที":
                    str = TimeType.Minute;
                    break;
                case "ชั่วโมง":
                    str = TimeType.Hour;
                    break;
                case "0":
                    str = TimeType.Second;
                    break;
                case "1":
                    str = TimeType.Minute;
                    break;
                case "2":
                    str = TimeType.Hour;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toTimeType(this TimeType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case TimeType.Second:
                    str = "second";
                    break;
                case TimeType.Minute:
                    str = "minute";
                    break;
                case TimeType.Hour:
                    str = "hour";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static ExamingStatus toExamingStatus(this string text)
        {
            ExamingStatus str = ExamingStatus.None;
            switch (text)
            {
                case "ยังไม่เริ่มสอบ":
                    str = ExamingStatus.None;
                    break;
                case "กำลังสอบ":
                    str = ExamingStatus.Examing;
                    break;
                case "ไม่เข้าสอบ":
                    str = ExamingStatus.Absent;
                    break;
                case "สิ้นสุดแบบทดสอบ":
                    str = ExamingStatus.Done;
                    break;
                case "0":
                    str = ExamingStatus.None;
                    break;
                case "1":
                    str = ExamingStatus.Examing;
                    break;
                case "2":
                    str = ExamingStatus.Absent;
                    break;
                case "3":
                    str = ExamingStatus.Done;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toExamingStatus(this ExamingStatus statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ExamingStatus.None:
                    str = "ยังไม่เริ่มสอบ";
                    break;
                case ExamingStatus.Examing:
                    str = "กำลังสอบ";
                    break;
                case ExamingStatus.Absent:
                    str = "ไม่เข้าสอบ";
                    break;
                case ExamingStatus.Done:
                    str = "สิ้นสุดแบบทดสอบ";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static ProveStatus toProveStatus(this string text)
        {
            ProveStatus str = ProveStatus.Pending;
            switch (text)
            {
                case "รอตรวจสอบ":
                    str = ProveStatus.Pending;
                    break;
                case "ตรวจแล้ว":
                    str = ProveStatus.Proved;
                    break;
                case "0":
                    str = ProveStatus.Pending;
                    break;
                case "1":
                    str = ProveStatus.Proved;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toProveStatusName(this ProveStatus statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ProveStatus.Pending:
                    str = "รอตรวจสอบ";
                    break;
                case ProveStatus.Proved:
                    str = "ตรวจแล้ว";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static AttitudeAnsType toAttitudeAnsType(this string text)
        {
            AttitudeAnsType str = AttitudeAnsType.Type3;
            switch (text)
            {
                case "2 ตัวเลือก":
                    str = AttitudeAnsType.Type2;
                    break;
                case "3 ตัวเลือก":
                    str = AttitudeAnsType.Type3;
                    break;
                case "4 ตัวเลือก":
                    str = AttitudeAnsType.Type4;
                    break;
                case "5 ตัวเลือก":
                    str = AttitudeAnsType.Type5;
                    break;
                case "6 ตัวเลือก":
                    str = AttitudeAnsType.Type6;
                    break;
                case "7 ตัวเลือก":
                    str = AttitudeAnsType.Type7;
                    break;
                case "2":
                    str = AttitudeAnsType.Type2;
                    break;
                case "3":
                    str = AttitudeAnsType.Type3;
                    break;
                case "4":
                    str = AttitudeAnsType.Type4;
                    break;
                case "5":
                    str = AttitudeAnsType.Type5;
                    break;
                case "6":
                    str = AttitudeAnsType.Type6;
                    break;
                case "7":
                    str = AttitudeAnsType.Type7;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toAttitudeAnsTypeName(this AttitudeAnsType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case AttitudeAnsType.Type2:
                    str = "2 ตัวเลือก";
                    break;
                case AttitudeAnsType.Type3:
                    str = "3 ตัวเลือก";
                    break;
                case AttitudeAnsType.Type4:
                    str = "4 ตัวเลือก";
                    break;
                case AttitudeAnsType.Type5:
                    str = "5 ตัวเลือก";
                    break;
                case AttitudeAnsType.Type6:
                    str = "6 ตัวเลือก";
                    break;
                case AttitudeAnsType.Type7:
                    str = "7 ตัวเลือก";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static AttitudeAnsSubType toAttitudeAnsSubType(this string text)
        {
            AttitudeAnsSubType str = AttitudeAnsSubType.Sub1;
            switch (text)
            {
                case "แบบที่ 1":
                    str = AttitudeAnsSubType.Sub1;
                    break;
                case "แบบที่ 2":
                    str = AttitudeAnsSubType.Sub2;
                    break;
                case "1":
                    str = AttitudeAnsSubType.Sub1;
                    break;
                case "2":
                    str = AttitudeAnsSubType.Sub2;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toAttitudeAnsSubType(this AttitudeAnsSubType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case AttitudeAnsSubType.Sub1:
                    str = "แบบที่ 1";
                    break;
                case AttitudeAnsSubType.Sub2:
                    str = "แบบที่ 2";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static AuthType toAuthType(this string text)
        {
            AuthType str = AuthType.Login;
            switch (text)
            {
                case "เข้าสู่ระบบ":
                    str = AuthType.Login;
                    break;
                case "ออกจากระบบ":
                    str = AuthType.Logout;
                    break;
                case "0":
                    str = AuthType.Login;
                    break;
                case "1":
                    str = AuthType.Logout;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toAuthType(this AuthType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case AuthType.Login:
                    str = "เข้าสู่ระบบ";
                    break;
                case AuthType.Logout:
                    str = "ออกจากระบบ";
                    break;
                default:
                    break;
            }
            return str;
        }


        public static ExamRegisterType toExamRegisterType(this string text)
        {
            ExamRegisterType str = ExamRegisterType.Advance;
            switch (text)
            {
                case "ล่วงหน้า":
                    str = ExamRegisterType.Advance;
                    break;
                case "Walk In":
                    str = ExamRegisterType.WalkIn;
                    break;
                case "0":
                    str = ExamRegisterType.Advance;
                    break;
                case "1":
                    str = ExamRegisterType.WalkIn;
                    break;
                default:
                    break;
            }
            return str;
        }
        public static string toExamRegisterType(this ExamRegisterType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ExamRegisterType.Advance:
                    str = "ล่วงหน้า";
                    break;
                case ExamRegisterType.WalkIn:
                    str = "Walk In";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static string toTestDoExamType(this TestDoExamType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case TestDoExamType.ForwardAndBackword:
                    str = "เดินหน้าและถอยหลัง";
                    break;
                case TestDoExamType.ForwardOnly:
                    str = "เดินหน้าอย่างเดียว";
                    break;                
                default:
                    break;
            }
            return str;
        }

        public static TestDoExamType toTestDoExamType(this string text)
        {
            TestDoExamType str = TestDoExamType.ForwardAndBackword;
            switch (text)
            {
                case "เดินหน้าและถอยหลัง":
                    str = TestDoExamType.ForwardAndBackword;
                    break;
                case "เดินหน้าอย่างเดียว":
                    str = TestDoExamType.ForwardOnly;
                    break;
              
                case "0":
                    str = TestDoExamType.ForwardAndBackword;
                    break;
                case "1":
                    str = TestDoExamType.ForwardOnly;
                    break;                
                default:
                    break;
            }
            return str;
        }

        public static string toTestQuestionType(this TestQuestionType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case TestQuestionType.Custom:
                    str = "กำหนดเอง";
                    break;
                case TestQuestionType.Random:
                    str = "สุ่ม";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static TestQuestionType toTestQuestionType(this string text)
        {
            TestQuestionType str = TestQuestionType.Custom;
            switch (text)
            {
                case "กำหนดเอง":
                    str = TestQuestionType.Custom;
                    break;
                case "สุ่ม":
                    str = TestQuestionType.Random;
                    break;

                case "0":
                    str = TestQuestionType.Custom;
                    break;
                case "1":
                    str = TestQuestionType.Random;
                    break;
                default:
                    break;
            }
            return str;
        }

        public static string toTestCustomOrderType(this TestCustomOrderType statusType)
        {
            string str = "";
            switch (statusType)
            {
                case TestCustomOrderType.Order:
                    str = "เรียงลำดับ";
                    break;
                case TestCustomOrderType.Random:
                    str = "สลับข้อ";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static TestCustomOrderType toTestCustomOrderType(this string text)
        {
            TestCustomOrderType str = TestCustomOrderType.Order;
            switch (text)
            {
                case "เรียงลำดับ":
                    str = TestCustomOrderType.Order;
                    break;
                case "สลับข้อ":
                    str = TestCustomOrderType.Random;
                    break;

                case "0":
                    str = TestCustomOrderType.Order;
                    break;
                case "1":
                    str = TestCustomOrderType.Random;
                    break;
                default:
                    break;
            }
            return str;
        }

        public static string toShowResult(this ShowResult statusType)
        {
            string str = "";
            switch (statusType)
            {
                case ShowResult.Yes:
                    str = "แสดง";
                    break;
                case ShowResult.No:
                    str = "ไม่แสดง";
                    break;
                default:
                    break;
            }
            return str;
        }

        public static ShowResult toShowResult(this string text)
        {
            ShowResult str = ShowResult.Yes;
            switch (text)
            {
                case "แสดง":
                    str = ShowResult.Yes;
                    break;
                case "ไม่แสดง":
                    str = ShowResult.No;
                    break;

                case "0":
                    str = ShowResult.Yes;
                    break;
                case "1":
                    str = ShowResult.No;
                    break;
                default:
                    break;
            }
            return str;
        }

    }
}
