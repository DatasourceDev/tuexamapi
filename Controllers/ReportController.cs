using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.DTO;
using tuexamapi.Models;
using tuexamapi.Util;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Net.Mail;

namespace tuexamapi.Controllers
{
    public class Rpt
    {
        public static List<RptExamByDate> exambydate(TuExamContext _context, IOrderedQueryable<Subject> subjects, string from_search, string to_search, int? group_search)
        {
            var lists = new List<RptExamByDate>();


            var from = DateUtil.ToDate(from_search).Value.Date;
            var to = DateUtil.ToDate(to_search).Value.Date;

            var curr = from;
            while (curr <= to)
            {
                var rpt = new RptExamByDate();
                rpt.examdate = DateUtil.ToDisplayDate(curr);
                rpt.date = curr;
                rpt.studentbysubject = new List<decimal>();
                var registerd = _context.ExamRegisters.Where(w => w.Exam.ExamDate.Value.Date == curr & w.Exam.SubjectGroupID == group_search);
                foreach (var subject in subjects)
                {
                    var cnt = registerd.Where(w => w.Exam.SubjectID == subject.ID).Count();
                    rpt.studentbysubject.Add(cnt);
                }
                rpt.studentadvance = registerd.Where(w => w.ExamRegisterType == ExamRegisterType.Advance).Count();
                rpt.studentwalkin = registerd.Where(w => w.ExamRegisterType == ExamRegisterType.WalkIn).Count();
                rpt.total = NumUtil.ParseDecimal(rpt.studentadvance + rpt.studentwalkin);
                lists.Add(rpt);
                curr = curr.AddDays(1);
            }

            return lists.OrderByDescending(o => o.date).ToList();
        }

        public static List<RptExamByYear> exambymonth(TuExamContext _context, IOrderedQueryable<Subject> subjects, int? from_search, int? to_search, int? group_search)
        {
            var lists = new List<RptExamByYear>();

            var fromyear = NumUtil.ParseInteger(from_search);
            var toyear = NumUtil.ParseInteger(to_search);

            var curryear = fromyear;
            while (fromyear <= toyear)
            {
                for (var i = 1; i <= 12; i++)
                {
                    var rpt = new RptExamByYear();
                    rpt.year = fromyear;
                    rpt.month = i;
                    rpt.fullmonth = DateUtil.GetFullMonth(i);
                    rpt.studentbysubject = new List<decimal>();
                    var registerd = _context.ExamRegisters.Where(w => w.Exam.ExamDate.Value.Year == fromyear & w.Exam.ExamDate.Value.Month == i & w.Exam.SubjectGroupID == group_search);
                    foreach (var subject in subjects)
                    {
                        var cnt = registerd.Where(w => w.Exam.SubjectID == subject.ID).Count();
                        rpt.studentbysubject.Add(cnt);
                    }
                    rpt.studentadvance = registerd.Where(w => w.ExamRegisterType == ExamRegisterType.Advance).Count();
                    rpt.studentwalkin = registerd.Where(w => w.ExamRegisterType == ExamRegisterType.WalkIn).Count();
                    rpt.total = NumUtil.ParseDecimal(rpt.studentadvance + rpt.studentwalkin);
                    lists.Add(rpt);
                }
                fromyear++;
            }
            return lists.OrderByDescending(o => o.year).ThenBy(o => o.month).ToList();
        }

        public static List<RptExamStudent> examstudentall(TuExamContext _context, string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, string period_search)
        {
            var exam = _context.TestResultStudents.Include(i => i.Exam).Include(i => i.Test).Include(i => i.Exam.SubjectGroup).Where(w => 1 == 1);
            var register = _context.ExamRegisters.Include(i => i.Exam).Include(i => i.Exam.SubjectGroup).Where(w => 1 == 1);


            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                {
                    exam = exam.Where(w => w.Exam.SubjectID == subjectID);
                    register = register.Where(w => w.Exam.SubjectID == subjectID);
                }
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                {
                    exam = exam.Where(w => w.Exam.SubjectGroupID == groupID);
                    register = register.Where(w => w.Exam.SubjectGroupID == groupID);
                }
            }
            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                exam = exam.Where(w => w.Exam.ExamDate >= date);
                register = register.Where(w => w.Exam.ExamDate >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                exam = exam.Where(w => w.Exam.ExamDate <= date);
                register = register.Where(w => w.Exam.ExamDate <= date);
            }
            if (!string.IsNullOrEmpty(period_search))
            {
                exam = exam.Where(w => w.Exam.ExamPeriod == period_search.toExamPeriod());
                register = register.Where(w => w.Exam.ExamPeriod == period_search.toExamPeriod());
            }

            var studentIDs = exam.Select(s => s.StudentID);
            if (studentIDs.Count() > 0)
                register = register.Where(w => !studentIDs.Contains(w.StudentID));


            if (!string.IsNullOrEmpty(text_search))
            {
                if (!string.IsNullOrEmpty(text_search))
                {
                    exam = exam.Where(w => w.Student.FirstName.Contains(text_search)
                      | w.Student.LastName.Contains(text_search)
                      | w.Student.FirstNameEn.Contains(text_search)
                      | w.Student.LastNameEn.Contains(text_search)
                      | w.Student.IDCard.Contains(text_search)
                      | w.Student.Phone.Contains(text_search)
                      | w.Student.Email.Contains(text_search)
                      | w.Student.Passport.Contains(text_search)
                      | w.Student.StudentCode.Contains(text_search)
                      | (w.Student.FirstName + " " + w.Student.LastName).Contains(text_search)
                      | (w.Student.FirstNameEn + " " + w.Student.LastNameEn).Contains(text_search)
                      );

                    register = register.Where(w => w.Student.FirstName.Contains(text_search)
                        | w.Student.LastName.Contains(text_search)
                        | w.Student.FirstNameEn.Contains(text_search)
                        | w.Student.LastNameEn.Contains(text_search)
                        | w.Student.IDCard.Contains(text_search)
                        | w.Student.Phone.Contains(text_search)
                        | w.Student.Email.Contains(text_search)
                        | w.Student.Passport.Contains(text_search)
                        | w.Student.StudentCode.Contains(text_search)
                        | (w.Student.FirstName + " " + w.Student.LastName).Contains(text_search)
                        | (w.Student.FirstNameEn + " " + w.Student.LastNameEn).Contains(text_search)
                        );
                }
            }

            var lists = new List<RptExamStudent>();
            lists.AddRange(exam.Select(s => new RptExamStudent
            {
                test = s.Test.Name,
                group = s.Exam.SubjectGroup.Name,
                subject = s.Exam.Subject.Name,
                subjectorder = s.Exam.Subject.Order,
                examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                date = s.Exam.ExamDate,
                examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                examperiodid = s.Exam.ExamPeriod,
                examstatus = s.ExamingStatus.toExamingStatus(),
                examregistertype = s.ExamRegisterType.toExamRegisterType(),
                prefix = s.Student.Prefix.toPrefixName(),
                firstname = s.Student.FirstName,
                lastname = s.Student.LastName,
                firstnameen = s.Student.FirstNameEn,
                lastnameen = s.Student.LastNameEn,
                studentcode = s.Student.StudentCode,
            }).ToList());
            lists.AddRange(register.Select(s => new RptExamStudent
            {
                //test = s.Test.Name,
                group = s.Exam.SubjectGroup.Name,
                subject = s.Exam.Subject.Name,
                subjectorder = s.Exam.Subject.Order,
                examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                date = s.Exam.ExamDate,
                examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                examperiodid = s.Exam.ExamPeriod,
                examstatus = ExamingStatus.Absent.toExamingStatus(),
                examregistertype = s.ExamRegisterType.toExamRegisterType(),
                prefix = s.Student.Prefix.toPrefixName(),
                firstname = s.Student.FirstName,
                lastname = s.Student.LastName,
                firstnameen = s.Student.FirstNameEn,
                lastnameen = s.Student.LastNameEn,
                studentcode = s.Student.StudentCode,
            }).ToList());

            return lists.OrderByDescending(o => o.date).ThenBy(o => o.examperiodid).ThenBy(o => o.group).ThenBy(o => o.subjectorder).ThenBy(o => o.studentcode).ToList();

        }

        public static List<RptExamStudent> examstudent(TuExamContext _context, string text_search, string from_search, string to_search)
        {
            var tstudent = _context.TestResultStudents.Include(i => i.Student).Include(i => i.Exam).Include(i => i.Test).Include(i => i.Exam.SubjectGroup).Where(w => 1 == 1);


            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                tstudent = tstudent.Where(w => w.Exam.ExamDate >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                tstudent = tstudent.Where(w => w.Exam.ExamDate <= date);
            }
            var tstudents = new List<TestResultStudent>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        tstudents.AddRange(tstudent.Where(w => w.Student.FirstName.Contains(text)
                            | w.Student.LastName.Contains(text)
                            | w.Student.FirstNameEn.Contains(text)
                            | w.Student.LastNameEn.Contains(text)
                            | w.Student.IDCard.Contains(text)
                            | w.Student.Phone.Contains(text)
                            | w.Student.Email.Contains(text)
                            | w.Student.Passport.Contains(text)
                            | w.Student.StudentCode.Contains(text)
                            | (w.Student.FirstName + " " + w.Student.LastName).Contains(text)
                            | (w.Student.FirstNameEn + " " + w.Student.LastNameEn).Contains(text)
                        ));
                    }
                }
                tstudents = tstudents.Distinct().ToList();
            }
            else
            {
                tstudents = tstudent.ToList();
            }
            var lists = new List<RptExamStudent>();
            foreach (var item in tstudents)
            {
                if (lists.Where(w => w.studentid == item.StudentID).FirstOrDefault() != null)
                    continue;

                lists.Add(new RptExamStudent
                {
                    studentid = item.StudentID,
                    group = item.Exam.SubjectGroup.Name,
                    prefix = item.Student.Prefix.toPrefixName(),
                    firstname = item.Student.FirstName,
                    lastname = item.Student.LastName,
                    firstnameen = item.Student.FirstNameEn,
                    lastnameen = item.Student.LastNameEn,
                    studentcode = item.Student.StudentCode,
                });
            }
            return lists.OrderByDescending(o => o.date).ThenBy(o => o.examperiodid).ThenBy(o => o.group).ThenBy(o => o.subjectorder).ThenBy(o => o.studentcode).ToList();
        }

        public static RptExamStudentForm examstudentform(TuExamContext _context, Student student, string student_search, string greats)
        {
            var studentid = NumUtil.ParseInteger(student_search);

            var tresults = _context.TestResultStudents
                .Include(i => i.Exam)
                .Include(i => i.Test)
                .Include(i => i.Exam.SubjectGroup)
                .Include(i => i.Exam.Subject)
                .Where(w => w.StudentID == studentid);

            if (!string.IsNullOrEmpty(greats))
                tresults = tresults.Where(w => w.Exam.SubjectGroup.Name == "GREATS");
            else
                tresults = tresults.Where(w => w.Exam.SubjectGroup.Name != "GREATS");

            var lists = new RptExamStudentForm()
            {
                prefix = student.Prefix.toPrefixName(),
                firstname = student.FirstName,
                lastname = student.LastName,
                firstnameen = student.FirstNameEn,
                lastnameen = student.LastNameEn,
                studentcode = student.StudentCode,
                course = student.Course.toCourseName(),
                faculty = student.Faculty != null ? student.Faculty.FacultyName : "",
                data = tresults.Select(s => new RptExamStudentFormDtl
                {
                    id = s.ID,
                    test = s.Test.Name,
                    group = s.Exam.SubjectGroup.Name,
                    subject = s.Exam.Subject.Name,
                    subjectorder = s.Exam.Subject.Order,
                    examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                    starton = DateUtil.ToDisplayTime(s.Start_On),
                    date = s.Exam.ExamDate,
                    point = s.Point,
                    percent = (s.CorrectCnt / s.QuestionCnt) * 100,
                    questioncnt = s.QuestionCnt,
                    answeredcnt = s.AnsweredCnt,
                    correctcnt = s.CorrectCnt,
                    examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                    examperiodid = s.Exam.ExamPeriod,
                    examstatus = s.ExamingStatus.toExamingStatus(),
                    examregistertype = s.ExamRegisterType.toExamRegisterType(),
                }).OrderByDescending(o => o.date).ThenBy(o => o.subjectorder).ToList()
            };
            return lists;
        }

        public static RptExamStudentForm examstudentformbest(TuExamContext _context, Student student, string student_search)
        {
            var studentid = NumUtil.ParseInteger(student_search);

            var tresults = _context.TestResultStudents.Include(i => i.Exam).Include(i => i.Test).Include(i => i.Exam.SubjectGroup).Include(i => i.Exam.Subject).Where(w => w.StudentID == studentid);

            var tresults2 = tresults.Select(s => s.Exam.SubjectID).Distinct();

            var bests = new List<TestResultStudent>();
            foreach (var t2 in tresults2)
            {
                var best = tresults.Where(w => w.Exam.SubjectID == t2).OrderByDescending(o => (o.CorrectCnt / o.QuestionCnt) * 100).FirstOrDefault();
                if (best != null)
                    bests.Add(best);
            }

            var lists = new RptExamStudentForm()
            {
                prefix = student.Prefix.toPrefixName(),
                firstname = student.FirstName,
                lastname = student.LastName,
                firstnameen = student.FirstNameEn,
                lastnameen = student.LastNameEn,
                studentcode = student.StudentCode,
                course = student.Course.toCourseName(),
                data = bests.Select(s => new RptExamStudentFormDtl
                {
                    test = s.Test.Name,
                    group = s.Exam.SubjectGroup.Name,
                    subject = s.Exam.Subject.Name,
                    subjectorder = s.Exam.Subject.Order,
                    examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                    starton = DateUtil.ToDisplayTime(s.Start_On),
                    date = s.Exam.ExamDate,
                    percent = (s.CorrectCnt / s.QuestionCnt) * 100,
                    questioncnt = s.QuestionCnt,
                    answeredcnt = s.AnsweredCnt,
                    correctcnt = s.CorrectCnt,
                    examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                    examperiodid = s.Exam.ExamPeriod,
                    examstatus = s.ExamingStatus.toExamingStatus(),
                    examregistertype = s.ExamRegisterType.toExamRegisterType(),
                }).OrderBy(o => o.subjectorder).ToList()
            };

            return lists;
        }

        public static List<RptQuestionLevel> questionlevel(TuExamContext _context)
        {
            var lists = new List<RptQuestionLevel>();
            var mc = _context.Questions.Where(w => w.QuestionType == QuestionType.MultipleChoice & w.QuestionParentID == null);
            var tf = _context.Questions.Where(w => w.QuestionType == QuestionType.TrueFalse & w.QuestionParentID == null);
            var mm = _context.Questions.Where(w => w.QuestionType == QuestionType.MultipleMatching & w.QuestionParentID == null);
            var sa = _context.Questions.Where(w => w.QuestionType == QuestionType.ShortAnswer & w.QuestionParentID == null);
            var es = _context.Questions.Where(w => w.QuestionType == QuestionType.Essay & w.QuestionParentID == null);
            var ass = _context.Questions.Where(w => w.QuestionType == QuestionType.Assignment & w.QuestionParentID == null);
            var rt = _context.Questions.Where(w => w.QuestionType == QuestionType.ReadingText & w.QuestionParentID == null);
            var at = _context.Questions.Where(w => w.QuestionType == QuestionType.Attitude & w.QuestionParentID == null);

            lists.Add(getrptquestionlvl(mc, QuestionType.MultipleChoice.toQuestionType()));
            lists.Add(getrptquestionlvl(tf, QuestionType.TrueFalse.toQuestionType()));
            lists.Add(getrptquestionlvl(mm, QuestionType.MultipleMatching.toQuestionType()));
            lists.Add(getrptquestionlvl(sa, QuestionType.ShortAnswer.toQuestionType()));
            lists.Add(getrptquestionlvl(es, QuestionType.Essay.toQuestionType()));
            lists.Add(getrptquestionlvl(ass, QuestionType.Assignment.toQuestionType()));
            lists.Add(getrptquestionlvl(rt, QuestionType.ReadingText.toQuestionType()));
            lists.Add(getrptquestionlvl(at, QuestionType.Attitude.toQuestionType()));

            var total = new RptQuestionLevel();
            total.questiontype = "รวม";
            total.vereasy = lists.Sum(s => s.vereasy);
            total.easy = lists.Sum(s => s.easy);
            total.mid = lists.Sum(s => s.mid);
            total.hard = lists.Sum(s => s.hard);
            total.veryhard = lists.Sum(s => s.veryhard);
            total.total = lists.Sum(s => s.total);
            lists.Add(total);

            return lists;
        }

        private static RptQuestionLevel getrptquestionlvl(IQueryable<Question> questions, string questiontype)
        {
            var rptquestionlvl = new RptQuestionLevel();
            rptquestionlvl.questiontype = questiontype;
            rptquestionlvl.total = questions.Count();
            rptquestionlvl.vereasy = questions.Where(w => w.QuestionLevel == QuestionLevel.VeryEasy).Count();
            rptquestionlvl.easy = questions.Where(w => w.QuestionLevel == QuestionLevel.Easy).Count();
            rptquestionlvl.mid = questions.Where(w => w.QuestionLevel == QuestionLevel.Mid).Count();
            rptquestionlvl.hard = questions.Where(w => w.QuestionLevel == QuestionLevel.Hard).Count();
            rptquestionlvl.veryhard = questions.Where(w => w.QuestionLevel == QuestionLevel.VeryHard).Count();
            return rptquestionlvl;
        }

        public static List<RptQuestionAnalyze> questionanalyze(TuExamContext _context, string text_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search)
        {
            var questions = new List<Question>();
            var question = _context.Questions.Include(i => i.SubjectGroup).Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                {
                    question = question.Where(w => w.SubjectID == subjectID);
                }
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                {
                    question = question.Where(w => w.SubjectGroupID == groupID);
                }
            }
            if (!string.IsNullOrEmpty(sub_search))
            {
                var subID = NumUtil.ParseInteger(sub_search);
                if (subID > 0)
                {
                    question = question.Where(w => w.SubjectSubID == subID);
                }
            }
            if (!string.IsNullOrEmpty(status_search))
            {
                question = question.Where(w => w.Status == status_search.toStatus());
            }
            if (!string.IsNullOrEmpty(level_search))
            {
                question = question.Where(w => w.QuestionLevel == level_search.toQuestionLevel());
            }
            if (!string.IsNullOrEmpty(course_search))
            {
                var courseID = NumUtil.ParseInteger(course_search);
                if (courseID.ToString().toCourse() == Course.Thai)
                    question = question.Where(w => w.CourseTh == true);
                else if (courseID.ToString().toCourse() == Course.English)
                    question = question.Where(w => w.CourseEn == true);
            }
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        questions.AddRange(question.Where(w => w.QuestionCode.Contains(text)
                            | w.QuestionTh.Contains(text)
                            | w.QuestionEn.Contains(text)
                            | w.Remark.Contains(text)
                            | w.Keyword.Contains(text)
                        ));
                    }
                }
                questions = questions.Distinct().ToList();
            }
            else
            {
                questions = question.ToList();
            }
            var lists = new List<RptQuestionAnalyze>();
            foreach (var item in questions)
            {
                if (lists.Where(w => w.id == item.ID).FirstOrDefault() != null)
                    continue;

                var numberofuse = _context.TestResultStudentQAnies.Where(w => w.QuestionID == item.ID).Count();
                decimal? p = null;
                var ptext = "";
                var compare = "";
                if (numberofuse >= 100)
                {
                    var corrected = _context.TestResultStudentQAnies.Where(w => w.QuestionID == item.ID & w.Point > 0).Count();
                    p = corrected / numberofuse;
                    if (p >= 0 & p < 0.2M)
                        ptext = "ยากมาก";
                    else if (p >= 0.2M & p < 0.4M)
                        ptext = "ยาก";
                    else if (p >= 0.4M & p < 0.6M)
                        ptext = "ปานกลาง";
                    else if (p >= 0.6M & p < 0.8M)
                        ptext = "ง่าย";
                    else if (p >= 0.8M & p <= 1M)
                        ptext = "ง่ายมาก";

                    if (item.QuestionLevel.toQuestionLevelName() == ptext)
                        compare = "สอดคล้อง";
                    else
                        compare = "ไม่สอดคล้อง";
                }

                lists.Add(new RptQuestionAnalyze
                {
                    id = item.ID,
                    questioncode = item.QuestionCode,
                    questionlevel = item.QuestionLevel.toQuestionLevelName(),
                    questionth = item.QuestionTh,
                    questionen = item.QuestionEn,
                    numberofuse = numberofuse,
                    p = p,
                    ptext = ptext,
                    compare = compare,
                });
            }
            return lists.OrderBy(o => o.id).ToList();
        }

        public static List<RptExamAnswer> examanswer(TuExamContext _context, int? examid)
        {
            var lists = new List<RptExamAnswer>();
            var tresult = _context.TestResults
                .Include(i => i.Exam)
                .Where(w => w.ExamID == examid).FirstOrDefault();

            var tstudents = _context.TestResultStudents.Include(i => i.Student).Where(w => w.TestResultID == tresult.ID).OrderBy(o => o.Student.StudentCode);
            foreach (var tstudent in tstudents)
            {
                var examanswer = new RptExamAnswer();
                examanswer.StudentCode = tstudent.Student.StudentCode;
                examanswer.Answers = new List<int>();

                var tquestions = _context.TestResultStudentQAnies.Include(i => i.Question).Include(i => i.QuestionAns).Where(w => w.TestResultStudentID == tstudent.ID).OrderBy(o => o.Index);
                foreach (var tquestion in tquestions)
                {
                    if (tquestion.Question.QuestionType == QuestionType.MultipleChoice)
                    {
                        if (tquestion.QuestionAns != null)
                            examanswer.Answers.Add(tquestion.QuestionAns.Order);
                        else
                            examanswer.Answers.Add(0);
                    }
                    else if (tquestion.Question.QuestionType == QuestionType.Attitude)
                        examanswer.Answers.Add(tquestion.QuestionAnsAttitudeID.HasValue ? tquestion.QuestionAnsAttitudeID.Value : 0);
                    else
                        examanswer.Answers.Add(0);
                }
                lists.Add(examanswer);
            }

            return lists;

        }

    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        public TuExamContext _context;
        public SystemConf _smtp;

        public ReportController(ILogger<ReportController> logger, TuExamContext context, IOptions<SystemConf> smtp)
        {
            this._logger = logger;
            this._context = context;
            this._smtp = smtp.Value;
        }


        [HttpGet]
        [Route("exambydate")]
        public object exambydate(string from_search, string to_search, int? group_search, int pageno = 1)
        {
            if (!group_search.HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!DateUtil.ToDate(from_search).HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!DateUtil.ToDate(to_search).HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);

            var lists = Rpt.exambydate(_context, subjects, from_search, to_search, group_search);

            int skipRows = (pageno - 1) * 31;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / 31;
            if (itemcnt % 31 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(exambydate), new
            {
                data = lists.Skip(skipRows).Take(31).ToArray(),
                subject = subjects.Select(s => s.Name).ToArray(),
                subjectcnt = subjects.Count(),
                pagelen = pagelen
            }); ;

        }

        [HttpGet]
        [Route("exambymonth")]
        public object exambymonth(int? from_search, int? to_search, int? group_search, int pageno = 1)
        {
            if (!group_search.HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!from_search.HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!to_search.HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);
            var lists = Rpt.exambymonth(_context, subjects, from_search, to_search, group_search);

            int skipRows = (pageno - 1) * 12;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / 12;
            if (itemcnt % 12 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(exambymonth), new
            {
                data = lists.Skip(skipRows).Take(12).ToArray(),
                subject = subjects.Select(s => s.Name).ToArray(),
                subjectcnt = subjects.Count(),
                pagelen = pagelen
            }); ;

        }

        [HttpGet]
        [Route("examstudentall")]
        public object examstudentall(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, string period_search, int pageno = 1)
        {
            var lists = Rpt.examstudentall(_context, text_search, status_search, group_search, subject_search, from_search, to_search, period_search);

            int skipRows = (pageno - 1) * 100;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            return CreatedAtAction(nameof(examstudentall), new
            {
                data = lists.Skip(skipRows).Take(100).ToArray(),
                pagelen = pagelen
            });

        }

        [HttpGet]
        [Route("examstudent")]
        public object examstudent(string text_search, string from_search, string to_search, int pageno = 1)
        {
            var lists = Rpt.examstudent(_context, text_search, from_search, to_search);

            int skipRows = (pageno - 1) * 100;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            return CreatedAtAction(nameof(examstudent), new
            {
                data = lists.Skip(skipRows).Take(100).ToArray(),
                pagelen = pagelen
            }); ;

        }

        [HttpGet]
        [Route("examstudentform")]
        public object examstudentform(string student_search, string greats, int pageno = 1)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return CreatedAtAction(nameof(examstudentall), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var student = _context.Students.Include(i => i.Faculty).Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(examstudentall), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var list = Rpt.examstudentform(_context, student, student_search, greats);

            int skipRows = (pageno - 1) * 100;
            var itemcnt = list.data.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            list.pagelen = pagelen;

            return CreatedAtAction(nameof(examstudentform), list);

        }

        [HttpGet]
        [Route("examstudentformbest")]
        public object examstudentformbest(string student_search, int pageno = 1)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return CreatedAtAction(nameof(examstudentformbest), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var student = _context.Students.Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(examstudentformbest), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var list = Rpt.examstudentformbest(_context, student, student_search);


            int skipRows = (pageno - 1) * 100;
            var itemcnt = list.data.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            list.pagelen = pagelen;

            return CreatedAtAction(nameof(examstudentformbest), list);
        }


        [HttpGet]
        [Route("questionlevel")]
        public object questionlevel()
        {
            var lists = Rpt.questionlevel(_context);
            return CreatedAtAction(nameof(examstudentall), new
            {
                data = lists.ToArray(),
                pagelen = 0
            });
        }

        [HttpGet]
        [Route("questionanalyze")]
        public object questionanalyze(string text_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search, int pageno = 1)
        {
            var lists = Rpt.questionanalyze(_context, text_search, status_search, group_search, subject_search, sub_search, level_search, course_search);

            int skipRows = (pageno - 1) * 100;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            return CreatedAtAction(nameof(examstudentall), new
            {
                data = lists.Skip(skipRows).Take(100).ToArray(),
                pagelen = pagelen
            });

        }

        [HttpGet]
        [Route("examanswer")]
        public object examanswer(int? examid)
        {
            var lists = Rpt.examanswer(_context, examid);
            return CreatedAtAction(nameof(examanswer), new
            {
                data = lists.ToArray(),
            }); ;

        }
        public string sendNotificationEmail(string to, string header, string message)
        {
            var msg = new System.Text.StringBuilder();
            try
            {
                var SMTP_SERVER = _smtp.SMTP_SERVER;
                var SMTP_PORT = _smtp.SMTP_PORT;
                var SMTP_USERNAME = _smtp.SMTP_USERNAME;
                var SMTP_PASSWORD = _smtp.SMTP_PASSWORD;
                var SMTP_FROM = _smtp.SMTP_FROM;
                bool STMP_SSL = _smtp.STMP_SSL;

                SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
                System.Net.NetworkCredential cred = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = STMP_SSL;

                var mail = new MailMessage(SMTP_FROM, to, header, message);
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                smtpClient.Credentials = cred;
                smtpClient.Send(mail);

                return msg.ToString();
            }
            catch (Exception ex)
            {
                msg.AppendLine(" EXCEPTION: " + ex.Message);
            }
            return msg.ToString();
        }

        [HttpGet]
        [Route("mailtest")]
        public object mailtest(string email)
        {
            var msg = sendNotificationEmail("voranun.datasource@gmail.com", "ทดสอบส่ง email", "ทดสอบส่ง email");
            return CreatedAtAction(nameof(mailtest), new { Msg = msg });
        }
        //public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //    {
        //        viewName = this.ControllerContext.ActionDescriptor.ActionName;
        //    }

        //    this.ViewData.Model = model;

        //    using (var writer = new StringWriter())
        //    {
        //        IViewEngine viewEngine = this.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        //        ViewEngineResult viewResult = viewEngine.FindView(this.ControllerContext, viewName, !partial);

        //        if (viewResult.Success == false)
        //        {
        //            return $"A view with the name {viewName} could not be found";
        //        }

        //        ViewContext viewContext = new ViewContext(
        //            this.ControllerContext,
        //            viewResult.View,
        //            this.ViewData,
        //            this.TempData,
        //            writer,
        //            new HtmlHelperOptions()
        //        );

        //        await viewResult.View.RenderAsync(viewContext);

        //        return writer.GetStringBuilder().ToString();
        //    }


        //}
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ExcelReportController : ControllerBase
    {
        private readonly ILogger<ExcelReportController> _logger;
        public TuExamContext _context;

        public ExcelReportController(ILogger<ExcelReportController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }


        [HttpGet]
        [Route("exambydate")]

        public async Task<object> exambydate(string from_search, string to_search, int? group_search)
        {

            var lists = new List<RptExamByDate>();
            if (!group_search.HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!DateUtil.ToDate(from_search).HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!DateUtil.ToDate(to_search).HasValue)
                return CreatedAtAction(nameof(exambydate), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);

            lists = Rpt.exambydate(_context, subjects, from_search, to_search, group_search);

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\exambydate" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "วันที่";
                col++;
                foreach (var subject in subjects)
                {
                    worksheet.Cells[row, col].Value = subject.Name;
                    col++;
                }
                worksheet.Cells[row, col].Value = "ลงทะเบียน"; col++;
                worksheet.Cells[row, col].Value = "Walk-In"; col++;
                worksheet.Cells[row, col].Value = "รวม"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.examdate; col++;
                    foreach (var subject in item.studentbysubject)
                    {
                        worksheet.Cells[row, col].Value = subject;
                        col++;
                    }
                    worksheet.Cells[row, col].Value = item.studentadvance; col++;
                    worksheet.Cells[row, col].Value = item.studentwalkin; col++;
                    worksheet.Cells[row, col].Value = item.total; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(exambydate), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("exambymonth")]
        public async Task<object> exambymonth(int? from_search, int? to_search, int? group_search)
        {
            var lists = new List<RptExamByYear>();
            if (!group_search.HasValue)
                return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!from_search.HasValue)
                return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (!to_search.HasValue)
                return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);
            lists = Rpt.exambymonth(_context, subjects, from_search, to_search, group_search);

            lists = lists.OrderByDescending(o => o.year).ThenBy(o => o.month).ToList();

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\exambymonth" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "ปี"; col++;
                worksheet.Cells[row, col].Value = "เดือน"; col++;

                foreach (var subject in subjects)
                {
                    worksheet.Cells[row, col].Value = subject.Name;
                    col++;
                }
                worksheet.Cells[row, col].Value = "ลงทะเบียน"; col++;
                worksheet.Cells[row, col].Value = "Walk-In"; col++;
                worksheet.Cells[row, col].Value = "รวม"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.year; col++;
                    worksheet.Cells[row, col].Value = item.fullmonth; col++;
                    foreach (var subject in item.studentbysubject)
                    {
                        worksheet.Cells[row, col].Value = subject;
                        col++;
                    }
                    worksheet.Cells[row, col].Value = item.studentadvance; col++;
                    worksheet.Cells[row, col].Value = item.studentwalkin; col++;
                    worksheet.Cells[row, col].Value = item.total; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }

        [HttpGet]
        [Route("examstudentall")]
        public async Task<object> examstudentall(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, string period_search)
        {
            var lists = new List<RptExamStudent>();

            lists = Rpt.examstudentall(_context, text_search, status_search, group_search, subject_search, from_search, to_search, period_search);

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examstudentall" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "วันที่สอบ"; col++;
                worksheet.Cells[row, col].Value = "รอบ"; col++;
                worksheet.Cells[row, col].Value = "รหัสนักศึกษา"; col++;
                worksheet.Cells[row, col].Value = "ชื่อ-นามสกุล"; col++;
                worksheet.Cells[row, col].Value = "วิชา"; col++;
                worksheet.Cells[row, col].Value = "ช่องทางลงทะเบียน"; col++;
                worksheet.Cells[row, col].Value = "แบบทดสอบ"; col++;
                worksheet.Cells[row, col].Value = "สถานะ"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.examdate; col++;
                    worksheet.Cells[row, col].Value = item.examperiod; col++;
                    worksheet.Cells[row, col].Value = item.studentcode; col++;
                    worksheet.Cells[row, col].Value = item.prefix + " " + item.firstname + " " + item.lastname; col++;
                    worksheet.Cells[row, col].Value = item.subject; col++;
                    worksheet.Cells[row, col].Value = item.examregistertype; col++;
                    worksheet.Cells[row, col].Value = item.test; col++;
                    worksheet.Cells[row, col].Value = item.examstatus; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(examstudentall), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }

        [HttpGet]
        [Route("examstudent")]
        public async Task<object> examstudent(string text_search, string from_search, string to_search)
        {
            var lists = Rpt.examstudent(_context, text_search, from_search, to_search);


            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examstudent" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "รหัสนักศึกษา"; col++;
                worksheet.Cells[row, col].Value = "ชื่อ-นามสกุล"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.studentcode; col++;
                    worksheet.Cells[row, col].Value = item.prefix + " " + item.firstname + " " + item.lastname; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(examstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("examstudentform")]
        public async Task<object> examstudentform(string student_search, string greats)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return CreatedAtAction(nameof(examstudentform), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var student = _context.Students.Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(examstudentform), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var list = Rpt.examstudentform(_context, student, student_search, greats);
            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examstudentform" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = list.prefix + " " + list.firstname + " " + list.lastname; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = "รหัส " + list.studentcode; col++;
                row++;
                col = 1;
                worksheet.Cells[row, col].Value = "คณะ "; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = "หลักสูตร " + list.course; col++;
                row++;
                row++;
                col = 1;
                worksheet.Cells[row, col].Value = "กลุ่มวิชา"; col++;
                worksheet.Cells[row, col].Value = "วิชา"; col++;
                worksheet.Cells[row, col].Value = "วันที่สอบ"; col++;
                worksheet.Cells[row, col].Value = "เวลา"; col++;
                worksheet.Cells[row, col].Value = "ชื่อแบบทดสอบ"; col++;
                worksheet.Cells[row, col].Value = "คะแนน"; col++;
                row++;
                col = 1;
                foreach (var item in list.data)
                {
                    worksheet.Cells[row, col].Value = item.group; col++;
                    worksheet.Cells[row, col].Value = item.subject; col++;
                    worksheet.Cells[row, col].Value = item.examdate; col++;
                    worksheet.Cells[row, col].Value = item.starton; col++;
                    worksheet.Cells[row, col].Value = item.test; col++;
                    worksheet.Cells[row, col].Value = item.point; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(examstudentform), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("examstudentformbest")]
        public async Task<object> examstudentformbest(string student_search)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return CreatedAtAction(nameof(examstudentformbest), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var student = _context.Students.Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(examstudentformbest), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var list = Rpt.examstudentformbest(_context, student, student_search);
            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examstudentformbest" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = list.prefix + " " + list.firstname + " " + list.lastname; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = "รหัส " + list.studentcode; col++;
                row++;
                col = 1;
                worksheet.Cells[row, col].Value = "คณะ "; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = ""; col++;
                worksheet.Cells[row, col].Value = "หลักสูตร " + list.course; col++;
                row++;
                row++;
                col = 1;
                worksheet.Cells[row, col].Value = "กลุ่มวิชา"; col++;
                worksheet.Cells[row, col].Value = "วิชา"; col++;
                worksheet.Cells[row, col].Value = "วันที่สอบ"; col++;
                worksheet.Cells[row, col].Value = "วันที่สอบ"; col++;
                worksheet.Cells[row, col].Value = "ชื่อแบบทดสอบ"; col++;
                row++;
                col = 1;
                foreach (var item in list.data)
                {
                    worksheet.Cells[row, col].Value = item.group; col++;
                    worksheet.Cells[row, col].Value = item.subject; col++;
                    worksheet.Cells[row, col].Value = item.examdate; col++;
                    worksheet.Cells[row, col].Value = item.starton; col++;
                    worksheet.Cells[row, col].Value = item.test; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(examstudentformbest), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("questionanalyze")]
        public async Task<object> questionanalyze(string text_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search)
        {
            var lists = Rpt.questionanalyze(_context, text_search, status_search, group_search, subject_search, sub_search, level_search, course_search);

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\questionanalyze" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "ข้อสอบ"; col++;
                worksheet.Cells[row, col].Value = "จำนวนครั้งที่ใช้งาน"; col++;
                worksheet.Cells[row, col].Value = "ระดับความยาก (ที่กำหนดไว้)"; col++;
                worksheet.Cells[row, col].Value = "ค่าความยาก (ที่ระบบคำนวณ)"; col++;
                worksheet.Cells[row, col].Value = "การแปลงผล (ที่ระบบคำนวณ)"; col++;
                worksheet.Cells[row, col].Value = "ผลการเปรียบเทียบ"; col++;
                worksheet.Cells[row, col].Value = "อำนาจการจำแนก"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.questioncode; col++;
                    worksheet.Cells[row, col].Value = item.numberofuse; col++;
                    worksheet.Cells[row, col].Value = item.questionlevel; col++;
                    worksheet.Cells[row, col].Value = item.p; col++;
                    worksheet.Cells[row, col].Value = item.ptext; col++;
                    worksheet.Cells[row, col].Value = item.compare; col++;
                    worksheet.Cells[row, col].Value = item.r; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(questionanalyze), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }

        [HttpGet]
        [Route("questionlevel")]
        public async Task<object> questionlevel()
        {
            var lists = Rpt.questionlevel(_context);

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\questionlevel" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "ประเภทข้อสอบ"; col++;
                worksheet.Cells[row, col].Value = "ง่ายมาก"; col++;
                worksheet.Cells[row, col].Value = "ง่าย"; col++;
                worksheet.Cells[row, col].Value = "ปานกลาง"; col++;
                worksheet.Cells[row, col].Value = "ยาก"; col++;
                worksheet.Cells[row, col].Value = "ยากมาก"; col++;
                worksheet.Cells[row, col].Value = "รวม"; col++;
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.questiontype; col++;
                    worksheet.Cells[row, col].Value = item.vereasy; col++;
                    worksheet.Cells[row, col].Value = item.easy; col++;
                    worksheet.Cells[row, col].Value = item.mid; col++;
                    worksheet.Cells[row, col].Value = item.hard; col++;
                    worksheet.Cells[row, col].Value = item.veryhard; col++;
                    worksheet.Cells[row, col].Value = item.total; col++;
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(questionlevel), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }

        [HttpGet]
        [Route("examanswer")]
        public async Task<object> examanswer(int? examid)
        {
            var lists = new List<RptExamAnswer>();

            lists = Rpt.examanswer(_context, examid);

            var date = DateUtil.ToInternalDate3(DateUtil.Now());
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examanswer" + date + ".xlsx";

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = "รหัสนักศึกษา"; col++;
                if (lists != null && lists.Count > 0)
                {
                    var questioncnt = lists[0].Answers.Count();
                    for (var i = 0; i < questioncnt; i++)
                    {
                        worksheet.Cells[row, col].Value = i + 1; col++;
                    }
                }
                row++;
                col = 1;
                foreach (var item in lists)
                {
                    worksheet.Cells[row, col].Value = item.StudentCode; col++;
                    foreach (var answer in item.Answers)
                    {
                        worksheet.Cells[row, col].Value = answer;
                        col++;
                    }
                    row++;
                    col = 1;
                }

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }


        [HttpGet]
        [Route("examgreatsexcel")]
        public async Task<object> examgreatsexcel(string from_search, string to_search)
        {
            if (string.IsNullOrEmpty(from_search))
                return CreatedAtAction(nameof(examgreatsexcel), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
            if (string.IsNullOrEmpty(to_search))
                return CreatedAtAction(nameof(examgreatsexcel), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var dfrom_search = DateUtil.ToDate(from_search);
            var dto_search = DateUtil.ToDate(to_search);
            var lists = _context.TestResultStudents
                .Include(i => i.Exam)
                .Include(i => i.Student)
                .Where(w => w.Exam.ExamDate.Value.Date >= dfrom_search.Value.Date & w.Exam.ExamDate.Value.Date <= dto_search.Value.Date)
                .OrderBy(o => o.Exam.ExamDate)
                .ThenBy(o => o.Student.Faculty)
                .ThenBy(o => o.Student.StudentCode)
                .Select(s => new { Student = s.Student, ExamDate = s.Exam.ExamDate.Value.Date }).Distinct();

            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\temp\\examgreatsexcel" + DateUtil.ToInternalDate3(dfrom_search) + "_" + DateUtil.ToInternalDate3(dto_search) + ".xlsx";
            using (var package = new ExcelPackage())
            {
                var subjects = _context.Subjects.Where(w => w.SubjectGroup.Name == "GREATS").OrderBy(o => o.Order);

                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var col = 1;
                var row = 1;
                worksheet.Cells[row, col].Value = ""; col++;

                SubjectGSetup setupg = null;
                SubjectRSetup setupr = null;
                SubjectESetup setupe = null;
                SubjectASetup setupa = null;
                SubjectTSetup setupt = null;
                SubjectSSetup setups = null;
                foreach (var subject in subjects)
                {
                    var subs = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID).OrderBy(o => o.Order);
                    worksheet.Cells[row, col].Value = subject.Name;
                    worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (subject.Name == "G")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        setupg = _context.SubjectGSetups.FirstOrDefault();
                    }
                    else if (subject.Name == "R")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        setupr = _context.SubjectRSetups.FirstOrDefault();
                    }
                    else if (subject.Name == "E")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        setupe = _context.SubjectESetups.FirstOrDefault();
                    }
                    else if (subject.Name == "A")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        setupa = _context.SubjectASetups.FirstOrDefault();
                    }
                    else if (subject.Name == "T")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        setupt = _context.SubjectTSetups.FirstOrDefault();
                    }
                    else if (subject.Name == "S")
                    {
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        setups = _context.SubjectSSetups.FirstOrDefault();
                    }
                    worksheet.Cells[row, col, row, (col + subs.Count()) - 1].Merge = true;
                    col += subs.Count();
                }
                worksheet.Cells[row, col].Value = "วันที่สอบ"; col++;
                worksheet.Cells[row, col].Value = "วันที่หมดอายุ"; col++;

                row++;
                col = 1;

                worksheet.Cells[row, col].Value = "รหัสนักศึกษา"; col++;

                foreach (var subject in subjects)
                {
                    var subs = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID).OrderBy(o => o.Order);
                    foreach (var sub in subs)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        worksheet.Cells[row, col].Value = sub.Name; col++;
                    }
                }

                row++;
                col = 1;

                foreach (var list in lists)
                {
                    //if (row == 10)
                    //    break;

                    worksheet.Cells[row, col].Value = list.Student.StudentCode; col++;
                    foreach (var subject in subjects)
                    {
                        var tresultstudent = _context.TestResultStudents
                            .Include(i => i.Exam)
                            .Where(w => w.StudentID == list.Student.ID & w.Exam.SubjectID == subject.ID & w.Exam.ExamDate.Value.Date == list.ExamDate).FirstOrDefault();

                        var tsanswers = _context.TestResultStudentQAnies.Include(i => i.Question).Where(w => w.TestResultStudentID == tresultstudent.ID);
                        if (tsanswers == null || tsanswers.Count() == 0)
                            continue;

                        var type1cnt = 0;
                        var type2cnt = 0;
                        var type3cnt = 0;

                        if (subject.Name == "R")
                        {
                            var i = _context.SubjectSubs.Where(w => w.Name == "I").FirstOrDefault();
                            var s = _context.SubjectSubs.Where(w => w.Name == "S").FirstOrDefault();

                            var icnt = 0;
                            var scnt = 0;
                            var tsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.SubjectSub);
                            foreach (var tsanswer in tsanswersubs)
                            {
                                if (tsanswer.SubjectSubID == i.ID)
                                    icnt++;
                                else if (tsanswer.SubjectSubID == s.ID)
                                    scnt++;
                            }

                            var pointi = 0M;
                            var points = 0M;
                            var pointmaxi = 0M;
                            var pointmaxs = 0M;
                            var itsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == i.ID);
                            foreach (var tsanswer in itsanswersubs)
                            {
                                pointi += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                pointmaxi += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                            }

                            var stsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == s.ID);
                            foreach (var tsanswer in stsanswersubs)
                            {
                                points += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                pointmaxs += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                            }

                            var percenti = pointmaxi > 0 ? (pointi * 100) / pointmaxi : 0;
                            var percents = pointmaxi > 0 ? (points * 100) / pointmaxs : 0;

                            if (percenti >= 70)
                            {
                                worksheet.Cells[row, col].Value = 3; col++;
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value =1; col++;
                            }

                            if (percents >= 70)
                            {
                                worksheet.Cells[row, col].Value = 3; col++;
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value =1; col++;
                            }
                            if (icnt > scnt)
                            {
                                worksheet.Cells[row, col].Value = 1; col++;

                            }
                            else if (scnt > icnt)
                            {
                                worksheet.Cells[row, col].Value = 3; col++;
                            }
                            else
                            {
                                if (percents > percenti)
                                {
                                    worksheet.Cells[row, col].Value = 3; col++;
                                }
                                else
                                {
                                    worksheet.Cells[row, col].Value = 1; col++;
                                }
                            }
                        }
                        else
                        {
                            var subs = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID).OrderBy(o => o.Order);
                            foreach (var sub in subs)
                            {
                                if (subject.Name == "G")
                                {
                                    if (setupg != null)
                                    {
                                        var subtype1cnt = 0;
                                        var subtype2cnt = 0;
                                        var subtype3cnt = 0;
                                        var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);

                                        foreach (var tsanswer in tsanswersubs)
                                        {
                                            if (tsanswer.Point == setupg.Type1Point)
                                                subtype1cnt += 1;
                                            else if (tsanswer.Point == setupg.Type2Point)
                                                subtype2cnt += 1;
                                            else if (tsanswer.Point == setupg.Type3Point)
                                                subtype3cnt += 1;

                                            if (tsanswer.Point == setupg.Type1Point)
                                                type1cnt += 1;
                                            else if (tsanswer.Point == setupg.Type2Point)
                                                type2cnt += 1;
                                            else if (tsanswer.Point == setupg.Type3Point)
                                                type3cnt += 1;
                                        }
                                        var percent = (subtype3cnt * 100) / tsanswersubs.Count();
                                        if (tsanswersubs.Count() > 0 && percent >= setupg.PercentBySubjectSub)
                                        {
                                            worksheet.Cells[row, col].Value = 3; col++;
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, col].Value =1; col++;
                                        }
                                    }
                                }
                                else if (subject.Name == "E")
                                {
                                    if (setupe != null)
                                    {
                                        var point = 0M;
                                        var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                        foreach (var tsanswer in tsanswersubs)
                                        {
                                            point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                        }
                                        var max = tsanswersubs.Count() * setupe.MaxPoint;

                                        var percent = max > 0 ? (point * 100) / max : 0;
                                        if (percent >= setupe.PercentHigh)
                                        {
                                            worksheet.Cells[row, col].Value = 3; col++;
                                        }
                                        else if (percent >= setupe.PercentMid)
                                        {
                                            worksheet.Cells[row, col].Value = 2; col++;
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, col].Value =1; col++;
                                        }
                                    }
                                }
                                else if (subject.Name == "A")
                                {
                                    if (setupa != null)
                                    {
                                        var point = 0M;
                                        var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                        foreach (var tsanswer in tsanswersubs)
                                        {
                                            point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                        }
                                        var max = tsanswersubs.Count() * setupa.MaxPoint;

                                        var percent = max > 0 ? (point * 100) / max : 0;
                                        if (percent > setupa.PercentType3)
                                        {
                                            worksheet.Cells[row, col].Value = 3; col++;
                                        }
                                        else if (percent > setupa.PercentType2)
                                        {
                                            worksheet.Cells[row, col].Value = 2; col++;
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, col].Value =1; col++;
                                        }
                                    }
                                }
                                else if (subject.Name == "T")
                                {
                                    if (setupt != null)
                                    {
                                        var point = 0M;
                                        var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                        foreach (var tsanswer in tsanswersubs)
                                        {
                                            point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                        }
                                        var max = tsanswersubs.Count() * setupt.MaxPoint;

                                        var percent = max > 0 ? (point * 100) / max : 0;
                                        if (percent > setupt.PercentType3)
                                        {
                                            worksheet.Cells[row, col].Value = 3; col++;
                                        }
                                        else if (percent > setupt.PercentType2)
                                        {
                                            worksheet.Cells[row, col].Value = 2; col++;
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, col].Value =1; col++;
                                        }
                                    }
                                }
                                else if (subject.Name == "S")
                                {
                                    if (setups != null)
                                    {
                                        var point0cnt = 0M;
                                        var point1cnt = 0M;
                                        var point2cnt = 0M;
                                        var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                        foreach (var tsanswer in tsanswersubs)
                                        {
                                            var point = tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                            if (point == 0)
                                                point0cnt += 1;
                                            else if (point == 1)
                                                point1cnt += 1;
                                            else if (point == 2)
                                                point2cnt += 1;
                                        }
                                        if (point2cnt >= point1cnt && point2cnt >= point0cnt)
                                        {
                                            worksheet.Cells[row, col].Value = 3; col++;
                                        }
                                        else if (point1cnt >= point0cnt)
                                        {
                                            worksheet.Cells[row, col].Value = 2; col++;
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, col].Value =1; col++;
                                        }
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[row, col].Value = ""; col++;
                                }

                            }
                        }

                    }
                    worksheet.Cells[row, col].Value = DateUtil.ToDisplayDate(list.ExamDate); col++;
                    worksheet.Cells[row, col].Value = DateUtil.ToDisplayDate(list.ExamDate.AddYears(2)); col++;
                    col = 1;
                    row++;
                }
                var modelTable = worksheet.Cells;
                modelTable.AutoFitColumns();
                // calculate
                worksheet.Calculate();

                var filename = filePath;
                filePath = filePath.Replace("\\", "/");
                package.SaveAs(new FileInfo(filePath));
                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "application/vnd.ms-excel";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(exambymonth), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

        }

    }

    [ApiController]
    [Route("api/[controller]")]
    public class PdfReportController : ControllerBase
    {
        private readonly ILogger<PdfReportController> _logger;
        public TuExamContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PdfReportController(ILogger<PdfReportController> logger, TuExamContext context, IHostingEnvironment hostingEnvironment)
        {
            this._logger = logger;
            this._context = context;
            this._hostingEnvironment = hostingEnvironment;

        }


        [HttpGet]
        [Route("exambydate")]

        public void exambydate(string from_search, string to_search, int? group_search)
        {

            var lists = new List<RptExamByDate>();
            if (!group_search.HasValue)
                return;
            if (!DateUtil.ToDate(from_search).HasValue)
                return;
            if (!DateUtil.ToDate(to_search).HasValue)
                return;

            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);

            lists = Rpt.exambydate(_context, subjects, from_search, to_search, group_search);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(subjects.Count() + 4);
            var cell = new PdfPCell(new Phrase(12, "วันที่", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var subject in subjects)
            {
                cell = new PdfPCell(new Phrase(12, subject.Name, font));
                cell.Padding = 5;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Phrase(12, "ลงทะเบียน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "Walk-In", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รวม", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.examdate, font));
                cell.Padding = 5;
                table.AddCell(cell);
                foreach (var subject in item.studentbysubject)
                {
                    cell = new PdfPCell(new Phrase(12, subject.ToString(), font));
                    cell.Padding = 5;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(12, item.studentadvance.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.studentwalkin.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.total.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();
        }

        [HttpGet]
        [Route("exambymonth")]
        public void exambymonth(int? from_search, int? to_search, int? group_search, int pageno = 1)
        {
            var lists = new List<RptExamByYear>();
            if (!group_search.HasValue)
                return;
            if (!from_search.HasValue)
                return;
            if (!to_search.HasValue)
                return;


            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == group_search).OrderBy(o => o.Order);
            lists = Rpt.exambymonth(_context, subjects, from_search, to_search, group_search);

            lists = lists.OrderByDescending(o => o.year).ThenBy(o => o.month).ToList();

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(subjects.Count() + 5);
            var cell = new PdfPCell(new Phrase(12, "ปี", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "เดือน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var subject in subjects)
            {
                cell = new PdfPCell(new Phrase(12, subject.Name, font));
                cell.Padding = 5;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Phrase(12, "ลงทะเบียน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "Walk-In", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รวม", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.year.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.fullmonth, font));
                cell.Padding = 5;
                table.AddCell(cell);
                foreach (var subject in item.studentbysubject)
                {
                    cell = new PdfPCell(new Phrase(12, subject.ToString(), font));
                    cell.Padding = 5;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(12, item.studentadvance.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.studentwalkin.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.total.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;

        }

        [HttpGet]
        [Route("examstudentall")]
        public void examstudentall(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, string period_search, int pageno = 1)
        {
            var lists = new List<RptExamStudent>();

            lists = Rpt.examstudentall(_context, text_search, status_search, group_search, subject_search, from_search, to_search, period_search);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(8);
            var cell = new PdfPCell(new Phrase(12, "วันที่สอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รหัสนักศึกษา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ชื่อ-นามสกุล", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ช่องทางลงทะเบียน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "แบบทดสอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "สถานะ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.examdate, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.examperiod, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.studentcode, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.prefix + " " + item.firstname + " " + item.lastname, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.subject, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.examregistertype, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.test, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.examstatus, font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;

        }

        [HttpGet]
        [Route("examstudent")]
        public void examstudent(string text_search, string from_search, string to_search)
        {
            var lists = Rpt.examstudent(_context, text_search, from_search, to_search);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(2);
            var cell = new PdfPCell(new Phrase(12, "รหัสนักศึกษา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ชื่อ-นามสกุล", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.studentcode, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.prefix + " " + item.firstname + " " + item.lastname, font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;


        }

        [HttpGet]
        [Route("examstudentform")]
        public void examstudentform(string student_search, string greats)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return;

            var student = _context.Students.Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return; ;

            var list = Rpt.examstudentform(_context, student, student_search, greats);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(7);
            var cell = new PdfPCell(new Phrase(12, list.prefix + " " + list.firstname + " " + list.lastname, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รหัส " + list.studentcode, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "คณะ ", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "หลักสูตร " + list.course, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "กลุ่มวิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วันที่สอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "เวลา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "คะแนน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ชื่อแบบทดสอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "คะแนน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in list.data)
            {
                cell = new PdfPCell(new Phrase(12, item.group, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.subject, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.examdate, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.starton, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.percent.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.test, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.point.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;
        }

        [HttpGet]
        [Route("examstudentformbest")]
        public void examstudentformbest(string student_search)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return;

            var student = _context.Students.Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return;

            var list = Rpt.examstudentformbest(_context, student, student_search);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(6);
            var cell = new PdfPCell(new Phrase(12, list.prefix + " " + list.firstname + " " + list.lastname, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รหัส " + list.studentcode, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "คณะ ", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "หลักสูตร " + list.course, font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "กลุ่มวิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วันที่สอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "เวลา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "คะแนน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ชื่อแบบทดสอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in list.data)
            {
                cell = new PdfPCell(new Phrase(12, item.group, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.subject, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.examdate, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.starton, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.percent.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.test, font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();
            return;
        }

        [HttpGet]
        [Route("resultform2")]
        public void resultform2(string search_date)
        {
            if (string.IsNullOrEmpty(search_date))
                search_date = DateUtil.ToDisplayDate(DateUtil.Now());
            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), -80f, -80f, 10f, 0f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var ARIALUNI = BaseFont.CreateFont(webRootPath + @"\fonts\ArialUnicodeMS.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 13);
            Font fontsmall = new Font(THSarabunNew, 10);
            Font fontbig = new Font(THSarabunNew, 14);
            Font fontbigblue = new Font(THSarabunNew, 14, Font.BOLD);
            fontbigblue.Color = new BaseColor(69, 103, 184);
            Font fontHeader = new Font(THSarabunNew, 35, Font.BOLD);
            Font fontbold = new Font(THSarabunNew, 16, Font.BOLD);
            Font fontuni = new Font(ARIALUNI, 25);
            Font fontunismall = new Font(ARIALUNI, 12);
            var totalwidth = pdfDoc.PageSize.Width;

            pdfDoc.Open();

            var date = DateUtil.ToDate(search_date);
            var students = _context.TestResultStudents.Where(w => w.Start_On.Value.Date == date.Value.Date).OrderBy(o => o.Student.Faculty).ThenBy(o => o.Student.StudentCode).Select(s => s.StudentID).Distinct();

            var subjects = _context.Subjects.Where(w => w.SubjectGroup.Name == "GREATS").OrderBy(o => o.Order);
            var firstsubject = subjects.FirstOrDefault();
            if (firstsubject == null)
                return;

            SubjectGSetup setupg = null;
            SubjectESetup setupe = null;
            SubjectASetup setupa = null;
            SubjectTSetup setupt = null;
            SubjectSSetup setups = null;
            foreach (var subject in subjects)
            {
                if (subject.Name == "G")
                    setupg = _context.SubjectGSetups.FirstOrDefault();
                else if (subject.Name == "E")
                    setupe = _context.SubjectESetups.FirstOrDefault();
                else if (subject.Name == "A")
                    setupa = _context.SubjectASetups.FirstOrDefault();
                else if (subject.Name == "T")
                    setupt = _context.SubjectTSetups.FirstOrDefault();
                else if (subject.Name == "S")
                    setups = _context.SubjectSSetups.FirstOrDefault();
            }

            var row = 0;
            foreach (var studentid in students)
            {
                row++;
                var student = _context.Students.Include(i => i.Faculty).Where(w => w.ID == studentid).FirstOrDefault();
                if (student == null)
                    return;

                var tsresultsdefault = _context.TestResultStudents.Where(w => w.StudentID == student.ID & w.Exam.SubjectID == firstsubject.ID);

                tsresultsdefault = tsresultsdefault.OrderByDescending(o => o.Exam.ExamDate);
                var examdate = tsresultsdefault.OrderByDescending(o => o.Exam.ExamDate).Select(s => s.Exam.ExamDate).FirstOrDefault();
                if (examdate == null)
                    return;

                #region start pdf
                PdfPTable table = new PdfPTable(3);

                // Header row
                PdfPTable tableheader = new PdfPTable(8);
                tableheader.SetWidthPercentage(new float[] {
                (float)(0.1 * totalwidth) , //เลขทะเบียนนักศึกษา
                (float)(0.075 * totalwidth) , //เลขทะเบียนนักศึกษา
                (float)(0.04 * totalwidth) , //ชื่อ-สกุล
                (float)(0.135 * totalwidth) , //ชื่อ-สกุล
                (float)(0.03 * totalwidth) , //คณะ
                (float)(0.15 * totalwidth) , // คณะ
                (float)(0.27 * totalwidth) , // GRAETS
                (float)(0.08 * totalwidth) // DATE
            }, pdfDoc.PageSize);
                var cell = new PdfPCell(new Phrase(12, "เลขทะเบียนนักศึกษา:", fontbig));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, student.StudentCode, fontbigblue));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, "ชื่อ-สกุล:", fontbig));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, student.Prefix.toPrefixName() + student.FirstName + " " + student.LastName, fontbigblue));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, "คณะ:", fontbig));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, student.Faculty != null ? student.Faculty.FacultyName : "", fontbigblue));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, "GREATS scores are valid for two years after the test date.", fontbig));
                cell.PaddingBottom = 5;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, DateUtil.ToDisplayDate(examdate), fontbig));
                cell.PaddingBottom = 5;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                tableheader.AddCell(cell);

                cell = new PdfPCell(tableheader);
                cell.Colspan = 3;
                cell.Border = 0;
                table.AddCell(cell);

                // second row
                foreach (var subject in subjects)
                {
                    var subs = _context.SubjectSubs.Where(w => w.Subject.Name == subject.Name).OrderBy(o => o.Order);
                    var subremark = "";
                    var desc = new StringBuilder();
                    var subresult = "";
                    var type1cnt = 0;
                    var type2cnt = 0;
                    var type3cnt = 0;

                    PdfPTable tablesubject = new PdfPTable(subs.Count() + 1);
                    var widths = new List<float>();
                    widths.Add(0.17f * totalwidth);
                    foreach (var sub in subs)
                    {
                        widths.Add((float)(0.83f / subs.Count()) * totalwidth);
                    }
                    tablesubject.SetWidthPercentage(widths.ToArray(), pdfDoc.PageSize);
                    cell = new PdfPCell(new Phrase(subject.Name, fontHeader));
                    cell.Padding = 5;
                    cell.PaddingBottom = 5;
                    cell.PaddingTop = -10;
                    cell.Rowspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.UseAscender = true;
                    tablesubject.AddCell(cell);

                    var hasexam = false;

                    var subsigns = new List<string>();
                    foreach (var sub in subs)
                    {
                        var subsign = "\u25cb";
                        subsigns.Add(subsign);
                    }


                    var tsresults = _context.TestResultStudents.Where(w => w.StudentID == student.ID & w.Exam.SubjectID == subject.ID);
                    tsresults = tsresults.OrderByDescending(o => o.Exam.ExamDate);
                    IQueryable<TestResultStudentQAns> tsanswers = null;
                    if (tsresults.OrderByDescending(o => o.Exam.ExamDate).FirstOrDefault() != null)
                    {
                        tsanswers = _context.TestResultStudentQAnies.Include(i => i.Question).Where(w => w.TestResultStudentID == tsresults.OrderByDescending(o => o.Exam.ExamDate).FirstOrDefault().ID);
                        if (tsanswers != null || tsanswers.Count() > 0)
                            hasexam = true;
                    }

                    if (hasexam == false)
                    {
                        foreach (var sub in subs)
                        {
                            cell = new PdfPCell(new Phrase(sub.Name, fontbold));
                            cell.Padding = 5;
                            cell.PaddingBottom = 5;
                            cell.PaddingTop = 0;
                            cell.FixedHeight = 25f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tablesubject.AddCell(cell);

                            subremark += sub.Name + ": " + sub.Description + " ";
                        }
                        var subindex = 0;
                        foreach (var sub in subs)
                        {
                            cell = new PdfPCell(new Phrase(subsigns[subindex], fontuni));
                            cell.Padding = 5;
                            cell.PaddingBottom = 2;
                            cell.PaddingTop = -2;
                            cell.FixedHeight = 25f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tablesubject.AddCell(cell);
                            subindex++;
                        }
                        cell = new PdfPCell(new Phrase(desc.ToString(), font));
                        cell.Padding = 5;
                        cell.Colspan = 6;
                        cell.BorderWidthBottom = 0;
                        cell.FixedHeight = 172f;
                        tablesubject.AddCell(cell);

                        if (!string.IsNullOrEmpty(subject.Description))
                        {
                            cell = new PdfPCell(new Phrase(subject.Description, font));
                            cell.Padding = 5;
                            cell.PaddingTop = 0;
                            cell.Colspan = 6;
                            cell.PaddingTop = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            tablesubject.AddCell(cell);

                            cell = new PdfPCell(new Phrase(subremark, fontsmall));
                            cell.Padding = 5;
                            cell.Colspan = 6;
                            cell.PaddingTop = 0;
                            cell.BorderWidthTop = 0;
                            cell.FixedHeight = 27f;
                            tablesubject.AddCell(cell);
                        }
                        cell = new PdfPCell(tablesubject);
                        table.AddCell(cell);
                        continue;
                    }

                    foreach (var sub in subs)
                    {
                        cell = new PdfPCell(new Phrase(sub.Name, fontbold));
                        cell.Padding = 5;
                        cell.PaddingBottom = 5;
                        cell.PaddingTop = 0;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);

                        subremark += sub.Name + ": " + sub.Description + " ";
                    }

                    if (subject.Name == "R")
                    {
                        var subindex = 0;
                        var i = _context.SubjectSubs.Where(w => w.Name == "I").FirstOrDefault();
                        var s = _context.SubjectSubs.Where(w => w.Name == "S").FirstOrDefault();

                        var icnt = 0;
                        var scnt = 0;
                        var tsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.SubjectSub);
                        foreach (var tsanswer in tsanswersubs)
                        {
                            if (tsanswer.SubjectSubID == i.ID)
                                icnt++;
                            else if (tsanswer.SubjectSubID == s.ID)
                                scnt++;
                        }

                        var pointi = 0M;
                        var points = 0M;
                        var pointmaxi = 0M;
                        var pointmaxs = 0M;
                        var itsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == i.ID);
                        foreach (var tsanswer in itsanswersubs)
                        {
                            pointi += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                            pointmaxi += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                        }

                        var stsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == s.ID);
                        foreach (var tsanswer in stsanswersubs)
                        {
                            points += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                            pointmaxs += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                        }

                        var percenti = pointmaxi > 0 ? (pointi * 100) / pointmaxi : 0;
                        var percents = pointmaxi > 0 ? (points * 100) / pointmaxs : 0;

                        if (percenti < 70 & percents < 70)
                        {
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == null).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                        else if (percenti > 70 & percents < 70)
                        {
                            if (icnt < scnt)
                            {
                                // R2
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                            else
                            {
                                // R1
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                        }
                        else if (percenti < 70 & percents > 70)
                        {
                            if (icnt < scnt)
                            {
                                // R2
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                            else
                            {
                                // R1
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                        }
                        else if (percenti > 70 & percents > 70)
                        {
                            if (icnt < scnt)
                            {
                                // R2
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                            else
                            {
                                // R1
                                var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                                if (setup != null)
                                    desc.AppendLine("      " + setup.Description);
                            }
                        }

                        if (percenti >= 70)
                        {
                            subsigns[subindex] = "\u25CF"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                        }
                        else
                        {
                            subsigns[subindex] = "\u25cb"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                        }


                        cell.Padding = 5;
                        cell.PaddingBottom = 2;
                        cell.PaddingTop = -2;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);

                        if (percents >= 70)
                        {
                            subsigns[subindex] = "\u25CF"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                        }
                        else
                        {
                            subsigns[subindex] = "\u25cb"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                        }

                        cell.Padding = 5;
                        cell.PaddingBottom = 2;
                        cell.PaddingTop = -2;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);


                        if (icnt > scnt)
                        {
                            subsigns[subindex] = "\u25cb"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white

                        }
                        else if (scnt > icnt)
                        {
                            subsigns[subindex] = "\u25CF"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                        }
                        else
                        {
                            if (percents > percenti)
                            {
                                subsigns[subindex] = "\u25CF"; subindex++;
                                cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                            }
                            else
                            {
                                subsigns[subindex] = "\u25cb"; subindex++;
                                cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                            }
                        }

                        cell.Padding = 5;
                        cell.PaddingBottom = 2;
                        cell.PaddingTop = -2;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);
                    }
                    else
                    {
                        var subindex = 0;
                        foreach (var sub in subs)
                        {
                            if (subject.Name == "G")
                            {
                                if (setupg != null)
                                {
                                    var subtype1cnt = 0;
                                    var subtype2cnt = 0;
                                    var subtype3cnt = 0;
                                    var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);

                                    foreach (var tsanswer in tsanswersubs)
                                    {
                                        if (tsanswer.Point == setupg.Type1Point)
                                            subtype1cnt += 1;
                                        else if (tsanswer.Point == setupg.Type2Point)
                                            subtype2cnt += 1;
                                        else if (tsanswer.Point == setupg.Type3Point)
                                            subtype3cnt += 1;

                                        if (tsanswer.Point == setupg.Type1Point)
                                            type1cnt += 1;
                                        else if (tsanswer.Point == setupg.Type2Point)
                                            type2cnt += 1;
                                        else if (tsanswer.Point == setupg.Type3Point)
                                            type3cnt += 1;
                                    }
                                    var percent = (subtype3cnt * 100) / tsanswersubs.Count();
                                    if (tsanswersubs.Count() > 0 && percent >= setupg.PercentBySubjectSub)
                                    {
                                        subresult += sub.Name + ",";
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                    }
                                    else
                                    {
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                    }

                                    cell.Padding = 5;
                                    cell.PaddingBottom = 2;
                                    cell.PaddingTop = -2;
                                    cell.FixedHeight = 25f;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    tablesubject.AddCell(cell);
                                }
                            }
                            else if (subject.Name == "E")
                            {
                                if (setupe != null)
                                {
                                    var point = 0M;
                                    var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                    foreach (var tsanswer in tsanswersubs)
                                    {
                                        point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                    }
                                    var max = tsanswersubs.Count() * setupe.MaxPoint;

                                    var percent = max > 0 ? (point * 100) / max : 0;
                                    if (percent >= setupe.PercentHigh)
                                    {
                                        subsigns[subindex] = "\u25CF";
                                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                    }
                                    else if (percent >= setupe.PercentMid)
                                    {
                                        subsigns[subindex] = "\u25d0";
                                        cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                    }
                                    else
                                    {
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                    }


                                    if (percent >= setupe.PercentType3)
                                        desc.AppendLine("      " + setupe.DescriptionType3);
                                    else if (percent >= setupe.PercentType2)
                                        desc.AppendLine("      " + setupe.DescriptionType2);
                                    else
                                        desc.AppendLine("      " + setupe.DescriptionType1);

                                    cell.Padding = 5;
                                    cell.PaddingBottom = 2;
                                    cell.PaddingTop = -2;
                                    cell.FixedHeight = 25f;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    tablesubject.AddCell(cell);
                                }
                            }
                            else if (subject.Name == "A")
                            {
                                if (setupa != null)
                                {
                                    var point = 0M;
                                    var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                    foreach (var tsanswer in tsanswersubs)
                                    {
                                        point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                    }
                                    var max = tsanswersubs.Count() * setupa.MaxPoint;

                                    var percent = max > 0 ? (point * 100) / max : 0;
                                    if (percent > setupa.PercentType3)
                                    {
                                        subsigns[subindex] = "\u25CF";
                                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                    }
                                    else if (percent > setupa.PercentType2)
                                    {
                                        subsigns[subindex] = "\u25d0";
                                        cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                    }
                                    else
                                    {
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                    }

                                    if (percent > setupa.PercentType3)
                                        desc.AppendLine("      " + setupa.DescriptionType3);
                                    else if (percent > setupa.PercentType2)
                                        desc.AppendLine("      " + setupa.DescriptionType2);
                                    else
                                        desc.AppendLine("      " + setupa.DescriptionType1);

                                    cell.Padding = 5;
                                    cell.PaddingBottom = 2;
                                    cell.PaddingTop = -2;
                                    cell.FixedHeight = 25f;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    tablesubject.AddCell(cell);
                                }
                            }
                            else if (subject.Name == "T")
                            {
                                if (setupt != null)
                                {
                                    var point = 0M;
                                    var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                    foreach (var tsanswer in tsanswersubs)
                                    {
                                        point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                    }
                                    var max = tsanswersubs.Count() * setupt.MaxPoint;

                                    var percent = max > 0 ? (point * 100) / max : 0;
                                    if (percent > setupt.PercentType3)
                                    {
                                        subsigns[subindex] = "\u25CF";
                                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                    }
                                    else if (percent > setupt.PercentType2)
                                    {
                                        subsigns[subindex] = "\u25d0";
                                        cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                    }
                                    else
                                    {
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                    }
                                    if (percent > setupt.PercentType3)
                                        desc.AppendLine("      " + setupt.DescriptionType3);
                                    else if (percent > setupt.PercentType2)
                                        desc.AppendLine("      " + setupt.DescriptionType2);
                                    else
                                        desc.AppendLine("      " + setupt.DescriptionType1);

                                    cell.Padding = 5;
                                    cell.PaddingBottom = 2;
                                    cell.PaddingTop = -2;
                                    cell.FixedHeight = 25f;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    tablesubject.AddCell(cell);
                                }
                            }
                            else if (subject.Name == "S")
                            {
                                if (setups != null)
                                {
                                    var point0cnt = 0M;
                                    var point1cnt = 0M;
                                    var point2cnt = 0M;
                                    var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                    foreach (var tsanswer in tsanswersubs)
                                    {
                                        var point = tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                        if (point == 0)
                                            point0cnt += 1;
                                        else if (point == 1)
                                            point1cnt += 1;
                                        else if (point == 2)
                                            point2cnt += 1;
                                    }
                                    if (point2cnt >= point1cnt && point2cnt >= point0cnt)
                                    {
                                        subsigns[subindex] = "\u25CF";
                                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                        desc.AppendLine("      " + setups.DescriptionType3);
                                    }
                                    else if (point1cnt >= point0cnt)
                                    {
                                        subsigns[subindex] = "\u25d0";
                                        cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                        desc.AppendLine("      " + setups.DescriptionType2);
                                    }
                                    else
                                    {
                                        subsigns[subindex] = "\u25cb";
                                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                        desc.AppendLine("      " + setups.DescriptionType1);
                                    }

                                    cell.Padding = 5;
                                    cell.PaddingBottom = 2;
                                    cell.PaddingTop = -2;
                                    cell.FixedHeight = 25f;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    tablesubject.AddCell(cell);
                                }
                            }
                            else
                            {
                                subsigns[subindex] = "\u25d0";
                                cell = new PdfPCell(new Phrase("\u25d0", fontuni)); // half
                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                            subindex++;
                        }
                    }

                    if (subject.Name == "G")
                    {
                        if ((type3cnt * 100) / tsanswers.Count() >= setupg.PercentByType)
                        {
                            desc.AppendLine("      " + setupg.DescriptionType3);
                            if (!string.IsNullOrEmpty(subresult))
                            {
                                if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                    subresult = subresult.Substring(0, subresult.Length - 1);
                                desc.Append(" และสามารถปฎิบัติตามระเบียบปฎิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                            }
                        }
                        else if (type2cnt > type1cnt)
                        {
                            desc.AppendLine("      " + setupg.DescriptionType2);
                            if (!string.IsNullOrEmpty(subresult))
                            {
                                if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                    subresult = subresult.Substring(0, (subresult.Length - 1));
                                desc.Append(" อย่างไรก็ตามยอบรับที่จะปฏิบัติตามระเบียบปฏิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                            }
                        }
                        else
                        {
                            desc.AppendLine("      " + setupg.DescriptionType1);
                            if (!string.IsNullOrEmpty(subresult))
                            {
                                if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                    subresult = subresult.Substring(0, subresult.Length - 1);
                                desc.Append(" อย่างไรก็ตามยอบรับที่จะปฏิบัติตามระเบียบปฏิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                            }
                        }
                    }

                    cell = new PdfPCell(new Phrase(desc.ToString(), font));
                    cell.Padding = 5;
                    cell.Colspan = 6;
                    cell.BorderWidthBottom = 0;
                    cell.FixedHeight = 172f;
                    tablesubject.AddCell(cell);

                    if (!string.IsNullOrEmpty(subject.Description))
                    {
                        cell = new PdfPCell(new Phrase(subject.Description, font));
                        cell.Padding = 5;
                        cell.PaddingTop = 0;
                        cell.Colspan = 6;
                        cell.PaddingTop = 0;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        tablesubject.AddCell(cell);

                        cell = new PdfPCell(new Phrase(subremark, fontsmall));
                        cell.Padding = 5;
                        cell.Colspan = 6;
                        cell.PaddingTop = 0;
                        cell.BorderWidthTop = 0;
                        cell.FixedHeight = 27f;
                        tablesubject.AddCell(cell);
                    }


                    cell = new PdfPCell(tablesubject);
                    table.AddCell(cell);
                }

                PdfPTable tablefooter = new PdfPTable(8);
                tablefooter.SetWidthPercentage(new float[] {
                (float)(0.07 * totalwidth) , //Symbol definition
                (float)(0.005 * totalwidth) , //u25d0
                (float)(0.1 * totalwidth) , //= ระดับสมรรถณะ 1 (Level 1)
                (float)(0.005 * totalwidth) , //u25d0
                (float)(0.1 * totalwidth) , //= ระดับสมรรถณะ 2 (Level 2)
                (float)(0.005 * totalwidth) , // u25d0
                (float)(0.1 * totalwidth) , // = ระดับสมรรถณะ 3 (Level 3)
                (float)(0.4 * totalwidth) // 
            }, pdfDoc.PageSize);

                cell = new PdfPCell(new Phrase(12, "Symbol definition:", fontsmall));
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase("\u25cb", fontunismall)); // half
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 1 (Level 1)", fontsmall));
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase("\u25d0", fontunismall)); // half
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 2 (Level 2)", fontsmall));
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase("\u25CF", fontunismall)); // half
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 3 (Level 3)", fontsmall));
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(new Phrase(12, "", fontsmall));
                cell.PaddingBottom = 0;
                cell.Border = 0;
                tablefooter.AddCell(cell);

                cell = new PdfPCell(tablefooter);
                cell.Colspan = 3;
                cell.Border = 0;
                table.AddCell(cell);

                pdfDoc.Add(table);

                #endregion
            }

            pdfDoc.Close();


            return;
        }

        [HttpGet]
        [Route("resultform")]
        public void resultform(string student_search, string tsresult)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), -80f, -80f, 10f, 0f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var ARIALUNI = BaseFont.CreateFont(webRootPath + @"\fonts\ArialUnicodeMS.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 13);
            Font fontsmall = new Font(THSarabunNew, 10);
            Font fontbig = new Font(THSarabunNew, 14);
            Font fontbigblue = new Font(THSarabunNew, 14, Font.BOLD);
            fontbigblue.Color = new BaseColor(69, 103, 184);
            Font fontHeader = new Font(THSarabunNew, 35, Font.BOLD);
            Font fontbold = new Font(THSarabunNew, 16, Font.BOLD);
            Font fontuni = new Font(ARIALUNI, 25);
            Font fontunismall = new Font(ARIALUNI, 12);
            var totalwidth = pdfDoc.PageSize.Width;

            var studentid = NumUtil.ParseInteger(student_search);
            if (studentid == 0)
                return;

            String[] tsresultid = null;
            if (!string.IsNullOrEmpty(tsresult))
            {
                tsresultid = tsresult.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            var student = _context.Students.Include(i => i.Faculty).Where(w => w.ID == studentid).FirstOrDefault();
            if (student == null)
                return;

            var subjects = _context.Subjects.Where(w => w.SubjectGroup.Name == "GREATS").OrderBy(o => o.Order);
            var firstsubject = subjects.FirstOrDefault();
            if (firstsubject == null)
                return;
            var tsresultsdefault = _context.TestResultStudents.Where(w => w.StudentID == student.ID & w.Exam.SubjectID == firstsubject.ID);
            if (tsresultid != null && tsresultid.Length > 0)
            {
                tsresultsdefault = tsresultsdefault.Where(w => tsresultid.Contains(w.ID.ToString()));
            }
            tsresultsdefault = tsresultsdefault.OrderByDescending(o => o.Exam.ExamDate);
            var examdate = tsresultsdefault.OrderByDescending(o => o.Exam.ExamDate).Select(s => s.Exam.ExamDate).FirstOrDefault();
            if (examdate == null)
                return;
            pdfDoc.Open();

            #region start pdf
            PdfPTable table = new PdfPTable(3);

            // Header row
            PdfPTable tableheader = new PdfPTable(8);
            tableheader.SetWidthPercentage(new float[] {
                (float)(0.1 * totalwidth) , //เลขทะเบียนนักศึกษา
                (float)(0.075 * totalwidth) , //เลขทะเบียนนักศึกษา
                (float)(0.04 * totalwidth) , //ชื่อ-สกุล
                (float)(0.135 * totalwidth) , //ชื่อ-สกุล
                (float)(0.03 * totalwidth) , //คณะ
                (float)(0.15 * totalwidth) , // คณะ
                (float)(0.27 * totalwidth) , // GRAETS
                (float)(0.08 * totalwidth) // DATE
            }, pdfDoc.PageSize);
            var cell = new PdfPCell(new Phrase(12, "เลขทะเบียนนักศึกษา:", fontbig));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, student.StudentCode, fontbigblue));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "ชื่อ-สกุล:", fontbig));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, student.Prefix.toPrefixName() + student.FirstName + " " + student.LastName, fontbigblue));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "คณะ:", fontbig));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, student.Faculty != null ? student.Faculty.FacultyName : "", fontbigblue));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "GREATS scores are valid for two years after the test date.", fontbig));
            cell.PaddingBottom = 5;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, DateUtil.ToDisplayDate(examdate), fontbig));
            cell.PaddingBottom = 5;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tableheader.AddCell(cell);

            cell = new PdfPCell(tableheader);
            cell.Colspan = 3;
            cell.Border = 0;
            table.AddCell(cell);

            // second row
            foreach (var subject in subjects)
            {
                var subs = _context.SubjectSubs.Where(w => w.Subject.Name == subject.Name).OrderBy(o => o.Order);
                var subremark = "";
                var desc = new StringBuilder();
                var subresult = "";
                SubjectGSetup setupg = _context.SubjectGSetups.FirstOrDefault(); ;
                var type1cnt = 0;
                var type2cnt = 0;
                var type3cnt = 0;

                PdfPTable tablesubject = new PdfPTable(subs.Count() + 1);
                var widths = new List<float>();
                widths.Add(0.17f * totalwidth);
                foreach (var sub in subs)
                {
                    widths.Add((float)(0.83f / subs.Count()) * totalwidth);
                }
                tablesubject.SetWidthPercentage(widths.ToArray(), pdfDoc.PageSize);
                cell = new PdfPCell(new Phrase(subject.Name, fontHeader));
                cell.Padding = 5;
                cell.PaddingBottom = 5;
                cell.PaddingTop = -10;
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.UseAscender = true;
                tablesubject.AddCell(cell);

                var hasexam = false;

                var subsigns = new List<string>();
                foreach (var sub in subs)
                {
                    var subsign = "\u25cb";
                    subsigns.Add(subsign);
                }


                var tsresults = _context.TestResultStudents.Where(w => w.StudentID == student.ID & w.Exam.SubjectID == subject.ID);
                if (tsresultid != null && tsresultid.Length > 0)
                {
                    tsresults = tsresults.Where(w => tsresultid.Contains(w.ID.ToString()));
                }
                tsresults = tsresults.OrderByDescending(o => o.Exam.ExamDate);
                IQueryable<TestResultStudentQAns> tsanswers = null;
                if (tsresults.OrderByDescending(o => o.Exam.ExamDate).FirstOrDefault() != null)
                {
                    tsanswers = _context.TestResultStudentQAnies.Include(i => i.Question).Where(w => w.TestResultStudentID == tsresults.OrderByDescending(o => o.Exam.ExamDate).FirstOrDefault().ID);
                    if (tsanswers != null || tsanswers.Count() > 0)
                        hasexam = true;
                }

                if (hasexam == false)
                {
                    foreach (var sub in subs)
                    {
                        cell = new PdfPCell(new Phrase(sub.Name, fontbold));
                        cell.Padding = 5;
                        cell.PaddingBottom = 5;
                        cell.PaddingTop = 0;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);

                        subremark += sub.Name + ": " + sub.Description + " ";
                    }
                    var subindex = 0;
                    foreach (var sub in subs)
                    {
                        cell = new PdfPCell(new Phrase(subsigns[subindex], fontuni));
                        cell.Padding = 5;
                        cell.PaddingBottom = 2;
                        cell.PaddingTop = -2;
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tablesubject.AddCell(cell);
                        subindex++;
                    }
                    cell = new PdfPCell(new Phrase(desc.ToString(), font));
                    cell.Padding = 5;
                    cell.Colspan = 6;
                    cell.BorderWidthBottom = 0;
                    cell.FixedHeight = 172f;
                    tablesubject.AddCell(cell);

                    if (!string.IsNullOrEmpty(subject.Description))
                    {
                        cell = new PdfPCell(new Phrase(subject.Description, font));
                        cell.Padding = 5;
                        cell.PaddingTop = 0;
                        cell.Colspan = 6;
                        cell.PaddingTop = 0;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        tablesubject.AddCell(cell);

                        cell = new PdfPCell(new Phrase(subremark, fontsmall));
                        cell.Padding = 5;
                        cell.Colspan = 6;
                        cell.PaddingTop = 0;
                        cell.BorderWidthTop = 0;
                        cell.FixedHeight = 27f;
                        tablesubject.AddCell(cell);
                    }
                    cell = new PdfPCell(tablesubject);
                    table.AddCell(cell);
                    continue;
                }

                foreach (var sub in subs)
                {
                    cell = new PdfPCell(new Phrase(sub.Name, fontbold));
                    cell.Padding = 5;
                    cell.PaddingBottom = 5;
                    cell.PaddingTop = 0;
                    cell.FixedHeight = 25f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tablesubject.AddCell(cell);

                    subremark += sub.Name + ": " + sub.Description + " ";
                }

                if (subject.Name == "R")
                {
                    var subindex = 0;
                    var i = _context.SubjectSubs.Where(w => w.Name == "I").FirstOrDefault();
                    var s = _context.SubjectSubs.Where(w => w.Name == "S").FirstOrDefault();

                    var icnt = 0;
                    var scnt = 0;
                    var tsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.SubjectSub);
                    foreach (var tsanswer in tsanswersubs)
                    {
                        if (tsanswer.SubjectSubID == i.ID)
                            icnt++;
                        else if (tsanswer.SubjectSubID == s.ID)
                            scnt++;
                    }

                    var pointi = 0M;
                    var points = 0M;
                    var pointmaxi = 0M;
                    var pointmaxs = 0M;
                    var itsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == i.ID);
                    foreach (var tsanswer in itsanswersubs)
                    {
                        pointi += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                        pointmaxi += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                    }

                    var stsanswersubs = tsanswers.Where(w => w.Question.AnswerType == AnswerType.Point & w.Question.SubjectSubID == s.ID);
                    foreach (var tsanswer in stsanswersubs)
                    {
                        points += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                        pointmaxs += tsanswer.Question.MaxPoint.HasValue ? tsanswer.Question.MaxPoint.Value : 0;
                    }

                    var percenti = pointmaxi > 0 ? (pointi * 100) / pointmaxi : 0;
                    var percents = pointmaxi > 0 ? (points * 100) / pointmaxs : 0;

                    if (percenti < 70 & percents < 70)
                    {
                        var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == null).FirstOrDefault();
                        if (setup != null)
                            desc.AppendLine("      " + setup.Description);
                    }
                    else if (percenti > 70 & percents < 70)
                    {
                        if (icnt < scnt)
                        {
                            // R2
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                        else
                        {
                            // R1
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == false & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                    }
                    else if (percenti < 70 & percents > 70)
                    {
                        if (icnt < scnt)
                        {
                            // R2
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                        else
                        {
                            // R1
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == false & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                    }
                    else if (percenti > 70 & percents > 70)
                    {
                        if (icnt < scnt)
                        {
                            // R2
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == s.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                        else
                        {
                            // R1
                            var setup = _context.SubjectRSetups.Where(w => w.Sub1MoreThanPercent == true & w.Sub2MoreThanPercent == true & w.SubjectSubfromPart1ID == i.ID).FirstOrDefault();
                            if (setup != null)
                                desc.AppendLine("      " + setup.Description);
                        }
                    }
                    if (percenti >= 70)
                    {
                        subsigns[subindex] = "\u25CF"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                    }
                    else
                    {
                        subsigns[subindex] = "\u25cb"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                    }


                    cell.Padding = 5;
                    cell.PaddingBottom = 2;
                    cell.PaddingTop = -2;
                    cell.FixedHeight = 25f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tablesubject.AddCell(cell);

                    if (percents >= 70)
                    {
                        subsigns[subindex] = "\u25CF"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                    }
                    else
                    {
                        subsigns[subindex] = "\u25cb"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                    }

                    cell.Padding = 5;
                    cell.PaddingBottom = 2;
                    cell.PaddingTop = -2;
                    cell.FixedHeight = 25f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tablesubject.AddCell(cell);

                    if (icnt > scnt )
                    {
                        subsigns[subindex] = "\u25cb"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                        
                    }
                    else if (scnt > icnt)
                    {
                        subsigns[subindex] = "\u25CF"; subindex++;
                        cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                    }
                    else
                    {
                        if(percents > percenti)
                        {
                            subsigns[subindex] = "\u25CF"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25CF", fontuni)); // black
                        }
                        else
                        {
                            subsigns[subindex] = "\u25cb"; subindex++;
                            cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                        }
                    }

                    cell.Padding = 5;
                    cell.PaddingBottom = 2;
                    cell.PaddingTop = -2;
                    cell.FixedHeight = 25f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tablesubject.AddCell(cell);
                }
                else
                {
                    var subindex = 0;
                    foreach (var sub in subs)
                    {
                        if (subject.Name == "G")
                        {
                            if (setupg != null)
                            {
                                var subtype1cnt = 0;
                                var subtype2cnt = 0;
                                var subtype3cnt = 0;
                                var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);


                                foreach (var tsanswer in tsanswersubs)
                                {
                                    if (tsanswer.Point == setupg.Type1Point)
                                        subtype1cnt += 1;
                                    else if (tsanswer.Point == setupg.Type2Point)
                                        subtype2cnt += 1;
                                    else if (tsanswer.Point == setupg.Type3Point)
                                        subtype3cnt += 1;

                                    if (tsanswer.Point == setupg.Type1Point)
                                        type1cnt += 1;
                                    else if (tsanswer.Point == setupg.Type2Point)
                                        type2cnt += 1;
                                    else if (tsanswer.Point == setupg.Type3Point)
                                        type3cnt += 1;
                                }
                                var percent = (subtype3cnt * 100) / tsanswersubs.Count();
                                if (tsanswersubs.Count() > 0 && percent >= setupg.PercentBySubjectSub)
                                {
                                    subresult += sub.Name + ",";
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                }
                                else
                                {
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                }

                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                        }
                        else if (subject.Name == "E")
                        {
                            var setup = _context.SubjectESetups.Where(w => w.SubjectSubID == sub.ID).FirstOrDefault();
                            if (setup != null)
                            {
                                var point = 0M;
                                var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                foreach (var tsanswer in tsanswersubs)
                                {
                                    point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                }
                                var max = tsanswersubs.Count() * setup.MaxPoint;

                                var percent = max > 0 ? (point * 100) / max : 0;
                                if (percent >= setup.PercentHigh)
                                {
                                    subsigns[subindex] = "\u25CF";
                                    cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                }
                                else if (percent >= setup.PercentMid)
                                {
                                    subsigns[subindex] = "\u25d0";
                                    cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                }
                                else
                                {
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                }


                                if (percent >= setup.PercentType3)
                                    desc.AppendLine("      " + setup.DescriptionType3);
                                else if (percent >= setup.PercentType2)
                                    desc.AppendLine("      " + setup.DescriptionType2);
                                else
                                    desc.AppendLine("      " + setup.DescriptionType1);

                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                        }
                        else if (subject.Name == "A")
                        {
                            var setup = _context.SubjectASetups.Where(w => w.SubjectSubID == sub.ID).FirstOrDefault();
                            if (setup != null)
                            {
                                var point = 0M;
                                var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                foreach (var tsanswer in tsanswersubs)
                                {
                                    point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                }
                                var max = tsanswersubs.Count() * setup.MaxPoint;

                                var percent = max > 0 ? (point * 100) / max : 0;
                                if (percent > setup.PercentType3)
                                {
                                    subsigns[subindex] = "\u25CF";
                                    cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                }
                                else if (percent > setup.PercentType2)
                                {
                                    subsigns[subindex] = "\u25d0";
                                    cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                }
                                else
                                {
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                }

                                if (percent > setup.PercentType3)
                                    desc.AppendLine("      " + setup.DescriptionType3);
                                else if (percent > setup.PercentType2)
                                    desc.AppendLine("      " + setup.DescriptionType2);
                                else
                                    desc.AppendLine("      " + setup.DescriptionType1);

                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                        }
                        else if (subject.Name == "T")
                        {
                            var setup = _context.SubjectTSetups.Where(w => w.SubjectSubID == sub.ID).FirstOrDefault();
                            if (setup != null)
                            {
                                var point = 0M;
                                var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                foreach (var tsanswer in tsanswersubs)
                                {
                                    point += tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                }
                                var max = tsanswersubs.Count() * setup.MaxPoint;

                                var percent = max > 0 ? (point * 100) / max : 0;
                                if (percent > setup.PercentType3)
                                {
                                    subsigns[subindex] = "\u25CF";
                                    cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                }
                                else if (percent > setup.PercentType2)
                                {
                                    subsigns[subindex] = "\u25d0";
                                    cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                }
                                else
                                {
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                }
                                if (percent > setup.PercentType3)
                                    desc.AppendLine("      " + setup.DescriptionType3);
                                else if (percent > setup.PercentType2)
                                    desc.AppendLine("      " + setup.DescriptionType2);
                                else
                                    desc.AppendLine("      " + setup.DescriptionType1);

                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                        }
                        else if (subject.Name == "S")
                        {
                            var setup = _context.SubjectSSetups.Where(w => w.SubjectSubID == sub.ID).FirstOrDefault();
                            if (setup != null)
                            {
                                var point0cnt = 0M;
                                var point1cnt = 0M;
                                var point2cnt = 0M;
                                var tsanswersubs = tsanswers.Where(w => w.Question.SubjectSubID == sub.ID);
                                foreach (var tsanswer in tsanswersubs)
                                {
                                    var point = tsanswer.Point.HasValue ? tsanswer.Point.Value : 0;
                                    if (point == 0)
                                        point0cnt += 1;
                                    else if (point == 1)
                                        point1cnt += 1;
                                    else if (point == 2)
                                        point2cnt += 1;
                                }
                                if (point2cnt >= point1cnt && point2cnt >= point0cnt)
                                {
                                    subsigns[subindex] = "\u25CF";
                                    cell = new PdfPCell(new Phrase("\u25CF", fontuni)); //black
                                    desc.AppendLine("      " + setup.DescriptionType3);
                                }
                                else if (point1cnt >= point0cnt)
                                {
                                    subsigns[subindex] = "\u25d0";
                                    cell = new PdfPCell(new Phrase("\u25d0", fontuni)); //half
                                    desc.AppendLine("      " + setup.DescriptionType2);
                                }
                                else
                                {
                                    subsigns[subindex] = "\u25cb";
                                    cell = new PdfPCell(new Phrase("\u25cb", fontuni)); // white
                                    desc.AppendLine("      " + setup.DescriptionType1);
                                }

                                cell.Padding = 5;
                                cell.PaddingBottom = 2;
                                cell.PaddingTop = -2;
                                cell.FixedHeight = 25f;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                tablesubject.AddCell(cell);
                            }
                        }
                        else
                        {
                            subsigns[subindex] = "\u25d0";
                            cell = new PdfPCell(new Phrase("\u25d0", fontuni)); // half
                            cell.Padding = 5;
                            cell.PaddingBottom = 2;
                            cell.PaddingTop = -2;
                            cell.FixedHeight = 25f;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tablesubject.AddCell(cell);
                        }
                        subindex++;
                    }
                }

                if (subject.Name == "G")
                {
                    if ((type3cnt * 100) / tsanswers.Count() >= setupg.PercentByType)
                    {
                        desc.AppendLine("      " + setupg.DescriptionType3);
                        if (!string.IsNullOrEmpty(subresult))
                        {
                            if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                subresult = subresult.Substring(0, subresult.Length - 1);
                            desc.Append(" และสามารถปฎิบัติตามระเบียบปฎิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                        }
                    }
                    else if (type2cnt > type1cnt)
                    {
                        desc.AppendLine("      " + setupg.DescriptionType2);
                        if (!string.IsNullOrEmpty(subresult))
                        {
                            if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                subresult = subresult.Substring(0, (subresult.Length - 1));
                            desc.Append(" อย่างไรก็ตามยอบรับที่จะปฏิบัติตามระเบียบปฏิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                        }
                    }
                    else
                    {
                        desc.AppendLine("      " + setupg.DescriptionType1);
                        if (!string.IsNullOrEmpty(subresult))
                        {
                            if (subresult.Substring(subresult.Length - 1, 1) == ",")
                                subresult = subresult.Substring(0, subresult.Length - 1);
                            desc.Append(" อย่างไรก็ตามยอบรับที่จะปฏิบัติตามระเบียบปฏิบัติสากลในด้าน " + subresult + " ได้เป็นอย่างดี");
                        }
                    }
                }

                cell = new PdfPCell(new Phrase(desc.ToString(), font));
                cell.Padding = 5;
                cell.Colspan = 6;
                cell.BorderWidthBottom = 0;
                cell.FixedHeight = 172f;
                tablesubject.AddCell(cell);

                if (!string.IsNullOrEmpty(subject.Description))
                {
                    cell = new PdfPCell(new Phrase(subject.Description, font));
                    cell.Padding = 5;
                    cell.PaddingTop = 0;
                    cell.Colspan = 6;
                    cell.PaddingTop = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    tablesubject.AddCell(cell);

                    cell = new PdfPCell(new Phrase(subremark, fontsmall));
                    cell.Padding = 5;
                    cell.Colspan = 6;
                    cell.PaddingTop = 0;
                    cell.BorderWidthTop = 0;
                    cell.FixedHeight = 27f;
                    tablesubject.AddCell(cell);
                }


                cell = new PdfPCell(tablesubject);
                table.AddCell(cell);
            }

            PdfPTable tablefooter = new PdfPTable(8);
            tablefooter.SetWidthPercentage(new float[] {
                (float)(0.07 * totalwidth) , //Symbol definition
                (float)(0.005 * totalwidth) , //u25d0
                (float)(0.1 * totalwidth) , //= ระดับสมรรถณะ 1 (Level 1)
                (float)(0.005 * totalwidth) , //u25d0
                (float)(0.1 * totalwidth) , //= ระดับสมรรถณะ 2 (Level 2)
                (float)(0.005 * totalwidth) , // u25d0
                (float)(0.1 * totalwidth) , // = ระดับสมรรถณะ 3 (Level 3)
                (float)(0.4 * totalwidth) // 
            }, pdfDoc.PageSize);

            cell = new PdfPCell(new Phrase(12, "Symbol definition:", fontsmall));
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase("\u25cb", fontunismall)); // half
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 1 (Level 1)", fontsmall));
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase("\u25d0", fontunismall)); // half
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 2 (Level 2)", fontsmall));
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase("\u25CF", fontunismall)); // half
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, " = ระดับสมรรถณะ 3 (Level 3)", fontsmall));
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(new Phrase(12, "", fontsmall));
            cell.PaddingBottom = 0;
            cell.Border = 0;
            tablefooter.AddCell(cell);

            cell = new PdfPCell(tablefooter);
            cell.Colspan = 3;
            cell.Border = 0;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion
            pdfDoc.Close();


            return;
        }


        [HttpGet]
        [Route("questionanalyze")]
        public void questionanalyze(string text_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search)
        {
            var lists = Rpt.questionanalyze(_context, text_search, status_search, group_search, subject_search, sub_search, level_search, course_search);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4, 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(7);
            var cell = new PdfPCell(new Phrase(12, "ข้อสอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "จำนวนครั้งที่ใช้งาน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ระดับความยาก (ที่กำหนดไว้)", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ค่าความยาก (ที่ระบบคำนวณ)", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "การแปลงผล (ที่ระบบคำนวณ)", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ผลการเปรียบเทียบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "อำนาจการจำแนก", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.questioncode, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.numberofuse.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.questionlevel, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.p.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.ptext, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.compare, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.r.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;


        }

        [HttpGet]
        [Route("questionlevel")]
        public void questionlevel()
        {
            var lists = Rpt.questionlevel(_context);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4, 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(7);
            var totalwidth = pdfDoc.PageSize.Width - 80f;
            table.SetWidthPercentage(new float[] {
                (float)(0.4 * totalwidth) ,
                (float)(0.1 * totalwidth) ,
                (float)(0.1 * totalwidth) ,
                (float)(0.1 * totalwidth) ,
                (float)(0.1 * totalwidth) ,
                (float)(0.1 * totalwidth) ,
                (float)(0.1 * totalwidth)
            }, pdfDoc.PageSize);
            var cell = new PdfPCell(new Phrase(12, "ประเภทข้อสอบ", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ง่ายมาก", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ง่าย", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ปานกลาง", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ยาก", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ยากมาก", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "รวม", font));
            cell.Padding = 5;
            table.AddCell(cell);
            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.questiontype, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.vereasy.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.easy.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.mid.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.hard.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.veryhard.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, item.total.ToString(), font));
                cell.Padding = 5;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            return;


        }

        [HttpGet]
        [Route("examanswer")]
        public void examanswer(int? examid)
        {
            var lists = new List<RptExamAnswer>();

            lists = Rpt.examanswer(_context, examid);

            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();
            var questioncnt = 0;
            if (lists != null && lists.Count > 0)
                questioncnt = lists[0].Answers.Count();

            PdfPTable table = new PdfPTable(questioncnt + 1);
            var cell = new PdfPCell(new Phrase(12, "รหัสนักศึกษา", font));
            cell.Padding = 5;
            cell.Border = 0;
            table.AddCell(cell);

            for (var i = 0; i < questioncnt; i++)
            {
                cell = new PdfPCell(new Phrase(12, (i + 1).ToString(), font));
                cell.Padding = 5;
                cell.Border = 0;
                table.AddCell(cell);
            }

            foreach (var item in lists)
            {
                cell = new PdfPCell(new Phrase(12, item.StudentCode, font));
                cell.Padding = 5;
                table.AddCell(cell);
                foreach (var answer in item.Answers)
                {
                    cell = new PdfPCell(new Phrase(12, answer.ToString(), font));
                    cell.Padding = 5;
                    table.AddCell(cell);
                }
            }

            pdfDoc.Add(table);

            pdfDoc.Close();
            return;

        }

    }
    public class RptExamStudent
    {
        public string test { get; set; }
        public string group { get; set; }
        public string subject { get; set; }
        public int? subjectorder { get; set; }
        public DateTime? date { get; set; }
        public string examdate { get; set; }
        public string examperiod { get; set; }
        public string examregistertype { get; set; }
        public ExamPeriod examperiodid { get; set; }
        public string examstatus { get; set; }

        public int? studentid { get; set; }
        public string prefix { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string firstnameen { get; set; }
        public string lastnameen { get; set; }
        public string studentcode { get; set; }

    }
    public class RptExamByDate
    {
        public string examdate { get; set; }
        public DateTime date { get; set; }
        public int groupid { get; set; }
        public List<decimal> studentbysubject { get; set; }
        public decimal studentadvance { get; set; }
        public decimal studentwalkin { get; set; }
        public decimal total { get; set; }

    }
    public class RptExamByYear
    {
        public int year { get; set; }
        public int month { get; set; }
        public string fullmonth { get; set; }
        public int groupid { get; set; }
        public List<decimal> studentbysubject { get; set; }
        public decimal studentadvance { get; set; }
        public decimal studentwalkin { get; set; }
        public decimal total { get; set; }

    }
    public class RptExamStudentForm
    {
        public int pagelen { get; set; }
        public string prefix { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string firstnameen { get; set; }
        public string lastnameen { get; set; }
        public string studentcode { get; set; }
        public string course { get; set; }
        public string faculty { get; set; }

        public List<RptExamStudentFormDtl> data { get; set; }

    }
    public class RptExamStudentFormDtl
    {
        public int id { get; set; }
        public string test { get; set; }
        public string group { get; set; }
        public string subject { get; set; }
        public int? subjectorder { get; set; }
        public string examdate { get; set; }
        public string starton { get; set; }
        public DateTime? date { get; set; }

        public int percent { get; set; }
        public decimal point { get; set; }
        public int questioncnt { get; set; }
        public int answeredcnt { get; set; }
        public int correctcnt { get; set; }
        public string examperiod { get; set; }
        public ExamPeriod examperiodid { get; set; }
        public string examstatus { get; set; }
        public string examregistertype { get; set; }
    }

    public class RptQuestionLevel
    {
        public string questiontype { get; set; }
        public decimal vereasy { get; set; }
        public decimal easy { get; set; }
        public decimal mid { get; set; }
        public decimal hard { get; set; }
        public decimal veryhard { get; set; }
        public decimal total { get; set; }

    }

    public class RptQuestionAnalyze
    {
        public int id { get; set; }
        public string questioncode { get; set; }
        public string questionth { get; set; }
        public string questionen { get; set; }
        public int numberofuse { get; set; }
        public string questionlevel { get; set; }
        public decimal? p { get; set; }
        public string ptext { get; set; }
        public string compare { get; set; }
        public decimal? r { get; set; }
        public string rtext { get; set; }

    }

    public class RptExamAnswer
    {
        public string StudentCode { get; set; }
        public List<int> Answers { get; set; }

    }
}
