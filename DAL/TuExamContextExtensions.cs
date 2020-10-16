using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tuexamapi.Models;
using tuexamapi.Util;

namespace tuexamapi.DAL
{
    public static class TuExamContextExtensions
    {
        public static void EnsureSeedData(this TuExamContext context)
        {
            SeedMasterData(context);
            UpdateDatabaseDescriptions(context);
        }

        public static void SeedMasterData(TuExamContext context)
        {
            if (context.SubjectGroups != null && !context.SubjectGroups.Any())
            {
                var group = new SubjectGroup
                {
                    Name = "GREATS",
                    DoExamOrder = true,
                    Color1 = "#8dc63f",
                    Color2 = "#ffba00",
                    Color3 = "#40bbea",
                    Status = StatusType.Active,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectGroups.Add(group);
                context.SaveChanges();
            }
            if (context.Subjects != null && !context.Subjects.Any())
            {
                var greats = context.SubjectGroups.Where(w => w.Name == "GREATS").FirstOrDefault();
                if (greats != null)
                {
                    var subject = new Subject
                    {
                        Name = "G",
                        Description = "Global Mindset",
                        SubjectGroupID = greats.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);

                    subject = new Subject
                    {
                        Name = "R",
                        Description = "Responsibility",
                        SubjectGroupID = greats.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);

                    subject = new Subject
                    {
                        Name = "E",
                        Description = "Eloquence",
                        SubjectGroupID = greats.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);
                    subject = new Subject
                    {
                        Name = "A",
                        Description = "Aesthetic Appre",
                        SubjectGroupID = greats.ID,
                        Order = 4,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);
                    subject = new Subject
                    {
                        Name = "T",
                        Description = "Team Leader",
                        SubjectGroupID = greats.ID,
                        Order = 5,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);
                    subject = new Subject
                    {
                        Name = "S",
                        Description = "Spirit of Thammasat",
                        SubjectGroupID = greats.ID,
                        Order = 6,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.Subjects.Add(subject);
                    context.SaveChanges();
                }
            }
            if (context.SubjectSubs != null && !context.SubjectSubs.Any())
            {
                var g = context.Subjects.Where(w => w.Name == "G").FirstOrDefault();
                if (g != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "H",
                        Description = "Humanity",
                        SubjectID = g.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "Z",
                        Description = "Modernization",
                        SubjectID = g.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "Y",
                        Description = "Diversity",
                        SubjectID = g.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "C",
                        Description = "Cosmopolitanism",
                        SubjectID = g.ID,
                        Order = 4,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "T",
                        Description = "Transnationalism",
                        SubjectID = g.ID,
                        Order = 5,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }

                var r = context.Subjects.Where(w => w.Name == "R").FirstOrDefault();
                if (r != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "I",
                        Description = "Self Responsibility",
                        SubjectID = r.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "S",
                        Description = "Social Responsibility",
                        SubjectID = r.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "O",
                        Description = "Over All",
                        SubjectID = r.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }

                var e = context.Subjects.Where(w => w.Name == "E").FirstOrDefault();
                if (e != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "R",
                        Description = "Logic, Relevance",
                        SubjectID = e.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "B",
                        Description = "Clarity, Brevity",
                        SubjectID = e.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "W",
                        Description = "Delivery robustness, Powerfulness",
                        SubjectID = e.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "E",
                        Description = "Engagement, Rapport building",
                        SubjectID = e.ID,
                        Order = 4,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                    sub = new SubjectSub
                    {
                        Name = "F",
                        Description = "Audience focus",
                        SubjectID = e.ID,
                        Order = 5,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }

                var a = context.Subjects.Where(w => w.Name == "A").FirstOrDefault();
                if (a != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "U",
                        Description = "Understand",
                        SubjectID = a.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "A",
                        Description = "Appreciation",
                        SubjectID = a.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "P",
                        Description = "Apply to life",
                        SubjectID = a.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }

                var t = context.Subjects.Where(w => w.Name == "T").FirstOrDefault();
                if (t != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "L",
                        Description = "Team Leadership Attitude",
                        SubjectID = t.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "M",
                        Description = "Motivation and Collaboration",
                        SubjectID = t.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }

                var s = context.Subjects.Where(w => w.Name == "S").FirstOrDefault();
                if (s != null)
                {
                    var sub = new SubjectSub
                    {
                        Name = "D",
                        Description = "Democracy",
                        SubjectID =s.ID,
                        Order = 1,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "V",
                        Description = "Volunteerims",
                        SubjectID = s.ID,
                        Order = 2,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);

                    sub = new SubjectSub
                    {
                        Name = "G",
                        Description = "Goodness",
                        SubjectID = s.ID,
                        Order = 3,
                        Status = StatusType.Active,
                        Create_On = DateUtil.Now(),
                        Create_By = "system",
                        Update_On = DateUtil.Now(),
                        Update_By = "system"
                    };
                    context.SubjectSubs.Add(sub);
                }
            }
            if (context.SubjectGSetups != null && !context.SubjectGSetups.Any())
            {
                var setup = new SubjectGSetup
                {
                    Type1Point = 1,
                    Type2Point = 2,
                    Type3Point = 3,
                    DescriptionType1 = "มีแนวโน้มที่จะไม่ปฏิบัติตามระเบียบปฏิบัติสากล ระบบธุรกิจ สังคม และวัฒนธรรมที่หลากหลาย เพราะขาดความเข้าใจในเรื่องดังกล่าว มีแนวโน้มจะเลือกอยู่อาศัยและทำงานในสภาพแวดล้อมที่มีความเปลี่ยน แปลงและหลากหลายน้อย",
                    DescriptionType2 = "แม้จะมีความรู้ความเข้าใจในระเบียบปฏิบัติสากล ระบบธุรกิจ สังคม และวัฒนธรรมที่หลากหลาย แต่มีแนวโน้มจะไม่ปฏิบัติตามระเบียบปฏิบัติ สากล โดยใช้เกณฑ์ความเหมาะสมของสถานการณ์และความจำเป็น ของตนเองในการตัดสินใจ",
                    DescriptionType3 = "มีความรู้ความเข้าใจในระเบียบปฏิบัติสากล ระบบธุรกิจ สังคม และวัฒนธรรมที่หลายหลายและมีแนวโน้มจะปฏิบัติตามระเบียบ ปฏิบัติสากล มีความพร้อมที่จะอยู่อาศัยและทำงานในสภาพแวดล้อมที่ เปลี่ยนแปลงและหลากหลายได้",
                    PercentByType = 50,
                    PercentBySubjectSub = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectGSetups.Add(setup);
                context.SaveChanges();
            }
            if (context.SubjectRSetups != null && !context.SubjectRSetups.Any())
            {
                var i = context.SubjectSubs.Where(w => w.Name == "I").FirstOrDefault();
                var s = context.SubjectSubs.Where(w => w.Name == "S").FirstOrDefault();

                // i < 70 s < 70
                var setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความตระหนักต่อหน้าที่ตนเองในเบื้องต้น ยังไม่สามารถจัดการกับหน้าที่ ภาระงาน และความรับผิดชอบที่มีอยู่ได้อย่างเต็มที่ ซึ่งเป็นอุปสรรคต่อการคำนึงถึงประโยชน์ส่วนรวม จำเป็นจะต้องพัฒนามุมมองข้างต้นด้านความรับผิดชอบ ควบคู่กับทักษะที่จำเป็นในการจัดการกับหน้าที่และภาระงานที่มีอยู่ใน ปัจจุบันเป็นอันดับแรกก่อนที่จะพัฒนาจิตสาธารณะและจิตสำนึก ต่อสังคมในขั้นต่อไป",
                    Sub1MoreThanPercent = false,
                    Sub2MoreThanPercent = false,
                    SubjectSubfromPart1ID = null,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i > 70 s < 70 r1
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความรับผิดชอบต่อตนเองอย่างชัดเจน ตระหนักรู้หน้าที่ของตนเอง สามารถเลือกแนวทางในการจัดการกับสถานการณ์และปัญหาตามบทบาท หน้าที่ที่ตัวเองรับผิดชอบทั้งนี้ยังขาดการคำนึงต่อผลกระทบในวงกว้าง ที่อาจเกิดขึ้นกับสังคม ",
                    Sub1MoreThanPercent = true,
                    Sub2MoreThanPercent = false,
                    SubjectSubfromPart1ID = i.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i > 70 s < 70 r2
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความรับผิดชอบต่อตนเองอย่าง ตระหนักรู้หน้าที่ของตนเอง สามารถเลือกแนวทางในการจัดการกับสถานการณ์และปัญหาตามบทบาทหน้าที่ที่ตัวเองรับผิดชอบ เริ่มมีการคำนึงถึงผลประโยชน์ของสังคมในบางสถานการณ์ มีความสับสนเมื่อต้องตัดสินใจเลือกระหว่างหน้าที่ความรับผิดชอบของตนเองหรือประโยชน์ของสังคม",
                    Sub1MoreThanPercent = true,
                    Sub2MoreThanPercent = false,
                    SubjectSubfromPart1ID = s.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i < 70 s > 70 r1
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความตระหนักต่อหน้าที่ของตนเองในเบื้องต้น ยังไม่สามารถจัดการ กับหน้าที่ภาระงานและความรับผิดชอบที่มีอยู่ได้อย่างเต็มที่ ถึงแม้จะมีความสำนึกต่อประโยชน์ส่วนร่วมในระดับที่ดี ข้อจำกัดข้างต้นจะเป็นอุปสรรคต่อการแสดงพฤติกรรมอันเป็นประโยชน์ต่อสังคม ",
                    Sub1MoreThanPercent = false,
                    Sub2MoreThanPercent = true,
                    SubjectSubfromPart1ID = i.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i < 70 s > 70 r2
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความตระหนักต่อสังคมสูงกว่าความตระหนักต่อตนเอง ยังไม่สามารถจัดการกับหน้าที่ ภาระงาน และความรับผิดชอบของตนที่มีอยู่ได้อย่างเต็มที่ การตัดสินใจมองที่ประโยชน์ส่วนรวมเป็นเบื้องต้น ซึ่งอาจสร้างผลกระทบในทางลบต่อตนเองและหน้าที่ ",
                    Sub1MoreThanPercent = false,
                    Sub2MoreThanPercent = true,
                    SubjectSubfromPart1ID = s.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i > 70 s > 70 r1
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความตระหนักต่อหน้าที่ของตนเองและสังคมอย่างดีเยี่ยม สามารถจัดลำดับความสำคัญ แก้ไขปัญหา และพัฒนาตนเอง พร้อมทั้งเชื่อมโยงตนเองเป็นส่วนหนึ่งของสังคม ไม่สร้างปัญหา และโดยทั่วไปจะไม่เพิกเฉยต่อปัญหาที่เกิดขึ้นต่อสังคม ยกเว้นสถานการณ์ที่การไม่เพิกเฉยอาจจะสร้างปัญหากับตัวเองหรือสิ่งที่ตน ให้ความสำคัญ",
                    Sub1MoreThanPercent = true,
                    Sub2MoreThanPercent = true,
                    SubjectSubfromPart1ID = i.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                // i > 70 s > 70 r2
                setup = new SubjectRSetup
                {
                    SubjectSubID1 = i.ID,
                    SubjectSubID2 = s.ID,
                    Description = "มีความตระหนักต่อหน้าที่ของตนเองและสังคมอย่างดีเยี่ยม สามารถจัดลำดับความสำคัญ แก้ไขปัญหา และพัฒนาตนเอง พร้อมทั้งเชื่อมโยงตนเองเป็นส่วนหนึ่งของสังคม ไม่สร้างปัญหา และไม่เพิกเฉยต่อปัญหาที่เกิดขึ้นต่อสังคมในทุกสถานการณ์ ",
                    Sub1MoreThanPercent = true,
                    Sub2MoreThanPercent = true,
                    SubjectSubfromPart1ID = s.ID,
                    Percent = 70,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectRSetups.Add(setup);

                context.SaveChanges();
            }
            if (context.SubjectESetups != null && !context.SubjectESetups.Any())
            {
                var r = context.SubjectSubs.Where(w => w.Name == "R").FirstOrDefault();
                var setup = new SubjectESetup
                {
                    SubjectSubID = r.ID,
                    DescriptionType1 = "ให้ความสำคัญในการใช้เหตุผล ตรรกะ และการเชื่อมโยงประเด็นต่างๆ ในการสื่อสารเพียงเล็กน้อย",
                    DescriptionType2 = "ให้ความสำคัญในการใช้เหตุผล ตรรกะ และการเชื่อมโยงประเด็นต่างๆ ในการสื่อสารในระดับปานกลาง",
                    DescriptionType3 = "ให้ความสำคัญในการใช้เหตุผล ตรรกะ และการเชื่อมโยงประเด็นต่างๆ ในการสื่อสารอย่างสูง",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    PercentHigh = 75,
                    PercentMid = 50,
                    PercentLow = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectESetups.Add(setup);

                var b = context.SubjectSubs.Where(w => w.Name == "B").FirstOrDefault();
                setup = new SubjectESetup
                {
                    SubjectSubID = b.ID,
                    DescriptionType1 = "ให้ความสำคัญเพียงเล็กน้อยในการใช้คำ รูปแบบประโยคและใจความที่มีความหมายตรงประเด็นเพื่อบรรลุเป้าหมายในการสื่อสาร เพื่อทำให้การสื่อสารนั้น มีความกระชับ",
                    DescriptionType2 = "ให้ความสำคัญในการใช้คำ รูปแบบประโยคและใจความที่มีความหมาย ตรงประเด็นเพื่อบรรลุเป้าหมายในการสื่อสาร เพื่อทำให้การสื่อสารนั้น มีความกระชับในระดับปานกลาง",
                    DescriptionType3 = "ให้ความสำคัญในการใช้คำ รูปแบบประโยคและใจความที่มีความหมาย ตรงประเด็นเพื่อบรรลุเป้าหมายในการสื่อสาร เพื่อทำให้การสื่อสารนั้น มีความกระชับในระดับสูง",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    PercentHigh = 75,
                    PercentMid = 50,
                    PercentLow = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectESetups.Add(setup);

                var w = context.SubjectSubs.Where(w => w.Name == "W").FirstOrDefault();
                setup = new SubjectESetup
                {
                    SubjectSubID = w.ID,
                    DescriptionType1 = "ให้ความสำคัญเล็กน้อยในการสื่อสาร เป็นที่จดจำและจับใจผู้รับสาร จากการใช้คำหรือน้ำเสียงที่ทรงพลังและน่าประทับใจ",
                    DescriptionType2 = "ให้ความสำคัญพอประมาณในการสื่อสาร เป็นที่จดจำและจับใจผู้รับสาร จากการใช้คำหรือน้ำเสียงที่ทรงพลังและน่าประทับใจ",
                    DescriptionType3 = "ให้ความสำคัญเป็นพิเศษในการสื่อสาร เป็นที่จดจำและจับใจผู้รับสาร จากการใช้คำหรือน้ำเสียงที่ทรงพลังและน่าประทับใจ",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    PercentHigh = 75,
                    PercentMid = 50,
                    PercentLow = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectESetups.Add(setup);

                var e = context.SubjectSubs.Where(w => w.Name == "E").FirstOrDefault();
                setup = new SubjectESetup
                {
                    SubjectSubID = e.ID,
                    DescriptionType1 = "ให้ความสำคัญต่อการมีส่วนร่วมระหว่างผู้ส่งสารและผู้รับสาร พยายามสร้างความสัมพันธ์และปฏิสัมพันธ์ที่ดีต่อผู้รับสารเพียงเล็กน้อย",
                    DescriptionType2 = "ให้ความสำคัญต่อการมีส่วนร่วมระหว่างผู้ส่งสารและผู้รับสาร พยายามสร้างความสัมพันธ์และปฏิสัมพันธ์ที่ดีต่อผู้รับสารพอประมาณ",
                    DescriptionType3 = "ให้ความสำคัญต่อการมีส่วนร่วมระหว่างผู้ส่งสารและผู้รับสาร พยายามสร้างความสัมพันธ์และปฏิสัมพันธ์ที่ดีต่อผู้รับสารเป็นอย่างสูง",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    PercentHigh = 75,
                    PercentMid = 50,
                    PercentLow = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectESetups.Add(setup);

                var f = context.SubjectSubs.Where(w => w.Name == "F").FirstOrDefault();
                setup = new SubjectESetup
                {
                    SubjectSubID = f.ID,
                    DescriptionType1 = "ให้ความสำคัญเกี่ยวกับภูมิหลังของผู้รับสาร ทำความเข้าใจผู้รับสารได้ดีก่อนที่จะสื่อสารออกไป มีความระมัดระวังในการเลือกใช้คำ ในการสื่อสารเพียงเล็กน้อย",
                    DescriptionType2 = "ให้ความสำคัญเกี่ยวกับภูมิหลังของผู้รับสาร ทำความเข้าใจผู้รับสารได้ดีก่อนที่จะสื่อสารออกไป มีความระมัดระวังในการเลือกใช้คำ ในการสื่อสารพอสมควร",
                    DescriptionType3 = "ให้ความสำคัญเกี่ยวกับภูมิหลังของผู้รับสาร ทำความเข้าใจผู้รับสารได้ดีก่อนที่จะสื่อสารออกไป มีความระมัดระวังในการเลือกใช้คำ ในการสื่อสารเป็นอย่างสูง",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    PercentHigh = 75,
                    PercentMid = 50,
                    PercentLow = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectESetups.Add(setup);
                context.SaveChanges();
            }
            if (context.SubjectASetups != null && !context.SubjectASetups.Any())
            {
                var u = context.SubjectSubs.Where(w => w.Name == "U").FirstOrDefault();
                var setup = new SubjectASetup
                {
                    SubjectSubID = u.ID,
                    DescriptionType1 = "การแสวงหาความรู้ความเข้าใจในงานศิลปะแขนงต่างๆ จะทำให้ตีความและอ่านความหมายของงานศิลปะได้ดีขึ้น",
                    DescriptionType2 = "มีคุณสมบัติพื้นฐานในการตีความและอ่านความหมายของงานศิลปะได้ ซึ่งในโลกของศิลปะยังมีความรู้และประสบการณ์อีกมากมายให้ค้นหา",
                    DescriptionType3 = "สามารถตีความและอ่านความหมายของงานศิลปะแขนงต่างๆ ได้เป็นอย่างดี ในโลกของศิลปะยังมีความรู้และประสบการณ์อีกมากมายให้ค้นหาเพื่อที่จะถ่ายทอดความเข้าใจนั้นสู่ผู้อื่นได้",
                    MaxPoint = 4,
                    PercentType3 = 90,
                    PercentType2 = 60,
                    PercentType1 = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectASetups.Add(setup);

                var a = context.SubjectSubs.Where(w => w.Name == "A").FirstOrDefault();
                setup = new SubjectASetup
                {
                    SubjectSubID = a.ID,
                    DescriptionType1 = "การสร้างโอกาสให้ตนเองรับรสงานศิลปะมากขึ้น จะได้สัมผัสประสบการณ์ใหม่ๆจากศิลปะแขนงต่างๆ",
                    DescriptionType2 = "มีต้นทุนที่ดีในการรับรสงานศิลปะ และสามารถพัฒนาตนเอง เพื่อรับความละเมียดละไมในการเสพศิลปะแขนงต่างๆ ต่อไปได้",
                    DescriptionType3 = "มีความละเอียดอ่อนละเมียดละไมในการรับรสงานศิลปะและสามารถเข้าถึง ความอิ่มเอมจากการเสพงานศิลปะได้เป็นอย่างดี",
                    MaxPoint = 4,
                    PercentType3 = 90,
                    PercentType2 = 60,
                    PercentType1 = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectASetups.Add(setup);

                var p = context.SubjectSubs.Where(w => w.Name == "P").FirstOrDefault();
                setup = new SubjectASetup
                {
                    SubjectSubID = p.ID,
                    DescriptionType1 = "การฝึกฝนสร้างความเข้าใจ ความซาบซึ้งในงานศิลปะ จะทำให้สามารถเลือกใช้ศิลปะแขนงต่างๆ สร้างสัมผัส สร้างความสุข ให้กับตนเองและผู้อื่นได้",
                    DescriptionType2 = "มีความเข้าใจ มีความซาบซึ้งในงานศิลปะ พร้อมเลือกใช้ ศิลปะแขนงต่างๆ สร้างสัมผัส สร้างความสุขให้กับตนเองและผู้อื่นได้ดี",
                    DescriptionType3 = "มีความสามารถใช้ความเข้าใจ ความซาบซึ้งในงานศิลปะ รู้จักเลือกใช้ศิลปะแขนงต่างๆ สร้างสัมผัส สร้างความสุขและ คุณภาพชีวิตที่ดีให้กับตนเองและผู้อื่นได้",
                    MaxPoint = 4,
                    PercentType3 = 90,
                    PercentType2 = 60,
                    PercentType1 = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectASetups.Add(setup);
                context.SaveChanges();
            }
            if (context.SubjectTSetups != null && !context.SubjectTSetups.Any())
            {
                var l = context.SubjectSubs.Where(w => w.Name == "L").FirstOrDefault();
                var setup = new SubjectTSetup
                {
                    SubjectSubID = l.ID,
                    DescriptionType1 = "มีแนวโน้มที่จะไม่แสดงตนเป็นผู้นำ มีความรักสันโดษ สงวนท่าทีถ้าต้องปฏิสัมพันธ์กับผู้อื่น รู้สึกสบายใจที่จะทำงานคนเดียว ชอบที่จะใช้กระบวนการแก้ปัญหาที่คุ้นชิน ในกรณีที่ได้รับมอบหมายให้เป็นผู้นำ มีแนวโน้มจะปล่อยให้สมาชิกในทีมมีอิสระเต็มที่ในการทำงาน และไม่ทำอะไรที่จะก่อให้เกิดการเผชิญหน้าหรือความขัดแย้งกับสมาชิกในทีม มักปล่อยให้สมาชิกทำงานไปตามความคิดเห็นของตนเองโดยไม่ต้องปรึกษาหารือกับตนก่อน",
                    DescriptionType2 = "มีแนวโน้มที่จะแสดงบทบาทเป็นทั้งผู้นำและผู้ตามได้ ชอบที่จะ ตัดสินใจด้วยตนเองแต่ก็สามารถรับฟังความคิดเห็นของสมาชิกในทีม สามารถที่จะอยู่ตามลำพังได้แต่ก็ไม่รู้สึกอึดอัดเกินไปที่จะสร้างปฏิสัมพันธ์กับผู้อื่น มีแนวโน้มที่จะแก้ปัญหาตามสถานการณ์โดยใช้ประสบการณ์ เดิมร่วมกับความคิดสร้างสรรค์",
                    DescriptionType3 = "มีแนวโน้มที่จะแสดงตนเป็นผู้นำสูง มีความกล้าเสี่ยงทำสิ่งใหม่ๆ เมื่อต้องตัดสินใจจะศึกษาข้อมูลและคิดหาวิธีการที่เหมาะสมในการทำงานให้สำเร็จ เห็นความสำคัญของการมีความสัมพันธ์ที่ดีกับเพื่อนร่วมงาน และมีการบริหารงานด้วยวิธีการกระจายอำนาจ ในขณะเดียวกันก็มีความกล้าที่จะแสดงความคิดเห็นอย่างตรงไปตรงมา",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectTSetups.Add(setup);

                var m = context.SubjectSubs.Where(w => w.Name == "M").FirstOrDefault();
                setup = new SubjectTSetup
                {
                    SubjectSubID = m.ID,
                    DescriptionType1 = "ในการทำงานเป็นทีม มีแนวโน้มที่จะทำงานแบบต่างคนต่างทำ การแบ่งบทบาทความรับผิดชอบเป็นแบบไม่กำหนดตายตัว มีการสื่อสารกันระหว่างสมาชิกในทีมน้อยหรือมักสื่อสารอย่างเป็นทางการเท่านั้น ทำให้สมาชิกในทีมขาดการตัดสินใจร่วมกัน",
                    DescriptionType2 = "เมื่อต้องทำงานเป็นทีม มีแนวโน้มที่จะแบ่งบทบาทหน้าที่ของสมาชิก อย่างชัดเจน มีแนวโน้มที่จะแบ่งปันข้อมูลและทรัพยากร กับเพื่อนในกลุ่ม มีการพูดคุยและตัดสินใจร่วมกับสมาชิกคนอื่นๆ ในทีมเมื่อถึงเวลาจำเป็น ค่อนข้างให้ความสำคัญกับการสื่อสารระหว่างสมาชิก โดยการสนับสนุนให้เพื่อนในกลุ่มพูดคุยกันตามความเหมาะสมและจัดลำดับความสำคัญในเรื่องที่สื่อสาร เมื่อต้องตัดสินใจเกี่ยวกับการทำงานกลุ่ม จะให้โอกาสสมาชิกทุกคนออกเสียงและออกความเห็น",
                    DescriptionType3 = "มีความเชื่อว่าในการทำงานเป็นทีม สมาชิกทุกคนถือเป็นทีมเดียวกัน ให้ความสำคัญกับการสื่อสารระหว่างสมาชิกและการสร้างความเชื่อมั่น และความไว้วางใจระหว่างกัน ในสถานการณ์ที่ต้องตัดสินใจร่วมกัน มุ่งที่จะได้ข้อตกลงที่สมาชิกทุกคนเห็นชอบร่วมกัน",
                    MaxPoint = 4,
                    PercentType3 = 75,
                    PercentType2 = 50,
                    PercentType1 = 0,
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectTSetups.Add(setup);

                context.SaveChanges();
            }
            if (context.SubjectSSetups != null && !context.SubjectSSetups.Any())
            {
                var d = context.SubjectSubs.Where(w => w.Name == "D").FirstOrDefault();
                var setup = new SubjectSSetup
                {
                    SubjectSubID = d.ID,
                    DescriptionType1 = "เข้าใจคุณสมบัติพลเมืองบางข้อ และสามารถพัฒนาตนเองได้ ด้วยการศึกษาและทำความเข้าใจเพื่อเปลี่ยนทัศนคติของตัวเอง",
                    DescriptionType2 = "มีความเข้าใจคุณสมบัติพื้นฐานพลเมืองครบทั้ง 6 ข้อ หากต้องการพัฒนาขึ้นอีก อาจศึกษาเรื่องการวิเคราะห์ และเข้าใจสังคมด้วยตัวเอง",
                    DescriptionType3 = "นอกจากมีคุณสมบัติพื้นฐานพลเมืองครบทั้ง 6 ข้อแล้ว ยังมีความสามารถในการเข้าใจและวิเคราะห์สังคมได้ดีถึงขั้นอาจจะเป็น พลเมืองชั้นนำ มีอุดมการณ์แน่วแน่และสามารถเป็นแบบอย่าง ให้แก่ผู้คนในสังคมได้ ขอให้ทำเพื่อผู้อื่นต่อไป",
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectSSetups.Add(setup);

                var v = context.SubjectSubs.Where(w => w.Name == "V").FirstOrDefault();
                setup = new SubjectSSetup
                {
                    SubjectSubID = v.ID,
                    DescriptionType1 = "บางครั้งมีแนวโน้มที่อาจจะละเลยเรื่องราวในสังคมหรืออาจให้ความสำคัญต่อตนเองมากกว่าสังคมรอบตัว ควรหาโอกาสเข้าร่วมกิจกรรมจิตอาสาหรือค่ายพัฒนาตนเองให้มากขึ้น",
                    DescriptionType2 = "มีความสนใจทำกิจกรรมที่เป็นประโยชน์ต่อผู้อื่น บางครั้งไม่เพียงแต่เข้าร่วมแต่ถึงขั้นลงมือช่วยจัดการกิจกรรมต่างๆ ในการพัฒนาต่อไปอาจท้าทายตัวเองด้วยการลองเป็นผู้จัดกิจกรรมจิตอาสา หรือจัดค่ายด้วยตัวเองบ้าง",
                    DescriptionType3 = "มีความเป็นผู้จัดกิจกรรมจิตอาสามากกว่าเป็นอาสาสมัครเฉยๆ และมุ่งเน้นการทำกิจกรรมเพื่อผู้อื่น มุ่งที่จะสร้างคุณค่า ให้แก่ผู้ด้อยโอกาสกว่าเสมอ โดยเน้นที่การแก้ปัญหาอย่างมีประสิทธิผล",
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectSSetups.Add(setup);

                var g = context.SubjectSubs.Where(w => w.Name == "G").FirstOrDefault();
                setup = new SubjectSSetup
                {
                    SubjectSubID = g.ID,
                    DescriptionType1 = "บางครั้งอาจมองเห็นปัญหาสังคมต่างๆ ที่ทำให้หงุดหงิด ไม่สบายใจ มักหาโอกาสในการเล่าเรื่องราวนี้ให้คนรอบข้างฟังอยู่เสมอ คงอาจจะดีไม่น้อยถ้าลุกขึ้นมาเปลี่ยนแปลงสังคมโดยเริ่มต้นที่ตัวเองก่อน เพราะทุกปัญหาล้วนเกี่ยวข้องสัมพันธ์กันเสมอ",
                    DescriptionType2 = "มีมุมมองในการแก้ปัญหาต่างๆ ในสังคมที่คิดเริ่มต้นจากตัวเอง เพราะการเปลี่ยนแปลงที่ตัวเรานั้นง่ายที่สุด ต่อไปอาจเริ่มต้นด้วยการชวนคนรอบข้างให้มาเป็นแนวร่วมเดียวกันในการแก้ปัญหา เพราะหลายๆ มือช่วยกันย่อมดีกว่าเสมอ",
                    DescriptionType3 = "นอกจากจะแก้ปัญหาต่างๆ โดยเริ่มเปลี่ยนแปลงตนเองและ คนรอบข้างแล้ว ก็มักจะเชิญชวนคนอื่นๆ ในสังคมให้ลุกมาช่วยกันเปลี่ยนแปลงสังคมอีกด้วย โดยการแก้ปัญหามักมองไปที่รากของปัญหา โดยไม่ยึดติดกรอบเดิมที่มีอยู่ ทำให้ปัญหาได้รับการแก้ไขอย่างยั่งยืน",
                    Create_On = DateUtil.Now(),
                    Create_By = "system",
                    Update_On = DateUtil.Now(),
                    Update_By = "system"
                };
                context.SubjectSSetups.Add(setup);
                context.SaveChanges();
            }
            if (context.Users != null && !context.Users.Any())
            {
                var user = new User { UserName = "admin", Password = DataEncryptor.Encrypt("admin123"), Create_On = DateUtil.Now(), Create_By = "system", Update_On = DateUtil.Now(), Update_By = "system" };
                var staff = new Staff() { FirstName = "admin", LastName = "super", Create_On = DateUtil.Now(), Create_By = "system", Update_On = DateUtil.Now(), Update_By = "system" };
                staff.User = user;
                context.Staffs.Add(staff);
                context.SaveChanges();
            }
            if (context.Facultys != null && !context.Facultys.Any())
            {
                var List = new List<Faculty>
                {
                    new Faculty { FacultyName ="คณะเภสัชศาสตร์"  },
                    new Faculty { FacultyName ="คณะเศรษฐศาสตร์"  },
                    new Faculty { FacultyName ="คณะแพทยศาสตร์"  },
                    new Faculty { FacultyName ="คณะทันตแพทยศาสตร์"  },
                    new Faculty { FacultyName ="คณะนิติศาสตร์"  },
                    new Faculty { FacultyName ="คณะพยาบาลศาสตร์"  },
                    new Faculty { FacultyName ="คณะพาณิชยศาสตร์และการบัญชี"  },
                    new Faculty { FacultyName ="คณะรัฐศาสตร์"  },
                    new Faculty { FacultyName ="คณะวารสารศาสตร์และสื่อสารมวลชน"  },
                    new Faculty { FacultyName ="คณะวิทยาการเรียนรู้และศึกษาศาสตร์"  },
                    new Faculty { FacultyName ="คณะวิทยาศาสตร์และเทคโนโลยี"  },
                    new Faculty { FacultyName ="คณะวิศวกรรมศาสตร์"  },
                    new Faculty { FacultyName ="คณะศิลปกรรมศาสตร์"  },
                    new Faculty { FacultyName ="คณะศิลปศาสตร์"  },
                    new Faculty { FacultyName ="คณะสถาปัตยกรรมศาสตร์และการผังเมือง"  },
                    new Faculty { FacultyName ="คณะสหเวชศาสตร์"  },
                    new Faculty { FacultyName ="คณะสังคมวิทยาและมานุษยวิทยา"  },
                    new Faculty { FacultyName ="คณะสังคมสงเคราะห์ศาสตร์"  },
                    new Faculty { FacultyName ="คณะสาธารณสุขศาสตร์"  },
                    new Faculty { FacultyName ="วิทยาลัยแพทยศาสตร์นานาชาติจุฬาภรณ์"  },
                    new Faculty { FacultyName ="วิทยาลัยนวัตกรรม"  },
                    new Faculty { FacultyName ="วิทยาลัยนานาชาติ ปรีดี พนมยงค์"  },
                    new Faculty { FacultyName ="วิทยาลัยพัฒนศาสตร์ ป๋วย อึ๊งภากรณ์"  },
                    new Faculty { FacultyName ="สถาบันเทคโนโลยีนานาชาติสิรินธร"  }
                };
                List = List.OrderBy(o => o.FacultyName).ToList();
                List.ForEach(s => context.Facultys.Add(s));
                context.SaveChanges();
            }
        }

        public static void UpdateDatabaseDescriptions(TuExamContext context)
        {
            var contextType = typeof(TuExamContext);
            var props = contextType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.PropertyType.GetGenericArguments().Length == 0)
                    continue;

                var tableType = prop.PropertyType.GetGenericArguments()[0];

                string fullTableName = prop.Name;
                Regex regex = new Regex(@"(\[\w+\]\.)?\[(?<table>.*)\]");
                Match match = regex.Match(fullTableName);
                string tableName;
                if (match.Success)
                    tableName = match.Groups["table"].Value;
                else
                    tableName = fullTableName;

                foreach (var prop2 in tableType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var attrs = prop2.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var columnName = prop2.Name;
                        var description = ((DisplayAttribute)attrs[0]).Name;
                        string strGetDesc = "select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table',N'" + tableName + "','column',null) where objname = N'" + columnName + "';";
                        using (var command = context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = strGetDesc;
                            command.CommandType = CommandType.Text;

                            context.Database.OpenConnection();

                            object val = null;
                            using (var result = command.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    val = result;
                                }
                            }
                            if (val == null)
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_addextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            else
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_updateextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            context.Database.CloseConnection();
                        }

                    }
                }
            }
        }


    }
}
